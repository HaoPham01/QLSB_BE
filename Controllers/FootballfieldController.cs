using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Drawing;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.X9;
using QLSB_APIs.DTO;
using QLSB_APIs.Models.Entities;
using QLSB_APIs.Services;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace QLSB_APIs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FootballfieldController : ControllerBase
    {
        private readonly IFootballfieldService _footballfieldService;
        private readonly MyDbContext _dbContext;
        private readonly IMapper _mapper;

        public FootballfieldController(IFootballfieldService footballfieldService, MyDbContext dbContext, IMapper mapper)
        {
            _footballfieldService = footballfieldService;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet("get-footballfield")]
        public IActionResult GetFootballfieldList()
        {
            try
            {
                var footballFieldsWithAdminNames = _dbContext.Footballfields
                            .Join(
                                _dbContext.Admins,
                                footballfield => footballfield.AdminId,
                                admin => admin.AdminId,
                                (footballfield, admin) => new
                                {
                                    footballfield.FieldId,
                                    footballfield.FieldName,
                                    footballfield.Type,
                                    footballfield.Content,
                                    footballfield.Status,
                                    footballfield.CreateDate,
                                    footballfield.UpdateDate,
                                    FullName = admin.FullName
                                }
                            ).Where(item => item.Status != 0)
                            .ToList();

                return Ok(footballFieldsWithAdminNames);
            }
            catch (Exception ex)
            {
                return BadRequest("not good");
            }
        }


        [HttpPost("add-footballfield")]
        public IActionResult AddFootballfield(FootballfieldDTO footballfieldDTO)
        {
            var existingField = _dbContext.Footballfields
                                      .FirstOrDefault(f => f.FieldName == footballfieldDTO.FieldName
                                                      );

            if (existingField != null)
            {
                return BadRequest("Tên sân đã tồn tại");
            }
            Footballfield newField = new Footballfield
            {
                AdminId = footballfieldDTO.AdminId,
                FieldName = footballfieldDTO.FieldName,
                Type = footballfieldDTO.Type,
                Content = footballfieldDTO.Content,
                Status = footballfieldDTO.Status,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };

            _dbContext.Footballfields.Add(newField);
            _dbContext.SaveChanges();

            // Gọi hàm AddPrice
            _dbContext.Database.ExecuteSqlRaw("Call AddPrice({0})", newField.FieldId);
            return Ok(newField);
        }



        [HttpPut("put-footballfield")]
        public IActionResult Put([FromBody] FootballfieldDTO footballfieldObj)
        {
            var footballfield = _dbContext.Footballfields.Find(footballfieldObj.FieldId);
            if (footballfield == null)
            {
                return BadRequest("Sân bóng không tồn tại");
            }
            var existingField = _dbContext.Footballfields
                                      .FirstOrDefault(f => f.FieldName == footballfieldObj.FieldName
                                                       && f.FieldId != footballfieldObj.FieldId);

            if (existingField != null)
            {
                return BadRequest("Tên sân đã tồn tại");
            }

            footballfield.FieldName = footballfieldObj.FieldName;
            footballfield.Type = footballfieldObj.Type;
            footballfield.Content = footballfieldObj.Content;
            footballfield.Status = footballfieldObj.Status; 
            footballfield.UpdateDate = DateTime.Now;
            _dbContext.SaveChanges();

            return Ok(footballfieldObj);
        }


        [HttpPost("delete-footballfield")]
        public IActionResult DeleteFootballfield([FromBody] FootballfieldDTO footballfieldDTO)
        {
            try
            {
                _footballfieldService.DeleteFootballfield(footballfieldDTO);
                return Ok(footballfieldDTO);
            }
            catch (Exception ex)
            {
                return BadRequest("Xóa thất bại");
            }

        }

        [HttpGet("search/{keyword}")]
        public IActionResult Search(string keyword)
        {
            var footballFieldsWithAdminNames = _dbContext.Footballfields
            .Join(
                _dbContext.Admins,
                footballfield => footballfield.AdminId,
                admin => admin.AdminId,
                (footballfield, admin) => new
                {
                    footballfield.FieldId,
                    footballfield.FieldName,
                    footballfield.Type,
                    footballfield.Status,
                    footballfield.CreateDate,
                    footballfield.UpdateDate,
                    FullName = admin.FullName
                }
            ).Where(item => item.Status != 0 && item.FieldName.Contains(keyword))
            .ToList();
            return Ok(footballFieldsWithAdminNames);
        }


        [HttpGet("get-name-field-by-id/{id}")]
        public IActionResult GetNameFieldById(int id)
        {
            var field = _dbContext.Footballfields.FirstOrDefault(u => u.FieldId == id);
            if (field == null)
                return BadRequest("Không tồn tại sân");
            return Ok(field);
        }





        [HttpGet("get-field-active")]
        public IActionResult GetFieldActive()
        {
            try
            {
                var footballFieldsWithAdminNames = _dbContext.Footballfields
                    .Join(
                        _dbContext.Fieldimages,
                        footballfield => footballfield.FieldId,
                        field => field.FieldId,
                        (footballfield, field) => new
                        {
                            footballfield.FieldId,
                            footballfield.FieldName,
                            footballfield.Type,
                            footballfield.Content,
                            footballfield.Status,
                            footballfield.CreateDate,
                            footballfield.UpdateDate,
                            imageUrl = field.ImageUrl
                        }
                    )
                    .GroupBy(item => new
                    {
                        item.FieldId,
                        item.FieldName,
                        item.Type,
                        item.Content,
                        item.Status,
                        item.CreateDate,
                        item.UpdateDate
                    })
                    .Select(group => new
                    {
                        FieldId = group.Key.FieldId,
                        FieldName = group.Key.FieldName,
                        Type = group.Key.Type,
                        Content = group.Key.Content,
                        Status = group.Key.Status,
                        CreateDate = group.Key.CreateDate,
                        UpdateDate = group.Key.UpdateDate,
                        ImageUrl = group.FirstOrDefault().imageUrl
                    })
                    .Where(item => item.Status == 1)
                    .ToList();

                var PricesByField = _dbContext.Footballfields
                .Where(field => field.Status == 1)
                .GroupJoin(
                    _dbContext.Prices,
                    field => field.FieldId,
                    price => price.FieldId,
                    (field, prices) => new
                    {
                        FieldName = field.FieldName,
                        MinPrice = prices.Min(price => price.Price1),
                        MaxPrice = prices.Max(price => price.Price1)
                    }
                )
                .ToList();


                var combinedList = (from f in footballFieldsWithAdminNames
                                    join p in PricesByField on f.FieldName equals p.FieldName
                                    select new
                                    {
                                        f.FieldId,
                                        f.FieldName,
                                        f.Type,
                                        f.Content,
                                        f.Status,
                                        f.CreateDate,
                                        f.UpdateDate,
                                        f.ImageUrl,
                                        p.MinPrice,
                                        p.MaxPrice
                                    }).ToList();

                return Ok(combinedList);
            }
            catch (Exception ex)
            {
                return BadRequest("not good");
            }
        }


        [HttpGet("get-field-by-id/{id}")]
        public IActionResult GetFieldById(int id)
        {
            try
            {
                var footballFieldsWithAdminNames = _dbContext.Footballfields
                    .Join(
                        _dbContext.Admins,
                        footballfield => footballfield.AdminId,
                        admin => admin.AdminId,
                        (footballfield, admin) => new
                        {
                            footballfield.FieldId,
                            footballfield.FieldName,
                            footballfield.Type,
                            footballfield.Content,
                            footballfield.Status,
                            footballfield.CreateDate,
                            footballfield.UpdateDate,
                            FullName = admin.FullName
                        }
                    )
                    .Where(item => item.Status != 0 && item.FieldId == id)
                    .ToList();

                // Lấy giá thấp nhất và cao nhất
                var minPrice = _dbContext.Prices
                    .Where(price => price.FieldId == id)
                    .Min(price => price.Price1);

                var maxPrice = _dbContext.Prices
                    .Where(price => price.FieldId == id)
                    .Max(price => price.Price1);

                // Thêm thông tin giá vào kết quả
                var result = footballFieldsWithAdminNames.Select(item => new
                {
                    item.FieldId,
                    item.FieldName,
                    item.Type,
                    item.Content,
                    item.Status,
                    item.CreateDate,
                    item.UpdateDate,
                    item.FullName,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("not good");
            }
        }

        [HttpGet("search-field-active/{keyword}")]
        public IActionResult SearchFieldActive(string keyword)
        {
            try
            {
                var footballFieldsWithAdminNames = _dbContext.Footballfields
                    .Join(
                        _dbContext.Fieldimages,
                        footballfield => footballfield.FieldId,
                        field => field.FieldId,
                        (footballfield, field) => new
                        {
                            footballfield.FieldId,
                            footballfield.FieldName,
                            footballfield.Type,
                            footballfield.Content,
                            footballfield.Status,
                            footballfield.CreateDate,
                            footballfield.UpdateDate,
                            imageUrl = field.ImageUrl
                        }
                    )
                    .GroupBy(item => new
                    {
                        item.FieldId,
                        item.FieldName,
                        item.Type,
                        item.Content,
                        item.Status,
                        item.CreateDate,
                        item.UpdateDate
                    })
                    .Select(group => new
                    {
                        FieldId = group.Key.FieldId,
                        FieldName = group.Key.FieldName,
                        Type = group.Key.Type,
                        Content = group.Key.Content,
                        Status = group.Key.Status,
                        CreateDate = group.Key.CreateDate,
                        UpdateDate = group.Key.UpdateDate,
                        ImageUrl = group.FirstOrDefault().imageUrl
                    })
                    .Where(item => item.Status == 1 &&
                           item.FieldName.Contains(keyword)) // Tìm kiếm theo FieldName, Type và Content
                    .ToList();

                var PricesByField = _dbContext.Footballfields
                    .Where(field => field.Status == 1 && field.FieldName.Contains(keyword))
                    .GroupJoin(
                        _dbContext.Prices,
                        field => field.FieldId,
                        price => price.FieldId,
                        (field, prices) => new
                        {
                            FieldName = field.FieldName,
                            MinPrice = prices.Min(price => price.Price1),
                            MaxPrice = prices.Max(price => price.Price1)
                        }
                    )
                    .ToList();

                var combinedList = (from f in footballFieldsWithAdminNames
                                    join p in PricesByField on f.FieldName equals p.FieldName
                                    select new
                                    {
                                        f.FieldId,
                                        f.FieldName,
                                        f.Type,
                                        f.Content,
                                        f.Status,
                                        f.CreateDate,
                                        f.UpdateDate,
                                        f.ImageUrl,
                                        p.MinPrice,
                                        p.MaxPrice
                                    }).ToList();

                return Ok(combinedList);
            }
            catch (Exception ex)
            {
                return BadRequest("not good");
            }
        }



    }

}

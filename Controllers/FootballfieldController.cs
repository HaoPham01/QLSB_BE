using System.Data.Entity;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
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
        public IActionResult AddFootballfield([FromBody] FootballfieldDTO footballfieldDTO)
        {
            _footballfieldService.AddFootballfield(footballfieldDTO);
            return Ok(footballfieldDTO);
        }



        [HttpPut("put-footballfield")]
        public IActionResult Put([FromBody] FootballfieldDTO footballfieldObj)
        {
            var footballfield = _dbContext.Footballfields.Find(footballfieldObj.FieldId);
            if (footballfield == null)
            {
                return BadRequest("Footballfield không tồn tại");
            }

            footballfield.FieldName = footballfieldObj.FieldName;
            footballfield.Type = footballfieldObj.Type;
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
    }

}

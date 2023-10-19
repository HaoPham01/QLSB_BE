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
    public class FieldimageController : ControllerBase
    {
        private readonly IFieldimageService _fieldimageService;
        private readonly MyDbContext _dbContext;
        private readonly IMapper _mapper;

        public FieldimageController(IFieldimageService fieldimageService, MyDbContext dbContext, IMapper mapper)
        {
            _fieldimageService = fieldimageService;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet("get-fieldimage")]
        public IActionResult GetFieldImageList()
        {
            try
            {
                var footballFieldsWithAdminNames = _dbContext.Fieldimages;
                return Ok(footballFieldsWithAdminNames);
            }
            catch (Exception ex)
            {
                return BadRequest("not good");
            }
        }

        [HttpPost("add-fieldimageurl")]
        public IActionResult AddFootballfield([FromBody] FieldimageDTO footballfieldDTO)
        {
            _fieldimageService.AddFieldImageUrl(footballfieldDTO);
            return Ok(footballfieldDTO);
        }

        [HttpPost("add-fieldimage")]
        public IActionResult UploadImage([FromForm] IFormFile file)
        {
            try
            {
                // getting file original name
                string FileName = file.FileName;

                // combining GUID to create unique name before saving in wwwroot
                string uniqueFileName = Guid.NewGuid().ToString() + FileName;

                // getting full path inside wwwroot/images
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/", FileName);

                // copying file
                file.CopyTo(new FileStream(imagePath, FileMode.Create));

                return Ok("File Uploaded Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("File Uploaded Fail");
            }
        }
    }
}

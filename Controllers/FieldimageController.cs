using System.Drawing;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLSB_APIs.DTO;
using QLSB_APIs.Models.Entities;
using QLSB_APIs.Services;

namespace QLSB_APIs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FieldImageController : ControllerBase
    {


        private readonly MyDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public FieldImageController(MyDbContext dbContext, IMapper mapper, IImageService imageService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _imageService = imageService;
        }

        [HttpGet("get-image/{fieldId}")]
        public IActionResult GetImage(int fieldId)
        {
            var fieldImage = _dbContext.Fieldimages
                .Where(image => image.FieldId == fieldId && image.ImageUrl != null)
                .ToList();
            if (fieldImage != null) {
                var image = _mapper.Map<List<ImageDTO>>(fieldImage);
                return Ok(image); 
            }    
            else return BadRequest(new { 
                message = "không tồn tại"
            });
         
        }


        [HttpPost("add-image/{fieldId}")]
        public IActionResult AddProperty(IFormFile file, int fieldId)
        {
          var result = _imageService.UploadPhotoAsync(file);
          if (result.Error != null)
            return BadRequest(result.Error.Message);
            var image = new Fieldimage
            {
                ImageUrl = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                FieldId = fieldId
            };
            _dbContext.Fieldimages.Add(image);
            _dbContext.SaveChanges();
          return Ok(new
          {
              message = "Thêm thành công"
          });

        }

        [HttpDelete("delete-image/{id}/{publicId}")]
        public IActionResult DeleteProperty(int id, string publicId)
        {
            var img = _dbContext.Fieldimages.Find(id);
            if (img == null)
            {
                return BadRequest("không tồn tại");
            }
            var result = _imageService.DeletePhotoAsync(publicId);
            _dbContext.Fieldimages.Remove(img);
            _dbContext.SaveChanges();
            return Ok(new 
            {
                result = result,
                message = "Xóa thành công"
            });
        }
    }
}
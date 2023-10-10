
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QLSB_APIs.DTO;
using QLSB_APIs.Models.Entities;

namespace QLSB_APIs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PriceController : ControllerBase
    {

        private readonly MyDbContext _dbContext;

        private readonly IMapper _mapper;
        public PriceController(MyDbContext dbContext, IMapper mapper)
        {

            _dbContext = dbContext;
            _mapper = mapper;
        }


        [HttpGet("get-price-field/{idfield}")]
        public IActionResult GetPriceField(int idfield)
        {
            var priceField = _dbContext.Prices
                .Where(item => item.FieldId == idfield)
            .ToList();
            var priceDTOs = _mapper.Map<List<PriceDTO>>(priceField);
            return Ok(priceDTOs);
        }


        [HttpPut("put-price")]
        public IActionResult PutPrice([FromBody] PriceDTO priceDTO)
        {
            var pricefield = _dbContext.Prices.Find(priceDTO.PriceId);
            if (pricefield == null)
            {
                return BadRequest("Lỗi không tồn tại");
            }

            pricefield.StartTime = priceDTO.StartTime;
            pricefield.EndTime = priceDTO.EndTime;
            pricefield.Price1 = priceDTO.Price1;
            pricefield.UpdateDate = DateTime.Now;
            _dbContext.SaveChanges();

            return Ok(priceDTO);
        }


    }
}

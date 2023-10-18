using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLSB_APIs.DTO;
using QLSB_APIs.Models.Entities;

namespace QLSB_APIs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceController : ControllerBase
    {


        private readonly MyDbContext _dbContext;
        private readonly IMapper _mapper;

        public ServiceController(MyDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet("get-service/{bookingId}")]
        public IActionResult GetService(int bookingId)
        {
            var service = _dbContext.Services
                .Select(
                    service => new
                    {
                        service.SvId,
                        service.BookingId,
                        service.Type,
                        service.Quantity,
                        service.Price,
                        totalPrice = service.Price * service.Quantity
                    }
                ).Where(item => item.BookingId == bookingId)
                .ToList();
            //var totalPrices = service.Sum(item => item.totalPrice);
            return Ok(service);
        }

        [HttpPost("add-service")]
        public IActionResult AddService([FromBody] ServiceDTO service)
        {
            if (service == null)
                return BadRequest();
            var newService = new Service
            {
                BookingId = service.BookingId,
                Type = service.Type,
                Price = service.Price,
                Quantity = service.Quantity
            };

            _dbContext.Services.Add(newService);
            _dbContext.SaveChanges();
            return Ok(service);
        }

        [HttpGet("remove-service/{svId}")]
        public IActionResult RemoveService(int svId)
        {
            var service = _dbContext.Services.Find(svId);
            if (service == null)
            {
                return BadRequest("không tồn tại");
            }
            _dbContext.Services.Remove(service);
            _dbContext.SaveChanges();
            return Ok(new ResultDTO()
            {
                message = "xóa thành công"
            });
        }
    }
}

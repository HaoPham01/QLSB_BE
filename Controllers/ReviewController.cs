using Microsoft.AspNetCore.Mvc;
using QLSB_APIs.Models.Entities;

namespace QLSB_APIs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReviewController : ControllerBase
    {


        [HttpGet]
        public IEnumerable<Review> Get()
        {
            MyDbContext context = new MyDbContext();
            return context.Reviews.ToList();
        }
    }
}

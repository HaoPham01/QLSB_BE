using Microsoft.AspNetCore.Mvc;
using QLSB_APIs.Models.Entities;

namespace QLSB_APIs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewsController : ControllerBase
    {


        [HttpGet]
        public IEnumerable<News> Get()
        {
            MyDbContext context = new MyDbContext();
            return context.News.ToList();
        }
    }
}

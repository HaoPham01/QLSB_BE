using Microsoft.AspNetCore.Mvc;
using QLSB_APIs.Models.Entities;

namespace QLSB_APIs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FavoritefieldController : ControllerBase
    {


        [HttpGet]
        public IEnumerable<Favoritefield> Get()
        {
            MyDbContext context = new MyDbContext();
            return context.Favoritefields.ToList();
        }
    }
}

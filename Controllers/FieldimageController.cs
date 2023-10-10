using Microsoft.AspNetCore.Mvc;
using QLSB_APIs.Models.Entities;

namespace QLSB_APIs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FieldimageController : ControllerBase
    {


        [HttpGet]
        public IEnumerable<Fieldimage> Get()
        {
            MyDbContext context = new MyDbContext();
            return context.Fieldimages.ToList();
        }
    }
}

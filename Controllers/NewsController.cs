using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QLSB_APIs.DTO;
using QLSB_APIs.Models.Entities;
using QLSB_APIs.Services;

namespace QLSB_APIs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewsController : ControllerBase
    {


        private readonly MyDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public NewsController(MyDbContext dbContext, IMapper mapper, IImageService imageService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _imageService = imageService;
        }


        [HttpGet("get-news")]
        public IActionResult GetNewsList()
        {
            try
            {
                var newsWithAdminNames = _dbContext.News
                            .Join(
                                _dbContext.Admins,
                                news => news.AdminId,
                                admin => admin.AdminId,
                                (news, admin) => new
                                {
                                    news.NewsId,
                                    news.Title,
                                    news.Content,
                                    news.Url,
                                    news.PublicId,
                                    news.CreateDate,
                                    news.UpdateDate,
                                    FullName = admin.FullName
                                }
                            )
                            .ToList();

                return Ok(newsWithAdminNames);
            }
            catch (Exception ex)
            {
                return BadRequest("not good");
            }
        }


        [HttpPost("add-news")]
        public IActionResult AddNews(NewsDTO newsDTO)
        {
            var existingNews = _dbContext.News
                                      .FirstOrDefault(f => f.Title == newsDTO.Title
                                                      );

            if (existingNews != null)
            {
                return BadRequest("Tiêu đề đã tồn tại");
            }
            News newsField = new News
            {
                AdminId = newsDTO.AdminId,
                Title = newsDTO.Title,
                Content = newsDTO.Content,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };
            _dbContext.News.Add(newsField);
            _dbContext.SaveChanges();
            return Ok(newsField);
        }



        [HttpPut("put-news")]
        public IActionResult Put([FromBody] NewsDTO newsObj)
        {
            var news = _dbContext.News.Find(newsObj.NewsId);
            if (news == null)
            {
                return BadRequest("Tin tức không tồn tại");
            }
            var existingField = _dbContext.News
                                      .FirstOrDefault(f => f.Title == newsObj.Title
                                                       && f.NewsId != newsObj.NewsId);

            if (existingField != null)
            {
                return BadRequest("Tiêu đề đã tồn tại");
            }

            news.Title = newsObj.Title;
            news.Content = newsObj.Content;
            news.UpdateDate = DateTime.Now;
            _dbContext.SaveChanges();

            return Ok(newsObj);
        }


        [HttpPost("delete-news")]
        public IActionResult DeleteNews([FromBody] NewsDTO newsDTO)
        {
            try
            {
                var news = _dbContext.News.Find(newsDTO.NewsId);
                if (news == null)
                {
                    return BadRequest("Tin tức không tồn tại");
                }
                _dbContext.News.Remove(news);
                _dbContext.SaveChanges();
                return Ok(new ResultDTO()
                {
                    message = "xóa thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Xóa thất bại");
            }

        }

        [HttpGet("search/{keyword}")]
        public IActionResult Search(string keyword)
        {
            var newsWithAdminNames = _dbContext.News
            .Join(
                _dbContext.Admins,
                news => news.AdminId,
                admin => admin.AdminId,
                (news, admin) => new
                {
                    news.NewsId,
                    news.Title,
                    news.Content,
                    news.Url,
                    news.PublicId,
                    news.CreateDate,
                    news.UpdateDate,
                    FullName = admin.FullName
                }
            ).Where(item => item.Title.Contains(keyword))
            .ToList();
            return Ok(newsWithAdminNames);
        }



        [HttpPost("add-image/{newsId}")]
        public IActionResult AddProperty(IFormFile file, int newsId)
        {
            var result = _imageService.UploadPhotoNews(file);
            if (result.Error != null)
                return BadRequest(result.Error.Message);
            var news = _dbContext.News.Find(newsId);
            if(news == null)
                return BadRequest("Tin tức không tồn tại");
            news.Url = result.SecureUrl.AbsoluteUri;
            news.PublicId = result.PublicId;
            _dbContext.SaveChanges();
            return Ok(new
            {
                message = "Thêm thành công"
            });

        }

        [HttpDelete("delete-image/{id}/{publicId}")]
        public IActionResult DeleteProperty(int id, string publicId)
        {
            var img = _dbContext.News.Find(id);
            if (img == null)
            {
                return BadRequest("không tồn tại");
            }
            var result = _imageService.DeletePhotoAsync(publicId);
            img.Url = "";
            _dbContext.SaveChanges();
            return Ok(new
            {
                result = result,
                message = "Xóa thành công"
            });
        }


        [HttpGet("get-news/{id}")]
        public IActionResult GetNewsId(int id)
        {
            try
            {
                var newsWithAdminNames = _dbContext.News
                            .Join(
                                _dbContext.Admins,
                                news => news.AdminId,
                                admin => admin.AdminId,
                                (news, admin) => new
                                {
                                    news.NewsId,
                                    news.Title,
                                    news.Content,
                                    news.Url,
                                    news.PublicId,
                                    news.CreateDate,
                                    news.UpdateDate,
                                    FullName = admin.FullName
                                }
                            ).Where(item => item.NewsId == id)
                            .ToList();

                return Ok(newsWithAdminNames);
            }
            catch (Exception ex)
            {
                return BadRequest("not good");
            }
        }

        [HttpGet("get-latest-news")]
        public IActionResult GetLatestNews()
        {
            try
            {
                var latestNews = _dbContext.News
                    .Join(
                        _dbContext.Admins,
                        news => news.AdminId,
                        admin => admin.AdminId,
                        (news, admin) => new
                        {
                            news.NewsId,
                            news.Title,
                            news.Content,
                            news.Url,
                            news.PublicId,
                            news.CreateDate,
                            news.UpdateDate,
                            FullName = admin.FullName
                        }
                    )
                    .OrderByDescending(news => news.CreateDate) // Sắp xếp theo ngày tạo giảm dần
                    .Take(3) // Chỉ lấy 3 bài đầu tiên
                    .ToList();

                return Ok(latestNews);
            }
            catch (Exception ex)
            {
                return BadRequest("not good");
            }
        }

    }
}

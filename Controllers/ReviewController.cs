using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QLSB_APIs.DTO;
using QLSB_APIs.Models.Entities;

namespace QLSB_APIs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly MyDbContext _dbContext;
        private readonly IMapper _mapper;

        public ReviewController(MyDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet("get-reviews")]
        public IActionResult GetReviews()
        {
            var reviews = _dbContext.Reviews
                .Join(
                _dbContext.Bookings,
                reviews => reviews.BookingId,
                booking => booking.BookingId,
                (reviews, booking)=> new
                {
                    reviews.ReviewId,
                    reviews.BookingId,
                    reviews.Comment,
                    reviews.UpdateDate,
                    reviews. CreateDate,
                    booking.UserId,
                    booking.FieldId

                }
                ).Join(
                    _dbContext.Footballfields,
                    booking => booking.FieldId,
                    field => field.FieldId,
                    (booking, field) => new 
                    {
                        booking.ReviewId,
                        booking.BookingId,
                        booking.Comment,
                        booking.UpdateDate,
                        booking.CreateDate,
                        booking.UserId,
                        booking.FieldId,
                        field.FieldName
                    }
                ).Join(
                _dbContext.Users,
                booking => booking.UserId,
                user => user.UserId,
                (booking, user) => new
                {
                    booking.ReviewId,
                    booking.BookingId,
                    booking.Comment,
                    booking.UpdateDate,
                    booking.CreateDate,
                    booking.UserId,
                    booking.FieldId,
                    booking.FieldName,
                    user.FullName,
                    user.Email
                }
                ).Join(
                _dbContext.Invoices,
                booking => booking.BookingId,
                invoice => invoice.BookingId,
                (booking, invoice) => new
                {
                    booking.ReviewId,
                    booking.BookingId,
                    booking.Comment,
                    booking.UpdateDate,
                    booking.CreateDate,
                    booking.UserId,
                    booking.FieldId,
                    booking.FieldName,
                    booking.FullName,
                    booking.Email,
                    invoice.InvoiceId
                }
                )
                .ToList();

            return Ok(reviews);

        }




        [HttpPost("get-reviews-by-date")]
        public IActionResult GetReviewsByDate([FromBody] DateTime date)
        {
            string formattedDate = date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            DateTime today = date.ToLocalTime().Date;
            if (formattedDate == date.ToString())
            {
                today = date.Date;
            }
            var reviews = _dbContext.Reviews
                 .Join(
                 _dbContext.Bookings,
                 reviews => reviews.BookingId,
                 booking => booking.BookingId,
                 (reviews, booking) => new
                 {
                     reviews.ReviewId,
                     reviews.BookingId,
                     reviews.Comment,
                     reviews.UpdateDate,
                     reviews.CreateDate,
                     booking.UserId,
                     booking.FieldId

                 }
                 ).Join(
                     _dbContext.Footballfields,
                     booking => booking.FieldId,
                     field => field.FieldId,
                     (booking, field) => new
                     {
                         booking.ReviewId,
                         booking.BookingId,
                         booking.Comment,
                         booking.UpdateDate,
                         booking.CreateDate,
                         booking.UserId,
                         booking.FieldId,
                         field.FieldName
                     }
                 ).Join(
                 _dbContext.Users,
                 booking => booking.UserId,
                 user => user.UserId,
                 (booking, user) => new
                 {
                     booking.ReviewId,
                     booking.BookingId,
                     booking.Comment,
                     booking.UpdateDate,
                     booking.CreateDate,
                     booking.UserId,
                     booking.FieldId,
                     booking.FieldName,
                     user.FullName,
                     user.Email
                 }
                 ).Join(
                 _dbContext.Invoices,
                 booking => booking.BookingId,
                 invoice => invoice.BookingId,
                 (booking, invoice) => new
                 {
                     booking.ReviewId,
                     booking.BookingId,
                     booking.Comment,
                     booking.UpdateDate,
                     booking.CreateDate,
                     booking.UserId,
                     booking.FieldId,
                     booking.FieldName,
                     booking.FullName,
                     booking.Email,
                     invoice.InvoiceId
                 }
                 ).Where(item => item.CreateDate.Date == today)
                 .ToList();

            return Ok(reviews);
        }
    }
}

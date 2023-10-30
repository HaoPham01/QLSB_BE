using System.Globalization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QLSB_APIs.DTO;
using QLSB_APIs.Models.Entities;

namespace QLSB_APIs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserBookingController : ControllerBase
    {


        private readonly MyDbContext _dbContext;
        private readonly IMapper _mapper;

        public UserBookingController(MyDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet("user-get-booking/{userId}")]
        public IActionResult UserGetBooking(int userId)
        {
            var bookings = _dbContext.Bookings
                .Join(
                    _dbContext.Users,
                    booking => booking.UserId,
                    user => user.UserId,
                    (booking, user) => new
                    {
                        booking.BookingId,
                        booking.UserId,
                        booking.FieldId,
                        booking.PriceBooking,
                        booking.StartTime,
                        booking.EndTime,
                        booking.Status,
                        user.FullName,
                        user.Phone
                    }
                ).Join(
                        _dbContext.Footballfields,
                        booking => booking.FieldId,
                        footballfield => footballfield.FieldId,
                        (booking, footballfield) => new
                        {
                            booking.BookingId,
                            booking.UserId,
                            booking.FieldId,
                            booking.PriceBooking,
                            booking.StartTime,
                            booking.EndTime,
                            booking.Status,
                            booking.FullName,
                            booking.Phone,
                            footballfield.FieldName,
                            footballfield.Type,// Thêm trường tên sân bóng
                                                    // Thêm các trường khác của bảng FootballField nếu cần
                        }
                    ).Join(
                        _dbContext.Invoices,
                        booking => booking.BookingId,
                        invoice => invoice.BookingId,
                        (booking, invoice) => new
                        {
                            booking.BookingId,
                            booking.UserId,
                            booking.FieldId,
                            booking.PriceBooking,
                            booking.StartTime,
                            booking.EndTime,
                            booking.Status,
                            booking.FullName,
                            booking.Phone,
                            booking.FieldName,
                            booking.Type,
                            invoice.PayOnline,
                            statusInvoice = invoice.Status,
                            pricePay = invoice.TotalAmount,
                            idInvoice = invoice.InvoiceId,
                            adminId = invoice.AdminId,
                            createDate = invoice.CreateDate,
                        }
                    )
                // Tiếp tục các phần Join và Select cần thiết
                .Where(item => item.UserId == userId ) // Lọc theo UserId
                .OrderByDescending(item => item.createDate) // Sắp xếp theo StartTime
                .ToList();

            if (bookings == null || !bookings.Any())
            {
                return Ok(new ResultDTO()
                {
                    message = "Không có hóa đơn nào cho người dùng này",
                });
            }

            return Ok(bookings);
        }







        [HttpGet("user-search-booking/{userId}/{keyword}")]
        public IActionResult UserSearchBooking(int userId, string keyword)
        {
            DateTime date = DateTime.ParseExact(keyword, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            //DateTime date = keyword;
            var bookings = _dbContext.Bookings
                .Join(
                    _dbContext.Users,
                    booking => booking.UserId,
                    user => user.UserId,
                    (booking, user) => new
                    {
                        booking.BookingId,
                        booking.UserId,
                        booking.FieldId,
                        booking.PriceBooking,
                        booking.StartTime,
                        booking.EndTime,
                        booking.Status,
                        user.FullName,
                        user.Phone
                    }
                ).Join(
                        _dbContext.Footballfields,
                        booking => booking.FieldId,
                        footballfield => footballfield.FieldId,
                        (booking, footballfield) => new
                        {
                            booking.BookingId,
                            booking.UserId,
                            booking.FieldId,
                            booking.PriceBooking,
                            booking.StartTime,
                            booking.EndTime,
                            booking.Status,
                            booking.FullName,
                            booking.Phone,
                            footballfield.FieldName,
                            footballfield.Type,// Thêm trường tên sân bóng
                                               // Thêm các trường khác của bảng FootballField nếu cần
                        }
                    ).Join(
                        _dbContext.Invoices,
                        booking => booking.BookingId,
                        invoice => invoice.BookingId,
                        (booking, invoice) => new
                        {
                            booking.BookingId,
                            booking.UserId,
                            booking.FieldId,
                            booking.PriceBooking,
                            booking.StartTime,
                            booking.EndTime,
                            booking.Status,
                            booking.FullName,
                            booking.Phone,
                            booking.FieldName,
                            booking.Type,
                            invoice.PayOnline,
                            statusInvoice = invoice.Status,
                            pricePay = invoice.TotalAmount,
                            idInvoice = invoice.InvoiceId,
                            adminId = invoice.AdminId,
                            createDate = invoice.CreateDate

                        }
                    )
                // Tiếp tục các phần Join và Select cần thiết
                .Where(item => item.UserId == userId && item.StartTime.Value.Date == date) // Lọc theo UserId
                .OrderByDescending(item => item.createDate) // Sắp xếp theo StartTime
                .ToList();

            if (bookings == null || !bookings.Any())
            {
                return Ok(new ResultDTO()
                {
                    message = "Không có hóa đơn nào cho người dùng này",
                });
            }

            return Ok(bookings);
        }

        [HttpGet("user-get-isreview/{bookingId}")]
        public IActionResult UserGetIsReview(int bookingId)
        {
            var review = _dbContext.Reviews.FirstOrDefault(r => r.BookingId == bookingId);
            //if (review == null)
            //{
            //    return BadRequest(); // Đã tìm thấy đánh giá với BookingId tương ứng
            //}
            return Ok(review) ;
        }

        [HttpPost("user-post-review")]
        public IActionResult UserPostReview([FromBody] ReviewDTO review)
        {
            Review newReview = new Review
            {
                BookingId = review.BookingId,
                Comment = review.Comment,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };

            _dbContext.Reviews.Add(newReview);
            _dbContext.SaveChanges();
            return Ok(new
            {
                message = "gửi phản hồi thành công"
            });
        }
    }
}
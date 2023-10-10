using System.Data.Entity;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X9;
using QLSB_APIs.DTO;
using QLSB_APIs.Models.Entities;
using QLSB_APIs.Services;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace QLSB_APIs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingController : ControllerBase
    {


        private readonly MyDbContext _dbContext;
        private readonly IMapper _mapper;

        public BookingController( MyDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet("get-booking")]
        public IActionResult GetBooking()
        {
            var firstFieldId = _dbContext.Footballfields
                .Where(field => field.Status == 1) // Chọn chỉ các FieldId có trạng thái bằng 1
                .OrderBy(field => field.FieldId) // Sắp xếp theo FieldId tăng dần
                .Select(field => field.FieldId) // Chọn chỉ mục FieldId
                .FirstOrDefault(); // Lấy FieldId đầu tiên

            var booking = _dbContext.Bookings
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
                        start = booking.StartTime,
                        end = booking.EndTime,
                        booking.Status,
                        booking.CreateDate,
                        booking.UpdateDate,
                        title = user.FullName
                    }
                ).Where(item => item.Status == 1 && item.FieldId == firstFieldId)
                .ToList();
            return Ok(booking);
        }

        [HttpGet("get-booking/{fieldId}")]
        public IActionResult GetBookingFromFieldId(int fieldId)
        {
            var booking = _dbContext.Bookings
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
                        start = booking.StartTime,
                        end =  booking.EndTime,
                        booking.Status,
                        booking.CreateDate,
                        booking.UpdateDate,
                        title = user.FullName
                    }
                ).Where(item => item.Status == 1 && item.FieldId == fieldId)
                .ToList();
                return Ok(booking);
        }

        [HttpGet("get-field-list")]
        public IActionResult Getfieldlist()
        {
           var fieldList = _dbContext.Footballfields
             .Where(item => item.Status == 1)
             .Select(item => new
             {
                 item.FieldId,
                 item.FieldName,
                 item.Type
             })
             .ToList();

           return Ok(fieldList);
        }
        [HttpGet("get-price-from-time/{field1}/{start1}/{end1}")]
        public IActionResult GetGiaTheoKhungGio(int field1, string start1, string end1)
        {
            TimeSpan startTime = TimeSpan.Parse(Uri.UnescapeDataString(start1));
            TimeSpan endTime = TimeSpan.Parse(Uri.UnescapeDataString(end1));

            var gia = _dbContext.Pricebookings.FromSqlRaw("SELECT GetGiaTheoKhungGio({0}, {1}, {2})", field1, startTime, endTime).ToList();
            if (gia == null)
                return BadRequest();
            return Ok(gia);
        }

        //[HttpPost("add-booking2")]
        //public IActionResult AddBooking2([FromBody] BookingDTO bookingDTO)
        //{
        //    if (bookingDTO == null)
        //        return BadRequest();
        //    _dbContext.Database.ExecuteSqlRaw("CALL AddBooking({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})", bookingDTO.BookingId, bookingDTO.UserId, bookingDTO.FieldId, bookingDTO.PriceBooking, bookingDTO.StartTime, bookingDTO.EndTime, bookingDTO.Status, bookingDTO.CreateDate, bookingDTO.UpdateDate);
        //    return Ok();
        //}

        [HttpPost("add-booking")]
        public IActionResult AddBooking([FromBody] BookingDTO bookingDTO)
        {
            if (bookingDTO == null)
                return BadRequest();

            var newBooking = new Booking
            {
                UserId = bookingDTO.UserId,
                FieldId = bookingDTO.FieldId,
                PriceBooking = bookingDTO.PriceBooking,
                StartTime = bookingDTO.StartTime,
                EndTime = bookingDTO.EndTime,
                Status = bookingDTO.Status,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };

            _dbContext.Bookings.Add(newBooking);
            _dbContext.SaveChanges();

            return Ok(newBooking.BookingId);
        }


        [HttpGet("get-booking-by-bookingid/{bookingId}")]
        public IActionResult GetBookingFromBookingId(int bookingId)
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
                            booking.CreateDate,
                            booking.UpdateDate,
                            user.FullName
                        }
                    )
                    .Join(
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
                            booking.CreateDate,
                            booking.UpdateDate,
                            booking.FullName,
                            footballfield.FieldName, // Thêm trường tên sân bóng
                                                         // Thêm các trường khác của bảng FootballField nếu cần
                        }
                    )
                    .Where(item => item.Status == 1 && item.BookingId == bookingId)
                    .ToList();
            return Ok(bookings);
        }

    }

}

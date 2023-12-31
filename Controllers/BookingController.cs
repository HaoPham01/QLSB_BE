﻿using System.Data.Entity;
using System.Globalization;
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
                .Where(field => field.Status == 1)
                .Select(field => field.FieldId)
                .First();

            var bookings = _dbContext.Bookings
                .Join(
                    _dbContext.Users,
                    booking => booking.UserId,
                    user => user.UserId,
                    (booking, user) => new BookingColorDTO
                    {
                        BookingId = booking.BookingId,
                        UserId = booking.UserId,
                        FieldId = booking.FieldId,
                        PriceBooking = booking.PriceBooking,
                        Start = booking.StartTime,
                        End = booking.EndTime,
                        Status = booking.Status,
                        CreateDate = booking.CreateDate,
                        UpdateDate = booking.UpdateDate,
                        Title = user.FullName
                    }
                ).Where(item => item.Status != 0 && item.FieldId == firstFieldId)
                .ToList();

            foreach (var booking in bookings)
            {
                if (booking.Status == 1)
                {
                    booking.Color = "#04BFBF";
                }
                else if (booking.Status == -1)
                {
                    booking.Color = "#F28705";
                }
            }
            return Ok(bookings);
        }

        [HttpGet("get-booking/{fieldId}")]
        public IActionResult GetBookingFromFieldId(int fieldId)
        {
            var bookings = _dbContext.Bookings
                .Join(
                    _dbContext.Users,
                    booking => booking.UserId,
                    user => user.UserId,
                    (booking, user) => new BookingColorDTO
                    {
                        BookingId = booking.BookingId,
                        UserId = booking.UserId,
                        FieldId = booking.FieldId,
                        PriceBooking = booking.PriceBooking,
                        Start = booking.StartTime,
                        End = booking.EndTime,
                        Status = booking.Status,
                        CreateDate = booking.CreateDate,
                        UpdateDate = booking.UpdateDate,
                        Title = user.FullName
                    }
                ).Where(item => item.Status != 0 && item.FieldId == fieldId)
                .ToList();

            foreach (var booking in bookings)
            {
                if (booking.Status == 1)
                {
                    booking.Color = "#04BFBF";
                }
                else if (booking.Status == -1)
                {
                    booking.Color = "#F28705";
                }
            }

            return Ok(bookings);
        }


        [HttpGet("get-field-list")]
        public IActionResult GetFieldList()
        {
           var fieldList = _dbContext.Footballfields
             .Where(item => item.Status == 1)
             .Select(footballfield => new
             {
                 footballfield.FieldId,
                 footballfield.FieldName,
                 footballfield.Type
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
                    .Where(item => item.Status != 0 && item.BookingId == bookingId)
                    .ToList();
            return Ok(bookings);
        }



        [HttpGet("cancel-status-booking/{bookingId}")]
        public IActionResult CancelStatusBooking(int bookingId)
        {
            if (bookingId == null)
                return BadRequest("Id không tồn tại");
            Booking bookings = _dbContext.Bookings
                .Where(item => item.Status == -1 && item.BookingId == bookingId) // Chọn chỉ các FieldId có trạng thái bằng 1
                .First(); // Lấy FieldId đầu tiên
            if (bookings != null) {
                bookings.Status = 0;
                _dbContext.SaveChanges();
                return Ok(new ResultDTO()
                    {
                        message = "Lịch sân đã bị hủy do hết thời gian",

                    });
             }
            return Ok(new ResultDTO()
            {
                message = "Lỗi",

            }); ;
        }

        [HttpPost("search-empty-booking")]
        public IActionResult SearchEmptyBooking(SearchEmptyBookingDTO bookingDTO)
        {
            DateTime selectedDay = DateTime.ParseExact(bookingDTO.Day, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            // Chuyển đổi chuỗi giờ bắt đầu thành số nguyên
            int startHour = bookingDTO.Start;
            // Tính thời gian kết thúc dựa trên giờ bắt đầu và tổng thời gian
            //DateTime startTime = selectedDay.AddHours(startHour);
            DateTime startTime = new DateTime(selectedDay.Year, selectedDay.Month, selectedDay.Day, startHour, bookingDTO.Minutes, 0);
            int endHour = startHour + ((bookingDTO.TotalTime + bookingDTO.Minutes) / 60);
            int endMinute = (bookingDTO.TotalTime + bookingDTO.Minutes) % 60;

            // Tạo đối tượng DateTime cho EndTime
            DateTime endTime = new DateTime(selectedDay.Year, selectedDay.Month, selectedDay.Day, endHour, endMinute, 0);



            List<Footballfield> availableFields = new List<Footballfield>();
            // Lấy danh sách tất cả các sân
            var allFields = _dbContext.Footballfields.Where(item => item.Type == bookingDTO.Type && item.Status == 1).ToList();

            foreach (var field in allFields)
            {
                // Kiểm tra xem có bất kỳ đặt sân nào nằm trong khoảng thời gian này không
                bool isAvailable = !_dbContext.Bookings
                    .Any(booking =>
                        booking.Status != 0 && 
                        booking.FieldId == field.FieldId && (
                        (startTime >= booking.StartTime && startTime < booking.EndTime) ||
                        (endTime > booking.StartTime && endTime <= booking.EndTime)));

                if (isAvailable)
                {

                    availableFields.Add(field);
                }
            }

            return Ok(availableFields);
        }



        [HttpPost("date-conversion")]
        public IActionResult DateConversion(SearchEmptyBookingDTO bookingDTO)
        {
            DateTime selectedDay = DateTime.ParseExact(bookingDTO.Day, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            // Chuyển đổi chuỗi giờ bắt đầu thành số nguyên
            int startHour = bookingDTO.Start;
            // Tính thời gian kết thúc dựa trên giờ bắt đầu và tổng thời gian
            //DateTime startTime = selectedDay.AddHours(startHour);
            DateTime startTime = new DateTime(selectedDay.Year, selectedDay.Month, selectedDay.Day, startHour, bookingDTO.Minutes, 0);
            int endHour = startHour + ((bookingDTO.TotalTime + bookingDTO.Minutes) / 60);
            int endMinute = (bookingDTO.TotalTime + bookingDTO.Minutes) % 60;

            // Tạo đối tượng DateTime cho EndTime
            DateTime endTime = new DateTime(selectedDay.Year, selectedDay.Month, selectedDay.Day, endHour, endMinute, 0);

            return Ok(new
            {
                startTime = startTime,
                endTime = endTime
            });

        }

            //STAFF-BOOKING--------------------------------------------------------------------
            [HttpGet("staff-get-booking")]
        public IActionResult StaffGetBooking()
        {
            DateTime today = DateTime.Today;
            var firstFieldId = _dbContext.Footballfields
                .Where(field => field.Status == 1) // Chọn chỉ các FieldId có trạng thái bằng 1
                .OrderBy(field => field.FieldId) // Sắp xếp theo FieldId tăng dần
                .Select(field => field.FieldId) // Chọn chỉ mục FieldId
                .First(); ; // Lấy FieldId đầu tiên
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
                            user.FullName,
                            user.Phone
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
                            booking.Phone,
                            footballfield.FieldName, // Thêm trường tên sân bóng
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
                            booking.CreateDate,
                            booking.UpdateDate,
                            booking.FullName,
                            booking.Phone,
                            booking.FieldName,
                            invoice.PayOnline,
                            statusInvoice = invoice.Status,
                            totalInvoice = invoice.TotalAmount,
                            idInvoice = invoice.InvoiceId,
                            //adminId = invoice.AdminId,
                        }
                    )
                    .Where(item => item.Status == 1 && item.statusInvoice != 1 && item.FieldId == firstFieldId && item.StartTime.Value.Date == today)
                    .OrderBy(item => item.StartTime) // Sắp xếp theo StartTime
                    .ToList();
            return Ok(bookings);
        }

        [HttpGet("staff-get-booking/{fieldId}")]
        public IActionResult StaffGetBooking(int fieldId)
        {
            DateTime today = DateTime.Today;
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
                            user.FullName,
                            user.Phone
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
                            booking.Phone,
                            footballfield.FieldName, // Thêm trường tên sân bóng
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
                            booking.CreateDate,
                            booking.UpdateDate,
                            booking.FullName,
                            booking.Phone,
                            booking.FieldName,
                            invoice.PayOnline,
                            statusInvoice = invoice.Status,
                            totalInvoice = invoice.TotalAmount,
                            idInvoice = invoice.InvoiceId,
                            //adminId = invoice.AdminId,
                        }
                    )
                    .Where(item => item.Status == 1 && item.statusInvoice != 1 && item.FieldId == fieldId && item.StartTime.Value.Date == today)
                    .OrderBy(item => item.StartTime) // Sắp xếp theo StartTime
                    .ToList();
            return Ok(bookings);
        }

        [HttpGet("staff-search-booking/{keyword}")]
        public IActionResult Search(string keyword)
        {
            DateTime today = DateTime.Today;
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
                user.FullName,
                user.Phone
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
                booking.Phone,
                footballfield.FieldName, // Thêm trường tên sân bóng
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
                booking.CreateDate,
                booking.UpdateDate,
                booking.FullName,
                booking.Phone,
                booking.FieldName,
                statusInvoice = invoice.Status,
                totalInvoice = invoice.TotalAmount,
                idInvoice = invoice.InvoiceId,
                //adminId = invoice.AdminId,
            }
        )
        .Where(item => item.Status == 1 && item.statusInvoice != 1 && item.Phone.Contains(keyword) && item.StartTime.Value.Date == today)
        .OrderBy(item => item.StartTime) // Sắp xếp theo StartTime
        .ToList();
            return Ok(bookings);
        }
    }

}

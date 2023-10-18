using System.Globalization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QLSB_APIs.DTO;
using QLSB_APIs.Models.Entities;

namespace QLSB_APIs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvoiceController : ControllerBase
    {


        private readonly MyDbContext _dbContext;
        private readonly IMapper _mapper;

        public InvoiceController(MyDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet("staff-get-booking/{bookingId}")]
        public IActionResult StaffGetBooking(int bookingId)
        {
            if (bookingId == null)
                return BadRequest();
            DateTime today = DateTime.Today;
            var firstFieldId = _dbContext.Footballfields
                .Where(field => field.Status == 1) // Chọn chỉ các FieldId có trạng thái bằng 1
                .OrderBy(field => field.FieldId) // Sắp xếp theo FieldId tăng dần
                .Select(field => field.FieldId) // Chọn chỉ mục FieldId
                .FirstOrDefault(); // Lấy FieldId đầu tiên
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
                            booking.FullName,
                            booking.Phone,
                            booking.FieldName,
                            invoice.PayOnline,
                            statusInvoice = invoice.Status,
                            pricePay = invoice.TotalAmount,
                            idInvoice = invoice.InvoiceId,
                            //adminId = invoice.AdminId,
                            createDate = invoice.CreateDate
                            
                        }
                    )
                    .Where(item => item.Status == 1 && item.statusInvoice != 1 && item.BookingId == bookingId)
                    .OrderBy(item => item.createDate) // Sắp xếp theo StartTime
                    .ToList();
            return Ok(bookings);
        }


        [HttpGet("get-price-invoice/{bookingId}")]
        public IActionResult GetPriceInvoice(int bookingId)
        {
            var service = _dbContext.Services
                .Select(service => new
                {
                    service.BookingId,
                    service.Quantity,
                    service.Price,
                    totalPrice = service.Price * service.Quantity
                })
                .Where(item => item.BookingId == bookingId)
                .ToList();

            var totalPriceService = service.Sum(item => item.totalPrice);

            var priceBooking = _dbContext.Bookings
                .Where(item => item.BookingId == bookingId)
                .Select(booking => booking.PriceBooking)
                .FirstOrDefault(); // Use FirstOrDefault to get the single value, assuming BookingId is unique.

            var totalPrice = priceBooking + totalPriceService;
            return Ok(new PriceBookDTO()
            {
                Gia = (int)totalPrice
            }); ;
        }

        [HttpPost("confirm-invoice")]
        public IActionResult StaffConfirmInvoice([FromBody] InvoiceDTO invoiceDTO)
        {
           var invoice = _dbContext.Invoices.Find(invoiceDTO.InvoiceId);
           var adminId = _dbContext.Admins
                .Where(item => item.Email == invoiceDTO.adminEmail)
                .Select(admin => admin.AdminId)
                .FirstOrDefault(); 
            if (invoice == null)
           {
             return BadRequest("Hóa đơn không tồn tại");
           }
            invoice.AdminId = adminId;
            invoice.TotalAmount = invoiceDTO.TotalAmount;
            invoice.Status = 1;
            invoice.CreateDate = invoiceDTO.CreateDate;
            _dbContext.SaveChanges();
           return Ok(invoice);
        }

        [HttpPost("confirm-cancel-invoice")]
        public IActionResult StaffConfirmCancelInvoice([FromBody] InvoiceDTO invoiceDTO)
        {
            var invoice = _dbContext.Invoices.Find(invoiceDTO.InvoiceId);
            var adminId = _dbContext.Admins
                 .Where(item => item.Email == invoiceDTO.adminEmail)
                 .Select(admin => admin.AdminId)
                 .FirstOrDefault();
            if (invoice == null)
            {
                return BadRequest("Hóa đơn không tồn tại");
            }
            invoice.AdminId = adminId;
            invoice.Status = 1;
            _dbContext.SaveChanges();
            return Ok(invoice);
        }

        [HttpPost("staff-get-invoice")]
        public IActionResult StaffGetInvoice([FromBody] DateTime date)
        {
            string formattedDate = date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            DateTime today = date.ToLocalTime().Date;
             if (formattedDate == date.ToString())
            {
                today = date.Date;
            }
            //DateTime today = date.Date;
            var firstFieldId = _dbContext.Footballfields
                .Where(field => field.Status == 1) // Chọn chỉ các FieldId có trạng thái bằng 1
                .OrderBy(field => field.FieldId) // Sắp xếp theo FieldId tăng dần
                .Select(field => field.FieldId) // Chọn chỉ mục FieldId
                .FirstOrDefault(); // Lấy FieldId đầu tiên
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
                            booking.FullName,
                            booking.Phone,
                            booking.FieldName,
                            invoice.PayOnline,
                            statusInvoice = invoice.Status,
                            pricePay = invoice.TotalAmount,
                            idInvoice = invoice.InvoiceId,
                            adminId = invoice.AdminId,
                            createDate = invoice.CreateDate

                        }
                    ).Join(
                        _dbContext.Admins,
                        invoice => invoice.adminId,
                        admin => admin.AdminId,
                        (invoice, admin) => new
                        {
                            invoice.BookingId,
                            invoice.UserId,
                            invoice.FieldId,
                            invoice.PriceBooking,
                            invoice.StartTime,
                            invoice.EndTime,
                            invoice.Status,
                            invoice.FullName,
                            invoice.Phone,
                            invoice.FieldName,
                            invoice.PayOnline,
                            invoice.statusInvoice,
                            invoice.pricePay,
                            invoice.idInvoice,
                            invoice.adminId,
                            invoice.createDate,
                            adminName = admin.FullName

                        }
                    )
                    .Where(item => item.statusInvoice == 1 && item.createDate.Date == today)
                    .OrderBy(item => item.createDate) // Sắp xếp theo StartTime
                    .ToList();
            if(bookings == null)
                return Ok(new ResultDTO()
                {
                    message = "Không có hóa đơn trong ngày",

                });
            return Ok(bookings);
        }
    }
}

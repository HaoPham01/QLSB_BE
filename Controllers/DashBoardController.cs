using System.Globalization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QLSB_APIs.DTO;
using QLSB_APIs.Models.Entities;

namespace QLSB_APIs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DashBoardController : ControllerBase
    {


        private readonly MyDbContext _dbContext;
        private readonly IMapper _mapper;

        public DashBoardController(MyDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }


        [HttpGet("get-dashboard")]
        public IActionResult GetDashboard()
        {
            decimal totalInvoice = _dbContext.Invoices
                .Where(i => i.Status != 0)
                .Sum(i => i.TotalAmount);

            decimal totalReceipt = _dbContext.Expensereceipts
                .Where(i => i.Type == "thu")
                .Sum(i => i.TotalAmount);

            decimal totalExpense = _dbContext.Expensereceipts
                .Where(i => i.Type == "chi")
                .Sum(i => i.TotalAmount);
            int countBooking = _dbContext.Bookings.Count(booking => booking.Status != 0);
            int countReviews = _dbContext.Reviews.Count();

            return Ok(new
            {
                TotalRevenue = totalInvoice + totalReceipt,
                TotalProfit = totalInvoice + totalReceipt - totalExpense,
                TotalBooking = countBooking,
                TotalReviews = countReviews
            });
        }


        [HttpGet("dashboard-by-week/{year}/{month}/{day}")]
        public IActionResult GetDashboardByWeek(int year, int month, int day)
        {
            // Xác định tuần mà ngày thuộc về
            DateTime targetDate = new DateTime(year, month, day);
            DateTime startDate = targetDate.AddDays(-(int)targetDate.DayOfWeek+1); // Start of the week (Sunday)
            DateTime endDate = startDate.AddDays(6); // End of the week (Saturday)

            decimal totalInvoice = _dbContext.Invoices
                .Where(i => i.Status != 0 && i.CreateDate >= startDate && i.CreateDate <= endDate)
                .Sum(i => i.TotalAmount);

            decimal totalReceipt = _dbContext.Expensereceipts
                .Where(i => i.Type == "thu" && i.CreateDate >= startDate && i.CreateDate <= endDate)
                .Sum(i => i.TotalAmount);

            decimal totalExpense = _dbContext.Expensereceipts
                .Where(i => i.Type == "chi" && i.CreateDate >= startDate && i.CreateDate <= endDate)
                .Sum(i => i.TotalAmount);

            int countBooking = _dbContext.Bookings
                .Count(booking => booking.Status != 0 && booking.StartTime >= startDate && booking.StartTime <= endDate);

            int countReviews = _dbContext.Reviews
                .Count(review => review.CreateDate >= startDate && review.CreateDate <= endDate);

            return Ok(new
            {
                TotalRevenue = totalInvoice + totalReceipt,
                TotalProfit = totalInvoice + totalReceipt - totalExpense,
                TotalBooking = countBooking,
                TotalReviews = countReviews
            });
        }



        [HttpGet("revenue-by-field-and-week/{year}/{month}/{day}")]
        public IActionResult GetRevenueByFieldAndWeek(int year, int month, int day)
        {
            DateTime targetDate = new DateTime(year, month, day);
            DateTime startDate = targetDate.AddDays(-(int)targetDate.DayOfWeek+1); // Start of the week (Sunday)
            DateTime endDate = startDate.AddDays(6); // End of the week (Saturday)

            var revenueByField = _dbContext.Footballfields
                .Join(
                    _dbContext.Bookings,
                    field => field.FieldId,
                    booking => booking.FieldId,
                    (field, booking) => new { field, booking }
                )
                .Join(
                    _dbContext.Invoices,
                    combined => combined.booking.BookingId,
                    invoice => invoice.BookingId,
                    (combined, invoice) => new
                    {
                        FieldId = combined.field.FieldId,
                        FieldName = combined.field.FieldName,
                        TotalAmount = invoice.TotalAmount,
                        Status = invoice.Status,
                        Date = invoice.CreateDate.Date
                    }
                )
                .Where(item => item.Date >= startDate && item.Date <= endDate && item.Status != 0)
                .GroupBy(item => new { item.FieldId, item.FieldName })
                .Select(group => new
                {
                    FieldId = group.Key.FieldId,
                    FieldName = group.Key.FieldName,
                    TotalRevenue = group.Sum(item => item.TotalAmount)
                })
                .ToList();

            var revenueReceipt = _dbContext.Footballfields
                .Join(
                    _dbContext.Expensereceipts,
                    field => field.FieldId,
                    expense => expense.FieldId,
                    (field, expense) => new { field, expense }
                )
                .Where(combined => combined.expense.CreateDate >= startDate && combined.expense.CreateDate <= endDate)
                .GroupBy(combined => new { combined.field.FieldId, combined.field.FieldName })
                .Select(group => new
                {
                    FieldId = group.Key.FieldId,
                    FieldName = group.Key.FieldName,
                    TotalRevenue = group.Sum(item => item.expense.TotalAmount)
                })
                .ToList();

            var combinedList = revenueByField.Concat(revenueReceipt)
                .GroupBy(item => item.FieldId)
                .Select(group => new
                {
                    FieldId = group.Key,
                    FieldName = group.First().FieldName,
                    TotalRevenue = group.Sum(item => item.TotalRevenue)
                })
                .ToList();

            return Ok(combinedList);
        }


        [HttpGet("revenue-by-week/{year}/{month}/{day}")]
        public IActionResult GetRevenueByWeek(int year, int month, int day)
        {
            DateTime selectedDate = new DateTime(year, month, day);
            DayOfWeek selectedDayOfWeek = selectedDate.DayOfWeek;

            List<decimal> dailyRevenues = new List<decimal>();

            for (int i = 1; i <= 7; i++)
            {
                DateTime currentDate = selectedDate.AddDays(i - (int)selectedDayOfWeek);
                decimal totalInvoice = _dbContext.Invoices
                    .Where(i => i.CreateDate.Date == currentDate.Date && i.Status != 0)
                    .Sum(i => i.TotalAmount);

                decimal totalReceipt = _dbContext.Expensereceipts
                    .Where(i => i.CreateDate.Date == currentDate.Date && i.Type == "thu")
                    .Sum(i => i.TotalAmount);

                decimal totalRevenue = totalInvoice + totalReceipt;
                dailyRevenues.Add(totalRevenue);
            }

            return Ok(dailyRevenues);
        }

        [HttpGet("expense-by-week/{year}/{month}/{day}")]
        public IActionResult GetExpenseByWeek(int year, int month, int day)
        {
            DateTime selectedDate = new DateTime(year, month, day);
            DayOfWeek selectedDayOfWeek = selectedDate.DayOfWeek;

            List<decimal> dailyRevenues = new List<decimal>();

            for (int i = 1; i <= 7; i++)
            {
                DateTime currentDate = selectedDate.AddDays(i - (int)selectedDayOfWeek);
                decimal totalExpense = _dbContext.Expensereceipts
                    .Where(i => i.CreateDate.Date == currentDate.Date && i.Type == "chi")
                    .Sum(i => i.TotalAmount);

                decimal totalRevenue = totalExpense;
                dailyRevenues.Add(totalRevenue);
            }

            return Ok(dailyRevenues);
        }




        [HttpPost("revenue-by-date")]
        public IActionResult GetRevenueByDate([FromBody]DateTime date)
        {
            // Lọc hóa đơn theo ngày
            decimal totalInvoice = _dbContext.Invoices
                .Where(i => i.Status != 0 && i.CreateDate.Date == date.Date)
                .Sum(i => i.TotalAmount);
            decimal totalPayOnline = _dbContext.Invoices
                .Where(i => i.Status != 0 && i.CreateDate.Date == date.Date)
                .Sum(i => i.PayOnline);
            // Lọc phiếu thu theo ngày
            decimal totalReceipt = _dbContext.Expensereceipts
                .Where(i => i.Type == "thu" && i.CreateDate.Date == date.Date)
                .Sum(i => i.TotalAmount);

            // Lọc phiếu chi theo ngày
            decimal totalExpense = _dbContext.Expensereceipts
                .Where(i => i.Type == "chi" && i.CreateDate.Date == date.Date)
                .Sum(i => i.TotalAmount);

            return Ok(new
            {
                TotalRevenue = totalInvoice + totalReceipt,
                TotalExpense = totalExpense,
                TotalPayOnline = totalPayOnline
            });
        }

    }
}

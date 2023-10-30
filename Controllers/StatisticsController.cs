using System.Data.SqlClient;
using System.Globalization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;
using QLSB_APIs.DTO;
using QLSB_APIs.Models.Entities;

namespace QLSB_APIs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatisticsController : ControllerBase
    {


        private readonly MyDbContext _dbContext;
        private readonly IMapper _mapper;

        public StatisticsController(MyDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet("revenue-by-month/{year}")]
        public IActionResult GetRevenueByMonth(int year)
        {
            decimal[] revenueByMonth = new decimal[12];

            for (int month = 1; month <= 12; month++)
            {
                DateTime startDate = new DateTime(year, month, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                decimal totalInvoice = _dbContext.Invoices
                    .Where(i => i.CreateDate >= startDate && i.CreateDate <= endDate && i.Status != 0)
                    .Sum(i => i.TotalAmount);
                decimal totalReceipt = _dbContext.Expensereceipts
                    .Where(i => i.CreateDate >= startDate && i.CreateDate <= endDate && i.Type == "thu")
                    .Sum(i => i.TotalAmount);

                revenueByMonth[month - 1] = totalInvoice + totalReceipt;
            }

            return Ok(revenueByMonth);
        }

        [HttpGet("profit-by-month/{year}")]
        public IActionResult GetProfitByMonth(int year)
        {
            decimal[] revenueByMonth = new decimal[12];

            for (int month = 1; month <= 12; month++)
            {
                DateTime startDate = new DateTime(year, month, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                decimal totalInvoice = _dbContext.Invoices
                    .Where(i => i.CreateDate >= startDate && i.CreateDate <= endDate && i.Status != 0)
                    .Sum(i => i.TotalAmount);
                decimal totalReceipt = _dbContext.Expensereceipts
                    .Where(i => i.CreateDate >= startDate && i.CreateDate <= endDate && i.Type == "thu")
                    .Sum(i => i.TotalAmount);
                decimal totalExpense = _dbContext.Expensereceipts
                    .Where(i => i.CreateDate >= startDate && i.CreateDate <= endDate && i.Type == "chi")
                    .Sum(i => i.TotalAmount);
                revenueByMonth[month - 1] = totalInvoice + totalReceipt - totalExpense;
            }

            return Ok(revenueByMonth);
        }

        [HttpGet("expense-by-month/{year}")]
        public IActionResult GetExpenseByMonth(int year)
        {
            decimal[] revenueByMonth = new decimal[12];

            for (int month = 1; month <= 12; month++)
            {
                DateTime startDate = new DateTime(year, month, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                decimal totalExpense = _dbContext.Expensereceipts
                    .Where(i => i.CreateDate >= startDate && i.CreateDate <= endDate && i.Type == "chi")
                    .Sum(i => i.TotalAmount);

                revenueByMonth[month - 1] = totalExpense;
            }

            return Ok(revenueByMonth);
        }


        [HttpGet("revenue-by-quarter/{year}")]
        public IActionResult GetRevenueByQuarter(int year)
        {
            decimal[] revenueByQuarter = new decimal[4];

            for (int quarter = 1; quarter <= 4; quarter++)
            {
                int startMonth = (quarter - 1) * 3 + 1;
                int endMonth = quarter * 3;
                DateTime startDate = new DateTime(year, startMonth, 1);
                DateTime endDate = new DateTime(year, endMonth, DateTime.DaysInMonth(year, endMonth));

                decimal totalInvoice = _dbContext.Invoices
                    .Where(i => i.CreateDate >= startDate && i.CreateDate <= endDate && i.Status != 0)
                    .Sum(i => i.TotalAmount);
                decimal totalReceipt = _dbContext.Expensereceipts
                    .Where(i => i.CreateDate >= startDate && i.CreateDate <= endDate && i.Type == "thu")
                    .Sum(i => i.TotalAmount);

                revenueByQuarter[quarter - 1] = totalInvoice + totalReceipt;
            }

            return Ok(revenueByQuarter);
        }

        [HttpGet("profit-by-quarter/{year}")]
        public IActionResult GetProfitByQuarter(int year)
        {
            decimal[] profitByQuarter = new decimal[4];

            for (int quarter = 1; quarter <= 4; quarter++)
            {
                int startMonth = (quarter - 1) * 3 + 1;
                int endMonth = quarter * 3;
                DateTime startDate = new DateTime(year, startMonth, 1);
                DateTime endDate = new DateTime(year, endMonth, DateTime.DaysInMonth(year, endMonth));

                decimal totalInvoice = _dbContext.Invoices
                    .Where(i => i.CreateDate >= startDate && i.CreateDate <= endDate && i.Status != 0)
                    .Sum(i => i.TotalAmount);
                decimal totalReceipt = _dbContext.Expensereceipts
                    .Where(i => i.CreateDate >= startDate && i.CreateDate <= endDate && i.Type == "thu")
                    .Sum(i => i.TotalAmount);
                decimal totalExpense = _dbContext.Expensereceipts
                    .Where(i => i.CreateDate >= startDate && i.CreateDate <= endDate && i.Type == "chi")
                    .Sum(i => i.TotalAmount);

                profitByQuarter[quarter - 1] = totalInvoice + totalReceipt - totalExpense;
            }

            return Ok(profitByQuarter);
        }

        [HttpGet("expense-by-quarter/{year}")]
        public IActionResult GetExpenseByQuarter(int year)
        {
            decimal[] expenseByQuarter = new decimal[4];

            for (int quarter = 1; quarter <= 4; quarter++)
            {
                int startMonth = (quarter - 1) * 3 + 1;
                int endMonth = quarter * 3;
                DateTime startDate = new DateTime(year, startMonth, 1);
                DateTime endDate = new DateTime(year, endMonth, DateTime.DaysInMonth(year, endMonth));

                decimal totalExpense = _dbContext.Expensereceipts
                    .Where(i => i.CreateDate >= startDate && i.CreateDate <= endDate && i.Type == "chi")
                    .Sum(i => i.TotalAmount);

                expenseByQuarter[quarter - 1] = totalExpense;
            }

            return Ok(expenseByQuarter);
        }



        [HttpGet("revenue-by-field-and-year/{year}")]
        public IActionResult GetRevenueByFieldAndYear(int year)
        {
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
                        Year = invoice.CreateDate.Year
                    }
                )
                .Where(item => item.Year == year && item.Status != 0)
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
                .Where(combined => combined.expense.CreateDate.Year == year)
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
               fieldId = group.Key,
               fieldName = group.First().FieldName,
               totalRevenue = group.Sum(item => item.TotalRevenue)
           })
           .ToList();

            return Ok(combinedList);
        }



        [HttpGet("revenue-by-field-and-month/{year}/{month}")]
        public IActionResult GetRevenueByFieldAndMonth(int year, int month)
        {
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
                        Month = invoice.CreateDate.Month,
                        Year = invoice.CreateDate.Year
                    }
                )
                .Where(item => item.Year == year && item.Month == month && item.Status != 0)
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
                .Where(combined => combined.expense.CreateDate.Year == year && combined.expense.CreateDate.Month == month)
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


        [HttpGet("booking-time-by-field-and-year/{year}")]
        public IActionResult GetFieldBookingSummaryByYear(int year)
        {
            var bookingTimeByField = _dbContext.Timebookings.FromSqlRaw("CALL GetFieldBookingSummaryByYear({0})", year).ToList();

            return Ok(bookingTimeByField);

        }


        [HttpGet("booking-time-by-field-and-year-month/{year}/{month}")]
        public IActionResult GetFieldBookingSummaryByYearMonth(int year, int month)
        {
            var bookingTimeByField = _dbContext.Timebookings.FromSqlRaw("CALL GetFieldBookingSummaryByMonthAndYear({0}, {1})", month, year).ToList();

            return Ok(bookingTimeByField);

        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QLSB_APIs.DTO;
using QLSB_APIs.Models.Entities;
using QLSB_APIs.Services;

namespace QLSB_APIs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExpenseReceiptController : ControllerBase
    {
        private readonly MyDbContext _dbContext;
        private readonly IMapper _mapper;

        public ExpenseReceiptController(MyDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpPost("get-expense")]
        public IActionResult GetExpense([FromBody] DateTime date)
        {
            string formattedDate = date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            DateTime today = date.ToLocalTime().Date;
            if (formattedDate == date.ToString())
            {
                today = date.Date;
            }
            var expense = _dbContext.Expensereceipts
                .Select(
                    expense => new
                    {
                        expense.ErId,
                        expense.FieldId,
                        expense.AdminId,
                        expense.Type,
                        expense.Content,
                        expense.TotalAmount,
                        expense.CreateDate
                    }
                ).Join(
                        _dbContext.Admins,
                        expense => expense.AdminId,
                        admin => admin.AdminId,
                        (expense, admin) => new
                        {
                            expense.ErId,
                            expense.FieldId,
                            expense.AdminId,
                            expense.Type,
                            expense.Content,
                            expense.TotalAmount,
                            expense.CreateDate,
                            adminName = admin.FullName

                        }
                    )
                .Where(item => item.Type == "chi" && item.CreateDate.Date == today)
                    .OrderBy(item => item.CreateDate) // Sắp xếp theo StartTime
                    .ToList();
            return Ok(expense);
        }

        [HttpPost("add-expense")]
        public IActionResult AddExpense([FromBody] ExpenseReceiptDTO expense)
        {
            if (expense == null)
                return BadRequest();
            if(expense.FieldId == 0)
            {
                expense.FieldId = null;
            }
            var newExpense = new Expensereceipt
            {
                ErId = expense.ErId,
                FieldId = expense.FieldId,
                AdminId = expense.AdminId,
                Type    = "chi",
                Content = expense.Content,
                TotalAmount = expense.TotalAmount,
                CreateDate  = expense.CreateDate
            };

            _dbContext.Expensereceipts.Add(newExpense);
            _dbContext.SaveChanges();
            return Ok(expense);
        }

        [HttpGet("search-expense/{keyword}")]
        public IActionResult SearchExpense(string keyword)
        {
            var keywordInt = int.Parse(keyword);
            var expense = _dbContext.Expensereceipts
                .Select(
                    expense => new
                    {
                        expense.ErId,
                        expense.FieldId,
                        expense.AdminId,
                        expense.Type,
                        expense.Content,
                        expense.TotalAmount,
                        expense.CreateDate
                    }
                ).Join(
                        _dbContext.Admins,
                        expense => expense.AdminId,
                        admin => admin.AdminId,
                        (expense, admin) => new
                        {
                            expense.ErId,
                            expense.FieldId,
                            expense.AdminId,
                            expense.Type,
                            expense.Content,
                            expense.TotalAmount,
                            expense.CreateDate,
                            adminName = admin.FullName

                        }
                    )
                .Where(item => item.Type == "chi" && item.ErId == keywordInt)
                    .OrderBy(item => item.CreateDate) // Sắp xếp theo StartTime
                    .ToList();
            return Ok(expense);
        }


        [HttpPost("get-receipt")]
        public IActionResult GetReceipt([FromBody] DateTime date)
        {
            string formattedDate = date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            DateTime today = date.ToLocalTime().Date;
            if (formattedDate == date.ToString())
            {
                today = date.Date;
            }
            var receipt = _dbContext.Expensereceipts
                .Select(
                    expense => new
                    {
                        expense.ErId,
                        expense.FieldId,
                        expense.AdminId,
                        expense.Type,
                        expense.Content,
                        expense.TotalAmount,
                        expense.CreateDate
                    }
                ).Join(
                        _dbContext.Admins,
                        receipt => receipt.AdminId,
                        admin => admin.AdminId,
                        (receipt, admin) => new
                        {
                            receipt.ErId,
                            receipt.FieldId,
                            receipt.AdminId,
                            receipt.Type,
                            receipt.Content,
                            receipt.TotalAmount,
                            receipt.CreateDate,
                            adminName = admin.FullName

                        }
                ).Where(item => item.Type == "thu" && item.CreateDate.Date == today)
                    .OrderBy(item => item.CreateDate) // Sắp xếp theo StartTime
                    .ToList();
            return Ok(receipt);
        }

        [HttpPost("add-receipt")]
        public IActionResult AddReceipt([FromBody] ExpenseReceiptDTO receipt)
        {
            if (receipt == null)
                return BadRequest();
            if (receipt.FieldId == 0)
            {
                receipt.FieldId = null;
            }
            var newReceipt = new Expensereceipt
            {
                ErId = receipt.ErId,
                FieldId = receipt.FieldId,
                AdminId = receipt.AdminId,
                Type = "thu",
                Content = receipt.Content,
                TotalAmount = receipt.TotalAmount,
                CreateDate = receipt.CreateDate
            };

            _dbContext.Expensereceipts.Add(newReceipt);
            _dbContext.SaveChanges();
            return Ok(receipt);
        }

        [HttpGet("search-receipt/{keyword}")]
        public IActionResult SearchReceipt(string keyword)
        {
            var keywordInt = int.Parse(keyword);
            var expense = _dbContext.Expensereceipts
                .Select(
                    expense => new
                    {
                        expense.ErId,
                        expense.FieldId,
                        expense.AdminId,
                        expense.Type,
                        expense.Content,
                        expense.TotalAmount,
                        expense.CreateDate
                    }
                ).Join(
                        _dbContext.Admins,
                        expense => expense.AdminId,
                        admin => admin.AdminId,
                        (expense, admin) => new
                        {
                            expense.ErId,
                            expense.FieldId,
                            expense.AdminId,
                            expense.Type,
                            expense.Content,
                            expense.TotalAmount,
                            expense.CreateDate,
                            adminName = admin.FullName

                        }
                    )
                .Where(item => item.Type == "thu" && item.ErId == keywordInt)
                    .OrderBy(item => item.CreateDate) // Sắp xếp theo StartTime
                    .ToList();
            return Ok(expense);
        }




    }
}

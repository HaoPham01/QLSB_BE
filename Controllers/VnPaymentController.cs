using System.Linq;
using AutoMapper;
using MailKit.Search;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using QLSB_APIs.DTO;
using QLSB_APIs.Models.Entities;
using QLSB_APIs.Models.Vnpayment;

namespace QLSB_APIs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VnPaymentController : ControllerBase
    {
        private readonly MyDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public VnPaymentController(MyDbContext dbContext, IMapper mapper, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet("get-url/{typePaymentVN}/{orderCode}/{typePrice}")]
        public IActionResult UrlPayment(int typePaymentVN, int orderCode, string typePrice)
        {
            HttpContext httpContext = _httpContextAccessor.HttpContext;
            var urlPayment = "";
            var order = _dbContext.Bookings.FirstOrDefault(x => x.BookingId == orderCode);
            //Get Config Info
            string vnp_Returnurl = _config["appSettings:vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = _config["appSettings:vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = _config["appSettings:vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = _config["appSettings:vnp_HashSecret"]; //Secret Key

            VnPayLibrary vnpay = new VnPayLibrary();
            var price = (long)order.PriceBooking * 100;
            if(typePrice == "1") {
                price = price * 30 /100;
            }
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", price.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách th

            if (typePaymentVN == 1)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");

            }
            else if (typePaymentVN == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (typePaymentVN == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }

            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(httpContext));
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan lich san:" + order.BookingId);
            vnpay.AddRequestData("vnp_OrderType", "other");

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.BookingId.ToString());

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return Ok(new VnPayDTO()
            {
                Url = urlPayment

            });
        }


       

        [HttpPost]
        [Route("result")]
        public IActionResult VnPayResult([FromBody] VnPayResultDTO payload)
        {
            VnPayLibrary vnpay = new VnPayLibrary();
            string vnp_HashSecret = _config["appSettings:vnp_HashSecret"]; //Secret Key
            //bool checkSignature = vnpay.ValidateSignature(payload.vnp_SecureHash, vnp_HashSecret);
            //bool checkSignature = vnpay.ValidateSignature(payload.vnp_SecureHash, vnp_HashSecret);
            //if (checkSignature)
                //{
            var order = _dbContext.Bookings.FirstOrDefault(x => x.BookingId == payload.vnp_TxnRef);
                    if (payload.vnp_ResponseCode == "00" && payload.vnp_TransactionStatus == "00")
                    {
                        order.Status = 1;
                        _dbContext.SaveChanges();
                        return Ok(new ResultDTO()
                        {
                            message = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ",

                        });
                    }
                    else
                    {
                        order.Status = 0;
                        _dbContext.SaveChanges();
                        return BadRequest(new ResultDTO() { message = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: "});
                     }
                //}
                //else
                //{

                    //return BadRequest("Có lỗi xảy ra trong quá trình xử lý" );
                //}
        }

    }
}

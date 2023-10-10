using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
        [HttpGet("get-url/{typePaymentVN}/{orderCode}")]
        public String UrlPayment(int typePaymentVN, int orderCode)
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
            return urlPayment;
        }
        //public IActionResult VnpayReturn()
        //{
        //    if (Request.QueryString.Count > 0)
        //    {
        //        string vnp_HashSecret = _config["appSettings:vnp_HashSecret"]; //Secret Key
        //        var vnpayData = Request.QueryString;
        //        VnPayLibrary vnpay = new VnPayLibrary();

        //        foreach (string s in vnpayData)
        //        {
        //            //get all querystring data
        //            if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
        //            {
        //                vnpay.AddResponseData(s, vnpayData[s]);
        //            }
        //        }
        //        //vnp_TxnRef: Ma don hang merchant gui VNPAY tai command=pay    
        //        //vnp_TransactionNo: Ma GD tai he thong VNPAY
        //        //vnp_ResponseCode:Response code from VNPAY: 00: Thanh cong, Khac 00: Xem tai lieu
        //        //vnp_SecureHash: HmacSHA512 cua du lieu tra ve

        //        long orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
        //        long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
        //        string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
        //        string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
        //        String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
        //        String TerminalID = Request.QueryString["vnp_TmnCode"];
        //        long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
        //        String bankCode = Request.QueryString["vnp_BankCode"];

        //        bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
        //        if (checkSignature)
        //        {
        //            if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
        //            {
        //                //Thanh toan thanh cong
        //                //displayMsg.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
        //                log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId, vnpayTranId);
        //            }
        //            else
        //            {
        //                //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
        //                displayMsg.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
        //                log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
        //            }
        //            //displayTmnCode.InnerText = "Mã Website (Terminal ID):" + TerminalID;
        //            //displayTxnRef.InnerText = "Mã giao dịch thanh toán:" + orderId.ToString();
        //            //displayVnpayTranNo.InnerText = "Mã giao dịch tại VNPAY:" + vnpayTranId.ToString();
        //            //displayAmount.InnerText = "Số tiền thanh toán (VND):" + vnp_Amount.ToString();
        //            //displayBankCode.InnerText = "Ngân hàng thanh toán:" + bankCode;
        //        }
        //        else
        //        {
        //            //log.InfoFormat("Invalid signature, InputData={0}", Request.RawUrl);
        //            displayMsg.InnerText = "Có lỗi xảy ra trong quá trình xử lý";
        //        }
        //    }
        //    return Ok();
        //}

    }
}

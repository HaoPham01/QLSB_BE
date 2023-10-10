namespace QLSB_APIs.Helpers
{
    public static class EmailBody
    {
        public static string EmailStringBody(string email, string emailToken)
        {
            return $@"<html>
<head>
</head>
<body style=""margin:0;padding:0;front-family: Arial, Helvetica, sans-serif;"">
<div style="" height: auto; background: linear-gradient(to top, #A0E3F2 50%, #82C0D9  90%) no-repeat; width:400px;padding:30px"">
<div>
    <div>
        <h1>Thay đổi mật khẩu của bạn</h1>
        <hr>
        <p>Đây là Email đổi mật khẩu vui lòng không chia sẻ với bất cứ ai. Click vào nút bên dưới để tiến hành đổi mật khẩu của bạn</p>
        <a href=""http://localhost:4200/reset?email={email}&code={emailToken}""target=""_black""style=""background:#0d6efd;padding:10px;border:none;
        color:white;border-radius:4px;display:block;margin:0 auto;width:50%;text-align:center;text-decoration:none"">Reset Password</a><br>
        <p>MH SPORT,<br><br>
        Admin</p>
        </div>
      </div>
    </div>
    </body>
</html>";
        }
    }
}

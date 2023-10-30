using System.Data;
using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using QLSB_APIs.DTO;
using QLSB_APIs.Helpers;
using QLSB_APIs.Models;
using QLSB_APIs.Models.Entities;
using QLSB_APIs.Services;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;

namespace QLSB_APIs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly MyDbContext _authContext;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        public UserController(MyDbContext authContext, IConfiguration config, IEmailService emailService, IMapper mapper)
        {
            
            _authContext = authContext;
            _config = config;
            _emailService = emailService;
            _mapper = mapper;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] User userObj)
        {
            if (userObj == null)
            {
                return BadRequest();
            }
            var user = _authContext.Users.FirstOrDefault(x => x.Email == userObj.Email);
            if (user == null)
            {
                return NotFound(new { Message = "Tài khoản không tồn tại!" });
            }
            if (!PasswordHasher.VerifyPassword(userObj.Password, user.Password))
            {
                return BadRequest(new { Message = "Mật khẩu không chính xác" });
            }
            user.Token = CreateJwt(user);
            var newAcessToken = user.Token;
            var newRefreshToken = CreateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(1);
            _authContext.SaveChanges();
            return Ok(new TokenApiDTO()
            {
                AccessToken = newAcessToken,
                RefreshToken = newRefreshToken

            });

        }

        [HttpPost("register")]
        public IActionResult RegisterUser([FromBody] User userObj)
        {
            if (userObj == null)
                return BadRequest();
            //CheckEmail
            if (CheckEmailExist(userObj.Email))
                return BadRequest(new { Message = "Email đã tồn tại!" });
            //CheckPass
            var pass = CheckPasswordStrength(userObj.Password);
            if (!string.IsNullOrEmpty(pass))
                return BadRequest(new { Message = pass.ToString() });
            userObj.Password = PasswordHasher.HashPassword(userObj.Password);
            _authContext.Database.ExecuteSqlRaw("CALL AddUser({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})", userObj.UserId, userObj.Email, userObj.Password, userObj.FullName, userObj.Address, userObj.Phone, userObj.Status, userObj.CreateDate, userObj.UpdateDate);
            return Ok(new
            {
                Message = "Đăng ký tài khoản thành công"

            });
        }
        
            
        [HttpGet("get-user")]
        public IActionResult GetAllUsers()
        {
            var user = _authContext.Users
                            .Select(
                                user => new
                                {
    
                                    user.UserId,
                                    user.Email,
                                    user.FullName,
                                    user.Address,
                                    user.Phone,
                                    user.Status,
                                    user.CreateDate,
                                    user.UpdateDate
                                }
                            ).Where(item => item.Status == 1)
                            .ToList();
            return Ok(user);
        }



        [HttpPut("put-user")]
        public IActionResult Put([FromBody] UserDTO userObj)
        {
            var user = _authContext.Users.Find(userObj.UserId);
            if (user == null)
            {
                return BadRequest("User không tồn tại");
            }
            user.Email = userObj.Email;
            user.FullName = userObj.FullName;
            user.Address = userObj.Address;
            user.Phone = userObj.Phone;
            user.UpdateDate = DateTime.Now;
            _authContext.SaveChanges();

            return Ok(userObj);
        }

        [HttpPost("delete-user")]
        public IActionResult DeleteAdmin([FromBody] UserDTO userObj)
        {
            var user = _authContext.Users.Find(userObj.UserId);
            if (user == null)
            {
                return BadRequest("User không tồn tại");
            }
            user.Status = 0;
            _authContext.SaveChanges();
            return Ok(userObj);

        }

        [HttpGet("search/{keyword}")]
        public IActionResult Search(string keyword)
        {
            var search = new MySqlParameter("@Email", keyword);
            var users = _authContext.Users.FromSqlRaw("CALL SearchUserByName (@Email)", search).ToList();
            var userDTOs = _mapper.Map<List<UserDTO>>(users);
            return Ok(userDTOs);
        }

        [HttpGet("get-id-by-email/{email}")]
        public IActionResult GetIdByEmail(string email)
        {
            var user = _authContext.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return BadRequest();
            var id = user.UserId;
            return Ok(id);
        }


        [HttpGet("get-name-by-email/{email}")]
        public IActionResult GetNameByEmail(string email)
        {
            var user = _authContext.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return BadRequest();
            var name = user.FullName;
            return Ok(name);
        }

        [HttpGet("get-user/{id}")]
        public IActionResult GetAllUsers(int id)
        {
            var user = _authContext.Users
                            .Select(
                                user => new
                                {

                                    user.UserId,
                                    user.Email,
                                    user.FullName,
                                    user.Address,
                                    user.Phone,
                                    user.Status,
                                    user.CreateDate,
                                    user.UpdateDate
                                }
                            ).Where(item => item.Status == 1 && item.UserId == id)
                            .ToList();
            return Ok(user);
        }

        private string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new StringBuilder();
            if (password.Length < 8)
                sb.Append("Độ dài mật khẩu phải từ 8 ký tự. " + Environment.NewLine);
            //if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]"))) ;
            //sb.Append("Mật khẩu phải bao gồm chữ và số. " + Environment.NewLine);
            //if (!Regex.IsMatch(password, "[<,>,@,!,#,$,%,&,*,(,),_,+,\\[,\\],{,},?,:,;,|,',\\,.,/,~,`,,-,=]"))
            //    sb.Append("Mật khẩu phái có ít nhất 1 ký tự đặc biệt. " + Environment.NewLine);
            return sb.ToString();
        }
        private bool CheckEmailExist(string email)
        => _authContext.Users.Any(x => x.Email == email);
        private string CreateJwt(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryverysceret....");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email)

            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var tokenDesciptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddMinutes(30),
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDesciptor);
            return jwtTokenHandler.WriteToken(token);
        }
        private string CreateRefreshToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(tokenBytes);

            var tokenInUser = _authContext.Users.Any(a => a.RefreshToken == refreshToken);
            if (tokenInUser)
            {
                return CreateRefreshToken();
            }
            return refreshToken;
        }

        private ClaimsPrincipal GetPrincipleFromExpiredToken(string token)
        {
            var key = Encoding.ASCII.GetBytes("veryverysceret....");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("this is Invalid Token");
            return principal;
        }
        [HttpPost("refresh")]
        public IActionResult Refresh(TokenApiDTO tokenApiDTO)
        {
            if (tokenApiDTO is null)
                return BadRequest("Invalid Client Request");
            string accessToken = tokenApiDTO.AccessToken;
            string refreshToken = tokenApiDTO.RefreshToken;
            var principal = GetPrincipleFromExpiredToken(accessToken);
            var _email = principal.Identity.Name;
            var email = _authContext.Users.FirstOrDefault(u => u.Email == _email);
            if (email is null || email.RefreshToken != refreshToken || email.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid Request");
            var newAccessToken = CreateJwt(email);
            var newRefreshToken = CreateRefreshToken();
            email.RefreshToken = newRefreshToken;
            _authContext.SaveChanges();
            return Ok(new TokenApiDTO()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            });
        }
        [HttpPost("send-reset-email/{email}")]
        public IActionResult SendEmail(string email)
        {
            var user =  _authContext.Users.FirstOrDefault(u => u.Email == email);
            if(user is null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Email không tồn tại"
                });
            }
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var emailToken = Convert.ToBase64String(tokenBytes);
            user.ResetPasswordToken = emailToken;
            user.ResetPasswordExpiry = DateTime.Now.AddMinutes(15);
            string from = _config["EmailSettings:From"];
            var emailModel = new EmailModel(email, "Reset Password!!", EmailBody.EmailStringBody(email, emailToken));
            _emailService.SendEmail(emailModel);
            _authContext.Entry(user).State = EntityState.Modified;
             _authContext.SaveChanges();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Thông tin đổi mật khẩu đã được gửi tới địa chỉ Email của bạn!"
            });
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var newToken = resetPasswordDTO.EmailToken.Replace(" ", "+");
            var query = Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AsNoTracking(_authContext.Users);
            var user = query.FirstOrDefault(a => a.Email == resetPasswordDTO.Email);
            if (user is null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Người dùng không tồn tại"
                });
            }
            var tokenCode = user.ResetPasswordToken;
            DateTime emailTokenExpiry = user.ResetPasswordExpiry;
            if (tokenCode != resetPasswordDTO.EmailToken || emailTokenExpiry < DateTime.Now)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Link đã hết hạn"
                });
            }
            user.Password = PasswordHasher.HashPassword(resetPasswordDTO.NewPassword);
            _authContext.Entry(user).State = EntityState.Modified;
             _authContext.SaveChanges();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Mật khẩu đã được thay đổi"
            });
        }

        [HttpPost("change-password")]
        public IActionResult ChangePassword(ChangePasswordDTO change)
        {
   
            var user = _authContext.Users.FirstOrDefault(a => a.Email == change.Email);
            if (user == null)
            {
                return NotFound(new { Message = "Tài khoản không tồn tại!" });
            }
            if (!PasswordHasher.VerifyPassword(change.OldPassword, user.Password))
            {
                return BadRequest(new { Message = "Mật khẩu cũ không chính xác" });
            }
            if (change.NewPassword != change.ConfirmPassword)
            {
                return BadRequest(new { Message = "Mật khẩu mới không khớp" });
            }
            user.Password = PasswordHasher.HashPassword(change.NewPassword);
            _authContext.SaveChanges();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Mật khẩu đã được thay đổi"
            });
        }

    }
}
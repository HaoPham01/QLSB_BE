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
using QLSB_APIs.Models.Entities;
using QLSB_APIs.Services;

namespace QLSB_APIs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly MyDbContext _authContext;
        
        private readonly IAdminService _adminService;
        private readonly IMapper _mapper;
        public AdminController(IAdminService adminService, MyDbContext authContext, IMapper mapper)
        {
            _adminService = adminService;
            _authContext = authContext;
            _mapper = mapper;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] Admin adminObj)
        {
            if(adminObj == null)
            {
                return BadRequest();
            }
            var admin =  _authContext.Admins.FirstOrDefault(x => x.Email == adminObj.Email);
            if(admin == null)
            {
                return NotFound(new { Message = "Admin Not Found!" });
            }
            if(!PasswordHasher.VerifyPassword(adminObj.Password, admin.Password))
            {
                return BadRequest(new { Message = "Password is Incorrect" });
            }
            admin.Token = CreateJwt(admin);
            var newAcessToken = admin.Token;
            var newRefreshToken = CreateRefreshToken();
            admin.RefreshToken = newRefreshToken;
            admin.RefreshTokenExpiryTime = DateTime.Now.AddDays(1);
            _authContext.SaveChanges();
            return Ok(new TokenApiDTO()
            {
                AccessToken = newAcessToken,
                RefreshToken = newRefreshToken

            });


        }
  
        private bool CheckEmailExist(string email)
        => _authContext.Admins.Any(x => x.Email == email);
        private string CreateJwt(Admin admin) 
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryverysceret....");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, admin.Role),
                new Claim(ClaimTypes.Email, admin.Email)

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

            var tokenInAdmin = _authContext.Admins.Any(a => a.RefreshToken == refreshToken);
            if (tokenInAdmin)
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
            var email = _authContext.Admins.FirstOrDefault(u => u.Email == _email);
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


        [Authorize]
        [HttpGet("get-admin")]
        public IActionResult GetAdmins()
        {
            try
            {
                var admins = _adminService.GetAdminList();
                return Ok(admins);
            }
            catch (Exception ex)
            {
                return BadRequest("not good");
            }

        }

        [HttpPost("add-admin")]
        public  IActionResult AddAdmin([FromBody] Admin admin)
        {
           if(CheckEmailExist(admin.Email))
            return BadRequest(new {Message = "Email already exist!"});
            //var pass = CheckPasswordStrength(admin.Password);
            //if (!string.IsNullOrEmpty(pass))
            //    return BadRequest(new { Message = pass.ToString() });

            _adminService.AddAdmin(admin);
           return Ok(admin);
        }

        [HttpPut("put-admin")]
        public IActionResult Put([FromBody]AdminDTO adminObj)
        {
            var admin = _authContext.Admins.Find(adminObj.AdminId);
            if (admin == null)
            {
                return BadRequest("Admin không tồn tại");
            }
 
            admin.FullName = adminObj.FullName;
            admin.Role = adminObj.Role;
            admin.Email = adminObj.Email;
            admin.UpdateDate = DateTime.Now;
            _authContext.SaveChanges();

            return Ok(adminObj);
        }

        [HttpPost("delete-admin")]
        public IActionResult DeleteAdmin([FromBody] AdminDTO adminDTO)
        {
            try
            {
                _adminService.DeleteAdmin(adminDTO);
                return Ok(adminDTO);
            }
            catch (Exception ex)
            {
                return BadRequest("Xóa thất bại");
            }

        }

        [HttpGet("search/{keyword}")]
        public IActionResult Search(string keyword)
        {
            var search = new MySqlParameter("@Email", keyword);
            var admins = _authContext.Admins.FromSqlRaw<Admin>("CALL SearchAdminByName (@Email)", search).ToList();
            var adminDTOs = _mapper.Map<List<AdminDTO>>(admins);
            return Ok(adminDTOs);
        }
        [HttpGet("get-email-by-id/{id}")]
        public IActionResult GetEmailById(int id)
        {
            var admin = _authContext.Admins.FirstOrDefault(u => u.AdminId == id);
            var email = admin.Email;
            return Ok(email);
        }

        [HttpGet("get-id-by-email/{email}")]
        public IActionResult GetIdlByEmail(string email)
        {
            var admin = _authContext.Admins.FirstOrDefault(u => u.Email == email);
            var id = admin.AdminId;
            return Ok(id);
        }
        [HttpGet("get-name-by-email/{email}")]
        public IActionResult GetNameByEmail(string email)
        {
            var admin = _authContext.Admins.FirstOrDefault(u => u.Email == email);
            var name = admin.FullName;
            return Ok(new ResultDTO()
            {
                message = name

            });
        }


        [HttpGet("get-admin/{id}")]
        public IActionResult GetAdmin(int id)
        {
            var admin = _authContext.Admins
                            .Select(
                                user => new
                                {
                                    user.AdminId,
                                    user.Email,
                                    user.FullName,
                                    user.Role,
                                    user.Status,
                                    user.CreateDate,
                                    user.UpdateDate
                                }
                            ).Where(item => item.Status == 1 && item.AdminId == id)
                            .ToList();
            return Ok(admin);
        }

        [HttpPost("change-password-admin")]
        public IActionResult ChangePasswordAdmin(ChangePasswordDTO change)
        {

            var admin = _authContext.Admins.FirstOrDefault(a => a.Email == change.Email);
            if (admin == null)
            {
                return NotFound(new { Message = "Tài khoản không tồn tại!" });
            }
            if (!PasswordHasher.VerifyPassword(change.OldPassword, admin.Password))
            {
                return BadRequest(new { Message = "Mật khẩu cũ không chính xác" });
            }
            if (change.NewPassword != change.ConfirmPassword)
            {
                return BadRequest(new { Message = "Mật khẩu mới không khớp" });
            }
            admin.Password = PasswordHasher.HashPassword(change.NewPassword);
            _authContext.SaveChanges();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Mật khẩu đã được thay đổi"
            });
        }
    }
}

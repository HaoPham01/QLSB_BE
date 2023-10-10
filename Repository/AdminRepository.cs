using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using QLSB_APIs.DTO;
using QLSB_APIs.Helpers;
using QLSB_APIs.Models.Entities;

namespace QLSB_APIs.Repository
{
    public interface IAdminRepository
    {
        List<Admin> GetAdminList();

        List<Admin> SearchAdmin(AdminDTO adminDTO);
        void AddAdmin(Admin admin);
        void DeleteAdmin(AdminDTO adminDTO);
    
    }

    public class AdminRepository : IAdminRepository
    {
        private readonly MyDbContext _dbContext;

        public AdminRepository(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Admin> GetAdminList()
        {
            var admins = _dbContext.Admins.FromSqlRaw<Admin>("CALL GetAdminList()").ToList();
            return admins;
        }
        public void AddAdmin(Admin admin)
        {
            var hashpass = PasswordHasher.HashPassword(admin.Password);
            var email = new MySqlParameter("@Email", admin.Email);
            var password = new MySqlParameter("@Password",hashpass);
            var fullname = new MySqlParameter("@FullName", admin.FullName);
            var role = new MySqlParameter("@Role", admin.Role);
            var createdate = new MySqlParameter("@CreateDate", admin.CreateDate);
            var updatedate = new MySqlParameter("@UpdateDate", admin.UpdateDate);
            _dbContext.Database.ExecuteSqlRaw("CALL AddAdmin (@Email, @Password, @FullName, @Role, @CreateDate, @UpdateDate)", email, password, fullname, role, createdate, updatedate);      
        }
        public void DeleteAdmin(AdminDTO adminDTO)
        {
            var adminid = new MySqlParameter("@AdminId", adminDTO.AdminId);
            _dbContext.Database.ExecuteSqlRaw("CALL DeleteAdmin (@AdminId)",adminid);

        }

        public List<Admin> SearchAdmin(AdminDTO adminDTO)
        {
            var search = new MySqlParameter("@Email", adminDTO.Email);
            var admins = _dbContext.Admins.FromSqlRaw<Admin>("CALL SearchAdminByName (@Email)", search).ToList();
            return admins;
        }
    }
}

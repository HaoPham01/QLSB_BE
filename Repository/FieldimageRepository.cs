using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using QLSB_APIs.DTO;
using QLSB_APIs.Models.Entities;

namespace QLSB_APIs.Repository
{
    public interface IFieldimageRepository
    {
        List<Fieldimage> GetFieldImageList();
        
        void AddFieldImageUrl(FieldimageDTO footballfieldDTO);
    }

    public class FieldimageRepository : IFieldimageRepository
    {
        private readonly MyDbContext _dbContext;

        public FieldimageRepository(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Fieldimage> GetFieldImageList()
        {
            var fieldimages = _dbContext.Fieldimages.FromSqlRaw<Fieldimage>("CALL GetFieldImageList()").ToList();
            return fieldimages;
        }

        public void AddFieldImageUrl(FieldimageDTO footballfieldDTO)
        {
            var Id = new MySqlParameter("@Id", footballfieldDTO.Id);
            var FieldId = new MySqlParameter("@FieldId", footballfieldDTO.FieldId);
            var ImageUrl = new MySqlParameter("@ImageUrl", footballfieldDTO.ImageUrl);
            _dbContext.Database.ExecuteSqlRaw("CALL AddFieldImageUrl (@Id, @FieldId, @ImageUrl)", Id, FieldId, ImageUrl);
        }

    }

}

using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using QLSB_APIs.DTO;
using QLSB_APIs.Models.Entities;

namespace QLSB_APIs.Repository
{
    public interface IFootballfieldRepository
    {
        List<Footballfield> GetFootballfieldList();
        void AddFootballfield(FootballfieldDTO footballfieldDTO);

        void DeleteFootballfield(FootballfieldDTO footballfieldDTO);
    }

    public class FootballfieldRepository : IFootballfieldRepository
    {
        private readonly MyDbContext _dbContext;

        public FootballfieldRepository(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Footballfield> GetFootballfieldList()
        {
            var footballfields = _dbContext.Footballfields.FromSqlRaw<Footballfield>("CALL GetFootballfieldList()").ToList();
            return footballfields;
        }

        public void AddFootballfield(FootballfieldDTO footballfieldDTO)
        {
            var fieldId = new MySqlParameter("@FieldId", footballfieldDTO.FieldId);
            var adminId = new MySqlParameter("@AdminId", footballfieldDTO.AdminId);
            var fieldName = new MySqlParameter("@FieldName", footballfieldDTO.FieldName);
            var type = new MySqlParameter("@Type", footballfieldDTO.Type);
            var status = new MySqlParameter("@Status", footballfieldDTO.Status);
            var createdate = new MySqlParameter("@CreateDate", footballfieldDTO.CreateDate);
            var updatedate = new MySqlParameter("@UpdateDate", footballfieldDTO.UpdateDate);
            _dbContext.Database.ExecuteSqlRaw("CALL AddFootballfield (@FieldId, @AdminId, @FieldName, @Type, @Status, @CreateDate, @UpdateDate)", fieldId, adminId, fieldName, type, status, createdate, updatedate);
        }

        public void DeleteFootballfield(FootballfieldDTO footballfieldDTO)
        {
            var footballfieldid = new MySqlParameter("@FieldId", footballfieldDTO.FieldId);
            _dbContext.Database.ExecuteSqlRaw("CALL DeleteField (@FieldId)", footballfieldid);

        }
    }

}

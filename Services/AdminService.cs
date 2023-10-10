using AutoMapper;
using QLSB_APIs.DTO;
using QLSB_APIs.Models.Entities;
using QLSB_APIs.Repository;

namespace QLSB_APIs.Services
{
    // IAdminService.cs
    public interface IAdminService
    {
        List<AdminDTO> GetAdminList();

        List<AdminDTO> SearchAdmin(AdminDTO adminDTO);
        void AddAdmin(Admin admin);
        void DeleteAdmin(AdminDTO adminDTO);
    }

    // AdminService.cs
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IMapper _mapper;

        public AdminService(IAdminRepository adminRepository, IMapper mapper)
        {
            _adminRepository = adminRepository;
            _mapper = mapper;
        }

        public List<AdminDTO> GetAdminList()
        {
            var admins = _adminRepository.GetAdminList();
            var adminDTOs = _mapper.Map<List<AdminDTO>>(admins);
            return adminDTOs;
        }
        public void AddAdmin(Admin admin)
        {

            _adminRepository.AddAdmin(admin);


        }
        public void DeleteAdmin(AdminDTO adminDTO)
        {

            _adminRepository.DeleteAdmin(adminDTO);

        }
        public List<AdminDTO> SearchAdmin(AdminDTO adminDTO)
        {
            var admins = _adminRepository.SearchAdmin(adminDTO);
            var adminDTOs = _mapper.Map<List<AdminDTO>>(admins);
            return adminDTOs;
        }

    }

}

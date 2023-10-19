using AutoMapper;
using QLSB_APIs.DTO;
using QLSB_APIs.Models.Entities;
using QLSB_APIs.Repository;

namespace QLSB_APIs.Services
{
    public interface IFieldimageService
    {
        List<FieldimageDTO> GetFieldImageList();

        void AddFieldImageUrl(FieldimageDTO footballfieldDTO);

    }

    public class FieldimageService : IFieldimageService
    {
        private readonly IFieldimageRepository _fieldimageRepository;
        private readonly IMapper _mapper;
        public FieldimageService(IFieldimageRepository fieldimagetRepository, IMapper mapper)
        {
            _fieldimageRepository = fieldimagetRepository;
            _mapper = mapper;
        }

        public List<FieldimageDTO> GetFieldImageList()
        {
            var fieldimages = _fieldimageRepository.GetFieldImageList();
            var fieldimageDTOs = _mapper.Map<List<FieldimageDTO>>(fieldimages);
            return fieldimageDTOs;
        }

        public void AddFieldImageUrl(FieldimageDTO footballfieldDTO)
        {
            _fieldimageRepository.AddFieldImageUrl(footballfieldDTO);
        }
    }

}

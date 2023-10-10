using AutoMapper;
using QLSB_APIs.DTO;
using QLSB_APIs.Models.Entities;
using QLSB_APIs.Repository;

namespace QLSB_APIs.Services
{
    public interface IFootballfieldService
    {
        List<FootballfieldDTO> GetFootballfieldList();
        void AddFootballfield(FootballfieldDTO footballfieldDTO);
        void DeleteFootballfield(FootballfieldDTO footballfieldDTO);
    }

    public class FootballfieldService : IFootballfieldService
    {
        private readonly IFootballfieldRepository _footballfieldRepository;
        private readonly IMapper _mapper;
        public FootballfieldService(IFootballfieldRepository footballfieldtRepository, IMapper mapper)
        {
            _footballfieldRepository = footballfieldtRepository;
            _mapper = mapper;
        }

        public List<FootballfieldDTO> GetFootballfieldList()
        {
            var field = _footballfieldRepository.GetFootballfieldList();
            var fieldDTOs = _mapper.Map<List<FootballfieldDTO>>(field);
            return fieldDTOs;
        }
        public void AddFootballfield(FootballfieldDTO footballfieldDTO)
        {
            _footballfieldRepository.AddFootballfield(footballfieldDTO);
        }

        public void DeleteFootballfield(FootballfieldDTO footballfieldDTO)
        {

            _footballfieldRepository.DeleteFootballfield(footballfieldDTO);

        }
    }

}

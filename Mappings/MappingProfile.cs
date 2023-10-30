using AutoMapper;
using QLSB_APIs.DTO;
using QLSB_APIs.Models.Entities;

namespace QLSB_APIs.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Admin, AdminDTO>()
                .ForMember(dest => dest.AdminId, otp => otp.MapFrom(src => src.AdminId))
                .ForMember(dest => dest.Email, otp => otp.MapFrom(src => src.Email))
                //.ForMember(dest => dest.Password, otp => otp.MapFrom(src => src.Password))
                .ForMember(dest => dest.FullName, otp => otp.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Role, otp => otp.MapFrom(src => src.Role))
                .ForMember(dest => dest.Status, otp => otp.MapFrom(src => src.Status))
                .ForMember(dest => dest.CreateDate, otp => otp.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.UpdateDate, otp => otp.MapFrom(src => src.UpdateDate));

            CreateMap<Footballfield, FootballfieldDTO>()
                .ForMember(dest => dest.FieldId, otp => otp.MapFrom(src => src.FieldId))
                .ForMember(dest => dest.AdminId, otp => otp.MapFrom(src => src.AdminId))
                .ForMember(dest => dest.FieldName, otp => otp.MapFrom(src => src.FieldName))
                .ForMember(dest => dest.Type, otp => otp.MapFrom(src => src.Type))
                //.ForMember(dest => dest.FullName, otp => otp.MapFrom(src => src.Admin.FullName))
                .ForMember(dest => dest.Status, otp => otp.MapFrom(src => src.Status))
                .ForMember(dest => dest.CreateDate, otp => otp.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.UpdateDate, otp => otp.MapFrom(src => src.UpdateDate));
            CreateMap<Booking, EventDTO>()
               .ForMember(dest => dest.title, otp => otp.MapFrom(src => src.UserId))
               .ForMember(dest => dest.start, otp => otp.MapFrom(src => src.StartTime))
               .ForMember(dest => dest.end, otp => otp.MapFrom(src => src.EndTime));
            CreateMap <Price, PriceDTO>()
                .ForMember(dest => dest.PriceId, otp => otp.MapFrom(src => src.PriceId))
               .ForMember(dest => dest.FieldId, otp => otp.MapFrom(src => src.FieldId))
               .ForMember(dest => dest.StartTime, otp => otp.MapFrom(src => src.StartTime))
               .ForMember(dest => dest.EndTime, otp => otp.MapFrom(src => src.EndTime))
               .ForMember(dest => dest.Price1, otp => otp.MapFrom(src => src.Price1))
               .ForMember(dest => dest.CreateDate, otp => otp.MapFrom(src => src.CreateDate))
               .ForMember(dest => dest.UpdateDate, otp => otp.MapFrom(src => src.UpdateDate));
            CreateMap<User, UserDTO>()
               .ForMember(dest => dest.UserId, otp => otp.MapFrom(src => src.UserId))
               .ForMember(dest => dest.Email, otp => otp.MapFrom(src => src.Email))
               //.ForMember(dest => dest.Password, otp => otp.MapFrom(src => src.Password))
               .ForMember(dest => dest.FullName, otp => otp.MapFrom(src => src.FullName))
               .ForMember(dest => dest.Address, otp => otp.MapFrom(src => src.Address))
               .ForMember(dest => dest.Phone, otp => otp.MapFrom(src => src.Phone))
               .ForMember(dest => dest.Status, otp => otp.MapFrom(src => src.Status))
               .ForMember(dest => dest.CreateDate, otp => otp.MapFrom(src => src.CreateDate))
               .ForMember(dest => dest.UpdateDate, otp => otp.MapFrom(src => src.UpdateDate));
            CreateMap<Booking, BookingDTO>()
               .ForMember(dest => dest.BookingId, otp => otp.MapFrom(src => src.BookingId))
               .ForMember(dest => dest.UserId, otp => otp.MapFrom(src => src.UserId))
               //.ForMember(dest => dest.Password, otp => otp.MapFrom(src => src.Password))
               .ForMember(dest => dest.FieldId, otp => otp.MapFrom(src => src.FieldId))
               .ForMember(dest => dest.PriceBooking, otp => otp.MapFrom(src => src.PriceBooking))
               .ForMember(dest => dest.StartTime, otp => otp.MapFrom(src => src.StartTime))
               .ForMember(dest => dest.EndTime, otp => otp.MapFrom(src => src.EndTime))
               .ForMember(dest => dest.Status, otp => otp.MapFrom(src => src.Status))
               .ForMember(dest => dest.CreateDate, otp => otp.MapFrom(src => src.CreateDate))
               .ForMember(dest => dest.UpdateDate, otp => otp.MapFrom(src => src.UpdateDate));
            CreateMap<Fieldimage, ImageDTO>()
               .ForMember(dest => dest.Id, otp => otp.MapFrom(src => src.Id))
               .ForMember(dest => dest.FieldId, otp => otp.MapFrom(src => src.FieldId))
               .ForMember(dest => dest.PublicId, otp => otp.MapFrom(src => src.PublicId))
               .ForMember(dest => dest.ImageUrl, otp => otp.MapFrom(src => src.ImageUrl));
            CreateMap<Review, ReviewDTO>()
               .ForMember(dest => dest.ReviewId, otp => otp.MapFrom(src => src.ReviewId))
               .ForMember(dest => dest.BookingId, otp => otp.MapFrom(src => src.BookingId))
               .ForMember(dest => dest.Comment, otp => otp.MapFrom(src => src.Comment))
               .ForMember(dest => dest.CreateDate, otp => otp.MapFrom(src => src.CreateDate))
               .ForMember(dest => dest.UpdateDate, otp => otp.MapFrom(src => src.UpdateDate));
        }
    }
}

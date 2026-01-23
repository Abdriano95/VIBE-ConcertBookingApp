using AutoMapper;
using DMA_AU24_LAB2_Group4.Data.DTO;
using DMA_AU24_LAB2_Group4.Data.Entity;

namespace DMA_AU24_LAB2_Group4.API.Profiles
{
    public class ConcertProfile : Profile
    {
        public ConcertProfile()
        {
            // Map Entity to DTO
            CreateMap<Concert, ConcertDto>()
                .ForMember(dest => dest.ConcertId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.Performances, opt => opt.MapFrom(src => src.Performances));

            // Map DTO to Entity
            CreateMap<ConcertDto, Concert>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ConcertId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.Performances, opt => opt.MapFrom(src => src.Performances));
        }
    }
}

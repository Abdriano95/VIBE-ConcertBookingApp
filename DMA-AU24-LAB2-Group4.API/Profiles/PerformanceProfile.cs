using AutoMapper;
using DMA_AU24_LAB2_Group4.Data.DTO;
using DMA_AU24_LAB2_Group4.Data.Entity;

namespace DMA_AU24_LAB2_Group4.API.Profiles
{
    public class PerformanceProfile : Profile
    {
        public PerformanceProfile()
        {
            // Map Entity to DTO
            // Note that ConcertTitle does not exists in the Performance entity, it is a derived property from the Concert entity.

            CreateMap<Performance, PerformanceDto>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PerformanceDate, opt => opt.MapFrom(src => src.PerformanceDateAndTime))
                .ForMember(dest => dest.Venue, opt => opt.MapFrom(src => src.Venue))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.ConcertTitle, opt => opt.MapFrom(src => src.Concert != null ? src.Concert.Title : string.Empty));

            // Map DTO to Entity
            // Note that ConcertTitle does not exists in the Performance entity, it is a derived property from the Concert entity.

            CreateMap<PerformanceDto, Performance>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.PerformanceDateAndTime, opt => opt.MapFrom(src => src.PerformanceDate))
                .ForMember(dest => dest.Venue, opt => opt.MapFrom(src => src.Venue))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.Concert, opt => opt.Ignore()); // Concert is a navigation property, set separately
        }
    }
}

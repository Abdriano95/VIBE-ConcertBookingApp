using AutoMapper;
using DMA_AU24_LAB2_Group4.Data.DTO;
using DMA_AU24_LAB2_Group4.MAUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMA_AU24_LAB2_Group4.MAUI.Profiles
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
                .ForPath(dest => dest.ConcertTitle, opt => opt.MapFrom(src => src.Concert != null ? src.Concert.Title : string.Empty));

            // Map DTO to Entity

            CreateMap<PerformanceDto, Performance>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.PerformanceDateAndTime, opt => opt.MapFrom(src => src.PerformanceDate))
                .ForMember(dest => dest.Venue, opt => opt.MapFrom(src => src.Venue))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForPath(dest => dest.Country, opt => opt.MapFrom(src => src.Country));
        }
    }
}

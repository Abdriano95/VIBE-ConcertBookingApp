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
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            // Add the mapping for Booking to BookingDto
            CreateMap<BookingDto, Booking>()
             .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.BookingId))
             .ForMember(dest => dest.CustomerFirstName, opt => opt.MapFrom(src => src.CustomerFirstName))
             .ForMember(dest => dest.CustomerLastName, opt => opt.MapFrom(src => src.CustomerLastName))
             .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.CustomerEmail))
             .ForMember(dest => dest.PerformanceDate, opt => opt.MapFrom(src => src.PerformanceDate))
             .ForMember(dest => dest.Venue, opt => opt.MapFrom(src => src.Venue))
             .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
             .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
             .ForMember(dest => dest.ConcertTitle, opt => opt.MapFrom(src => src.ConcertTitle));

            // Add the mapping for BookingDto to Booking
            CreateMap<Booking, BookingDto>()
                .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.BookingId))
                .ForMember(dest => dest.CustomerFirstName, opt => opt.MapFrom(src => src.CustomerFirstName))
                .ForMember(dest => dest.CustomerLastName, opt => opt.MapFrom(src => src.CustomerLastName))
                .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.CustomerEmail))
                .ForMember(dest => dest.PerformanceDate, opt => opt.MapFrom(src => src.PerformanceDate))
                .ForMember(dest => dest.Venue, opt => opt.MapFrom(src => src.Venue))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.ConcertTitle, opt => opt.MapFrom(src => src.ConcertTitle));


            //    Add the mapping for BookingCreateDto to Booking

            CreateMap<BookingCreateDto, Booking>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.PerformanceId, opt => opt.MapFrom(src => src.PerformanceId));

            // If you need a reverse map from Booking to BookingCreateDto (optional), you can define it here
            CreateMap<Booking, BookingCreateDto>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.PerformanceId, opt => opt.MapFrom(src => src.PerformanceId));
        }
    }
    }

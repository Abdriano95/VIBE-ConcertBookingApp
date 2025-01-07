using AutoMapper;
using DMA_AU24_LAB2_Group4.Data.DTO;
using DMA_AU24_LAB2_Group4.Data.Entity;

namespace DMA_AU24_LAB2_Group4.API.Profiles
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            // Map Entity to DTO
            // Note that CustomerName does not exists in the Booking entity, it is a derived property from the Customer entity. FirstName + LastName = CustomerName.
            // Note that Venue, City, Country, PerformanceDate does not exists in the Booking entity, it is a derived property from the Performance entity.
            // Note that ConcertTitle does not exists in the Booking entity, it is a derived property from the Concert entity.

            CreateMap<Booking, BookingDto>()
                .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CustomerFirstName, opt => opt.MapFrom(src => src.Customer.FirstName))
                .ForMember(dest => dest.CustomerLastName, opt => opt.MapFrom(src => src.Customer.LastName))
                .ForMember(dest => dest.CustomerEmail,opt => opt.MapFrom(src => src.Customer.Email))
                .ForMember(dest => dest.Venue, opt => opt.MapFrom(src => src.Performance.Venue))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Performance.City))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Performance.Country))
                .ForMember(dest => dest.PerformanceDate, opt => opt.MapFrom(src => src.Performance.PerformanceDateAndTime))
                .ForMember(dest => dest.ConcertTitle, opt => opt.MapFrom(src => src.Performance.Concert.Title));

            // Map DTO to Entity
            // Note that CustomerName does not exists in the Booking entity, it is a derived property from the Customer entity. FirstName + LastName = CustomerName.
            // Note that Venue, City, Country, PerformanceDate does not exists in the Booking entity, it is a derived property from the Performance entity.
            // Note that ConcertTitle does not exists in the Booking entity, it is a derived property from the Concert entity.

            CreateMap<BookingDto, Booking>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.BookingId))
            .ForPath(dest => dest.Customer.FirstName, opt => opt.MapFrom(src => src.CustomerFirstName))
            .ForPath(dest => dest.Customer.LastName, opt => opt.MapFrom(src => src.CustomerLastName))
            .ForPath(dest => dest.Customer.Email, opt => opt.MapFrom(src => src.CustomerEmail))
            .ForPath(dest => dest.Performance.Venue, opt => opt.MapFrom(src => src.Venue))
            .ForPath(dest => dest.Performance.City, opt => opt.MapFrom(src => src.City))
            .ForPath(dest => dest.Performance.Country, opt => opt.MapFrom(src => src.Country))
            .ForPath(dest => dest.Performance.PerformanceDateAndTime, opt => opt.MapFrom(src => src.PerformanceDate))
            .ForPath(dest => dest.Performance.Concert.Title, opt => opt.MapFrom(src => src.ConcertTitle));

            // Add the mapping for BookingCreateDto to Booking
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

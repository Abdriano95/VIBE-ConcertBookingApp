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
                .ForMember(dest => dest.CustomerFirstName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.FirstName : string.Empty))
                .ForMember(dest => dest.CustomerLastName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.LastName : string.Empty))
                .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Email : string.Empty))
                .ForMember(dest => dest.Venue, opt => opt.MapFrom(src => src.Performance != null ? src.Performance.Venue : string.Empty))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Performance != null ? src.Performance.City : string.Empty))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Performance != null ? src.Performance.Country : string.Empty))
                .ForMember(dest => dest.PerformanceDate, opt => opt.MapFrom(src => src.Performance != null ? src.Performance.PerformanceDateAndTime : default(DateTime)))
                .ForMember(dest => dest.ConcertTitle, opt => opt.MapFrom(src => src.Performance != null && src.Performance.Concert != null ? src.Performance.Concert.Title : string.Empty));

            // Map DTO to Entity
            // Note that CustomerName does not exists in the Booking entity, it is a derived property from the Customer entity. FirstName + LastName = CustomerName.
            // Note that Venue, City, Country, PerformanceDate does not exists in the Booking entity, it is a derived property from the Performance entity.
            // Note that ConcertTitle does not exists in the Booking entity, it is a derived property from the Concert entity.
            // Navigation properties are set separately, not through mapping

            CreateMap<BookingDto, Booking>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.BookingId))
                .ForMember(dest => dest.Customer, opt => opt.Ignore()) // Navigation property, set separately
                .ForMember(dest => dest.Performance, opt => opt.Ignore()); // Navigation property, set separately

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

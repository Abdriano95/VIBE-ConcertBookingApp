using AutoMapper;
using DMA_AU24_LAB2_Group4.Data.DTO;
using DMA_AU24_LAB2_Group4.Data.Entity;

namespace DMA_AU24_LAB2_Group4.API.Profiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            // Map Entity to DTO
            CreateMap<Customer, CustomerDto>()
                .ForMember(dest => dest.CustomerID, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CustomerFirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.CustomerLastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            // Map DTO to Entity
            CreateMap<CustomerDto, Customer>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CustomerID))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.CustomerFirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.CustomerLastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            // Map RegisterCustomerDto to Customer
            CreateMap<RegisterCustomerDto, Customer>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));

            // Map Customer to RegisterCustomerDto
            CreateMap<Customer, RegisterCustomerDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForSourceMember(src => src.Password, opt => opt.DoNotValidate());
        }
    }
}

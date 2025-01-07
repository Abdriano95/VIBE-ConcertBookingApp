using AutoMapper;
using DMA_AU24_LAB2_Group4.Data.DTO;
using DMA_AU24_LAB2_Group4.MAUI.Models;


namespace DMA_AU24_LAB2_Group4.MAUI.Profiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile() 
        {
            // Map Entity to DTO
            CreateMap<CustomerDto, Customer>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CustomerID))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.CustomerFirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.CustomerLastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
            .ForMember(dest => dest.ConfirmPassword, opt => opt.Ignore());

            // Map DTO to Entity
            CreateMap<Customer, CustomerDto>()
                .ForMember(dest => dest.CustomerID, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CustomerFirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.CustomerLastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));


            //Create Map from Customer model to UpdateCustomerDTO
            CreateMap<Customer, UpdateCustomerDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));

            CreateMap<Customer, UpdateCustomerDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));



            // Map Customer model to CustomerDto
            CreateMap<Customer, RegisterCustomerDto>().ReverseMap();
            CreateMap<Customer, LoginDto>().ReverseMap();
        }
    }
}

using AutoMapper;
using ResteurantApi.Entities;
using ResteurantApi.Models;

namespace ResteurantApi
{
    //Plik ktory towrza powiazania pomiedzy encjami a plikami DTO
    public class ResteurantMappingProfile : Profile
    {
        public ResteurantMappingProfile()
        {
            CreateMap<Resteurant, ResteurantDto>()
                .ForMember(m => m.City, c => c.MapFrom(s => s.Address.City))
                .ForMember(m => m.Street, c => c.MapFrom(s => s.Address.Street))
                .ForMember(m => m.PostalCode, c => c.MapFrom(s => s.Address.PostalCode));

            CreateMap<Dish, DishDto>();
                
            CreateMap<CreateResteurantDto, Resteurant>()
                .ForMember(m => m.Address, c => c.MapFrom(dto => new Address() 
                    { City = dto.City, PostalCode = dto.PostalCode, Street = dto.Street}));

            //nie wpisujemy tutaj tych formember ponieawz entity dish i klasa dishDto wyglada tak samo w resteuracji musilismy okresilic dokladnie adres poniewaz tam brane byla ulica i postalcode
            //z tabeli address a nie z resturant
            CreateMap<CreateDishDto, Dish>();
                
        }
    }
}

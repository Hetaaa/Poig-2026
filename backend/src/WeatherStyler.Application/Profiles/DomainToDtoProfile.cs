using AutoMapper;
using WeatherStyler.Domain.Entities;
using WeatherStyler.Application.Dtos;

namespace WeatherStyler.Application.Profiles;

public class DomainToDtoProfile : Profile
{
    public DomainToDtoProfile()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<ClothingProperty, ClothingPropertyDto>().ReverseMap();
        CreateMap<ClothingSlot, ClothingSlotDto>().ReverseMap();
        CreateMap<Color, ColorDto>().ReverseMap();
        CreateMap<Outfit, OutfitDto>().ReverseMap();
        CreateMap<ProgramVariable, ProgramVariableDto>().ReverseMap();
        CreateMap<Style, StyleDto>().ReverseMap();
        CreateMap<UsageHistory, UsageHistoryDto>().ReverseMap();
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<ClothingItem, OutfitClothingItemDto>().ReverseMap();
        CreateMap<ClothingItem, ClothingItemDto>().ReverseMap();
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<ClothingSlot, ClothingSlotDto>().ReverseMap();
        CreateMap<CreateClothingItemRequest, ClothingItem>()
            .ForMember(dest => dest.PhotoUrl, opt => opt.Ignore());



            
    }
}

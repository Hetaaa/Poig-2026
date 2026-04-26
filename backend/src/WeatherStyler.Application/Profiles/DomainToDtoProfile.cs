using AutoMapper;
using WeatherStyler.Domain.Entities;
using WeatherStyler.Application.Dtos;

namespace WeatherStyler.Application.Profiles;

public class DomainToDtoProfile : Profile
{
    public DomainToDtoProfile()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<ClothingProperty, ClothingPropertyDto>();
        CreateMap<ClothingSlot, ClothingSlotDto>();
        CreateMap<Color, ColorDto>();
        CreateMap<Outfit, OutfitDto>();
        CreateMap<ProgramVariable, ProgramVariableDto>();
        CreateMap<Style, StyleDto>();
        CreateMap<UsageHistory, UsageHistoryDto>();
        CreateMap<User, UserDto>();
        CreateMap<ClothingItem, OutfitClothingItemDto>();
        CreateMap<ClothingItem, ClothingItemDto>();

    }
}

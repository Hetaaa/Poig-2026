using AutoMapper;
using WeatherStyler.Domain.Entities;
using WeatherStyler.Domain.Entities;
using WeatherStyler.Infrastructure.Entities;

namespace WeatherStyler.Infrastructure.Mapping;

public class WardrobeMappingProfile : Profile
{
    public WardrobeMappingProfile()
    {
        CreateMap<ApplicationUser, User>().ReverseMap();
        CreateMap<CategoryEntity, Category>().ReverseMap();
        CreateMap<ClothingSlotEntity, ClothingSlot>().ReverseMap();
        CreateMap<ClothingItemEntity, ClothingItem>().ReverseMap();
        CreateMap<ClothingPropertyEntity, ClothingProperty>().ReverseMap();
        // WarmthRating removed; map WarmthLevel directly on ClothingItem
        CreateMap<StyleEntity, Style>().ReverseMap();
        CreateMap<ColorEntity, Color>().ReverseMap();
        CreateMap<OutfitEntity, Outfit>().ReverseMap();
        CreateMap<UsageHistoryEntity, UsageHistory>().ReverseMap();
    }
}

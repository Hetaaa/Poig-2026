using AutoMapper;
using WeatherStyler.Domain.Wardrobe.Entities;
using WeatherStyler.Infrastructure.Entities;

namespace WeatherStyler.Infrastructure.Mapping;

public class WardrobeMappingProfile : Profile
{
    public WardrobeMappingProfile()
    {
        CreateMap<UserEntity, User>().ReverseMap();
        CreateMap<CategoryEntity, Category>().ReverseMap();
        CreateMap<ClothingItemEntity, ClothingItem>().ReverseMap();
        CreateMap<ClothingPropertyEntity, ClothingProperty>().ReverseMap();
        CreateMap<WarmthRatingEntity, WarmthRating>().ReverseMap();
        CreateMap<StyleEntity, Style>().ReverseMap();
        CreateMap<ColorEntity, Color>().ReverseMap();
        CreateMap<OutfitEntity, Outfit>().ReverseMap();
        CreateMap<UsageHistoryEntity, UsageHistory>().ReverseMap();
    }
}

using AutoMapper;

namespace TheAdventureJunkieWebAPI.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Models.Category, Models.DtoModels.CategoryDto>().ReverseMap();
            CreateMap<Models.Category, Models.DtoModels.CategoryDtoWithoutEvents>().ReverseMap();
        }
    }
}

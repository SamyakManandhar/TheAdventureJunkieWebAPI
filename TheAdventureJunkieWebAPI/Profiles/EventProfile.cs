using AutoMapper;

namespace TheAdventureJunkieWebAPI.Profiles
{
    public class EventProfile:Profile
    {
        public EventProfile()
        {
            CreateMap<Models.Event, Models.DtoModels.EventDto>().ReverseMap();
            CreateMap<Models.DtoModels.EventDtoForUpdates, Models.Event>().ReverseMap();
        }
    }
}

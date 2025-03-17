using Microsoft.AspNetCore.Mvc;
using TheAdventureJunkieWebAPI.Contracts;
using TheAdventureJunkieWebAPI.Models.DtoModels;

namespace TheAdventureJunkieWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;
        public EventController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents()
        {
            var events = await _eventRepository.AllEventsAsync();
            var results = new List<EventDto>();
            foreach (var evt in events)
            {
                results.Add(new EventDto
                {
                    EventId = evt.EventId,
                    Name = evt.Name,
                    ShortDescription = evt.ShortDescription,
                    LongDescription = evt.LongDescription,
                    Price = evt.Price,
                    ImageUrl = evt.ImageUrl,
                    EventLocation = evt.EventLocation,
                    EventDateTime = evt.EventDateTime,
                    CategoryId = evt.CategoryId,
                    Category = evt.Category
                });
            }
            return Ok(results);
        }
    }
}

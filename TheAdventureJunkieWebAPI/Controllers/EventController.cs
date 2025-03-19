using AutoMapper;
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
        private readonly IMapper _mapper;

        public EventController(IEventRepository eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents()
        {
            var events = await _eventRepository.AllEventsAsync();
            return Ok(_mapper.Map<IEnumerable<EventDto>>(events));
        }

        [HttpGet("{eventId}")]
        public async Task<ActionResult<EventDto>> GetEvent(int eventId)
        {
            var evt = await _eventRepository.GetEventByIdAsync(eventId);
            if (evt == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<EventDto>(evt));
        }
    }
}

using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TheAdventureJunkieWebAPI.Contracts;
using TheAdventureJunkieWebAPI.Models;
using TheAdventureJunkieWebAPI.Models.DtoModels;

namespace TheAdventureJunkieWebAPI.Controllers
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion(1)]
    public class EventController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public EventController(IEventRepository eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }

        /// <summary>
        /// Get all events
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents()
        {
            var events = await _eventRepository.AllEventsAsync();
            return Ok(_mapper.Map<IEnumerable<EventDto>>(events));
        }

        /// <summary>
        /// Get a single event
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpGet("{eventId}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<EventDto>> GetEvent(int eventId)
        {
            var evt = await _eventRepository.GetEventByIdAsync(eventId);
            if (evt == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<EventDto>(evt));
        }

        /// <summary>
        /// Create a new event
        /// </summary>
        /// <param name="eventDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<EventDto>> CreateEvent([FromBody] EventDto eventDto)
        {
            var evt = _mapper.Map<Event>(eventDto);
            await _eventRepository.CreateEventAsync(evt);
            return CreatedAtAction(nameof(GetEvent), new { eventId = evt.EventId }, _mapper.Map<EventDto>(evt));
        }

        /// <summary>
        /// Update an event
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="eventUpdateDto"></param>
        /// <returns></returns>
        [HttpPut("{eventId}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateEvent(int eventId, [FromBody] EventDtoForUpdates eventUpdateDto)
        {
            var evt = await _eventRepository.GetEventByIdAsync(eventId);
            if (evt == null)
            {
                return NotFound();
            }
            else
            {
                _mapper.Map(eventUpdateDto, evt);
                await _eventRepository.UpdateEventAsync(evt);
                return Ok();
            }
        }

        /// <summary>
        /// Partially update an event
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="patchDocument"></param>
        /// <returns></returns>
        [HttpPatch("{eventId}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PartiallyUpdateEvent(int eventId, [FromBody] JsonPatchDocument<EventDtoForUpdates> patchDocument)
        {
            var evt = await _eventRepository.GetEventByIdAsync(eventId);
            if (evt == null)
            {
                return NotFound();
            }
            else
            {
                var eventToPatch = _mapper.Map<EventDtoForUpdates>(evt);
                patchDocument.ApplyTo(eventToPatch, ModelState);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (!TryValidateModel(eventToPatch))
                {
                    return ValidationProblem(ModelState);
                }
                _mapper.Map(eventToPatch, evt);
                await _eventRepository.UpdateEventAsync(evt);
                return NoContent();
            }
        }

        /// <summary>
        /// Delete an event
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpDelete("{eventId}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteEvent(int eventId)
        {
            var evt = await _eventRepository.GetEventByIdAsync(eventId);
            if (evt == null)
            {
                return NotFound();
            }
            else
            {
                await _eventRepository.DeleteEventAsync(eventId);
                return NoContent();

            }
        }
    }
}

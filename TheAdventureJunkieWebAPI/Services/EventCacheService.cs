using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using TheAdventureJunkieWebAPI.Contracts;
using TheAdventureJunkieWebAPI.Data;
using TheAdventureJunkieWebAPI.Models;

namespace TheAdventureJunkieWebAPI.Services
{
    public class EventCacheService : IEventCacheService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IDistributedCache _cache;
        public EventCacheService(IEventRepository eventRepository, IDistributedCache cache)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }
        public async Task<IEnumerable<Event>> AllEventsAsync()
        {
            string cacheKey = "AllEvents";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
            {
                var events = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Event>>(cached);
                return events ?? Enumerable.Empty<Event>(); ;
            }
            else
            {
                var events = await _eventRepository.AllEventsAsync();
                var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(60)).SetAbsoluteExpiration(DateTimeOffset.UtcNow.AddHours(6));
                await _cache.SetStringAsync(cacheKey, Newtonsoft.Json.JsonConvert.SerializeObject(events), options);
                return events;
            }
        }

        public async Task CreateEventAsync(Event evt)
        {
            await _eventRepository.CreateEventAsync(evt);
            await ClearCache(null);
        }

        public async Task DeleteEventAsync(int eventId)
        {
            await _eventRepository.DeleteEventAsync(eventId);
            await ClearCache(null);
        }

        public async Task<Event?> GetEventByIdAsync(int eventId)
        {
            string cacheKey = $"Event:{eventId}";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
            {
                var evt = Newtonsoft.Json.JsonConvert.DeserializeObject<Event>(cached);
                return evt;
            }
            else
            {
                var evt = await _eventRepository.GetEventByIdAsync(eventId);
                if (evt != null)
                {
                    var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(60)).SetAbsoluteExpiration(DateTimeOffset.UtcNow.AddHours(6));
                    await _cache.SetStringAsync(cacheKey, Newtonsoft.Json.JsonConvert.SerializeObject(evt), options);
                }
                return evt;
            }
        }

        public async Task UpdateEventAsync(Event evt)
        {
            await _eventRepository.UpdateEventAsync(evt);
            await ClearCache(evt.EventId);
        }

        public async Task ClearCache(int? eventId)
        {
            var Keys = new List<string> { "AllEvents", "AllCategoriesWithEvents", "AllCategoriesWithoutEvents" };
            foreach (var key in Keys)
            {
                await _cache.RemoveAsync(key);
            }
            if (eventId.HasValue)
            {
                string eventCacheKey = $"Event:{eventId.Value}";
                await _cache.RemoveAsync(eventCacheKey);
            }
        }
    }
}

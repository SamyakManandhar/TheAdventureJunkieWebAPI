using Microsoft.EntityFrameworkCore;
using TheAdventureJunkieWebAPI.Contracts;
using TheAdventureJunkieWebAPI.Data;
using TheAdventureJunkieWebAPI.Models;

namespace TheAdventureJunkieWebAPI.Services
{
    public class EventRepository : IEventRepository
    {
        private readonly TheAdventureJunkieDbContext _context;
        public EventRepository(TheAdventureJunkieDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

        }
        public async Task<IEnumerable<Event>> AllEventsAsync()
        { 
            return await _context.Events.OrderBy(e=>e.Name).ToListAsync();
        }

        public async Task CreateEventAsync(Event evt)
        {
            await _context.Events.AddAsync(evt);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEventAsync(Event evt)
        {
            _context.Events.Update(evt);
            await _context.SaveChangesAsync();
        }
        public async Task<Event?> GetEventByIdAsync(int eventId)
        {
            return await _context.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
        }

        public async Task DeleteEventAsync(int eventId)
        {
            var evt = await GetEventByIdAsync(eventId);
            if (evt != null)
            {
                _context.Events.Remove(evt);
                await _context.SaveChangesAsync();
            }
        }
    }
}

using TheAdventureJunkieWebAPI.Models;

namespace TheAdventureJunkieWebAPI.Contracts
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> AllEventsAsync();
        Task<Event?> GetEventByIdAsync(int eventId);
        Task CreateEventAsync(Event evt);
        Task UpdateEventAsync(Event evt);
        Task DeleteEventAsync(int eventId);
    }
}

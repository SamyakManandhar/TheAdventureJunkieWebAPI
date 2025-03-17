using TheAdventureJunkieWebAPI.Models;

namespace TheAdventureJunkieWebAPI.Contracts
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> AllEventsAsync();
        Task <Event?> GetEventByIdAsync(int eventId);
    }
}

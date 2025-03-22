using TheAdventureJunkieWebAPI.Models;

namespace TheAdventureJunkieWebAPI.Contracts
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> AllCategoriesAsync();
        Task<IEnumerable<Category>> AllCategoriesWithEventsAsync();
    }
}

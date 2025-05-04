using TheAdventureJunkieWebAPI.Models;

namespace TheAdventureJunkieWebAPI.Contracts
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> AllCategoriesAsync();
        Task<List<Category>> AllCategoriesWithEventsAsync();
    }
}

using TheAdventureJunkieWebAPI.Models;

namespace TheAdventureJunkieWebAPI.Contracts
{
    public interface ICategoryCacheService
    {
        Task<IEnumerable<Category>> AllCategoriesAsync();
        Task<IEnumerable<Category>> AllCategoriesAsync(string? filterName, string? searchQuery, int pageNumber, int pageSize);
    }
}

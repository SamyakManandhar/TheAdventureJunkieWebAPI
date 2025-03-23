using TheAdventureJunkieWebAPI.Models;

namespace TheAdventureJunkieWebAPI.Contracts
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> AllCategoriesAsync();
        Task<IEnumerable<Category>> AllCategoriesAsync(string? name, string? searchQuery, int pageNumber, int pageSize);
    }
}

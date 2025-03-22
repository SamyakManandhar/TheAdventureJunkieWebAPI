using Microsoft.EntityFrameworkCore;
using TheAdventureJunkieWebAPI.Contracts;
using TheAdventureJunkieWebAPI.Data;
using TheAdventureJunkieWebAPI.Models;

namespace TheAdventureJunkieWebAPI.Services
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly TheAdventureJunkieDbContext _context;
        public CategoryRepository(TheAdventureJunkieDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<IEnumerable<Category>> AllCategoriesAsync()
        {
            return await _context.Categories.OrderBy(c => c.CategoryId).ToListAsync();
        }

        public async Task<IEnumerable<Category>> AllCategoriesWithEventsAsync()
        {
            return await _context.Categories
           .Include(c => c.Events)
           .OrderBy(c => c.CategoryId)
           .ToListAsync();
        }
    }
}
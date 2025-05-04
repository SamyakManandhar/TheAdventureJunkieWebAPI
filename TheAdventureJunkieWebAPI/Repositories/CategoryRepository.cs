using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TheAdventureJunkieWebAPI.Contracts;
using TheAdventureJunkieWebAPI.Data;
using TheAdventureJunkieWebAPI.Models;

namespace TheAdventureJunkieWebAPI.Services
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly TheAdventureJunkieDbContext _context;
        private readonly IDistributedCache _cache;

        public CategoryRepository(TheAdventureJunkieDbContext context, IDistributedCache cache)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<IEnumerable<Category>> AllCategoriesAsync()
        {
            return await _context.Categories.OrderBy(c => c.CategoryId).ToListAsync();
        }

        public async Task<List<Category>> AllCategoriesWithEventsAsync()
        {
            return await _context.Categories.Include(c => c.Events).OrderBy(c => c.CategoryId).ToListAsync();
        }
    }
}
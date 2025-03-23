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

        public async Task<IEnumerable<Category>> AllCategoriesAsync(string? filterName, string? searchQuery, int pageNumber, int pageSize)
        {
            var collection = _context.Categories.Include(c => c.Events) as IQueryable<Category>;
            if (!string.IsNullOrWhiteSpace(filterName))
            {
                filterName = filterName.Trim();
                collection = collection.Where(c =>
                 c.CategoryName.Contains(filterName) ||
                 c.Events.Any(p => p.Name.Contains(filterName))).Select(a => new Category
                 {
                     CategoryId = a.CategoryId,
                     CategoryName = a.CategoryName,
                     Description = a.Description,
                     Events = a.Events.Where(p => p.Name.Contains(filterName)).ToList()
                 });
            }
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(a => a.CategoryName.Contains(searchQuery) ||
                (a.Description != null && a.Description.Contains(searchQuery)) ||
                a.Events.Any(p => p.Name.Contains(searchQuery)) ||
                a.Events.Any(p => p.LongDescription != null && p.LongDescription.Contains(searchQuery)) ||
                a.Events.Any(p => p.ShortDescription != null && p.ShortDescription.Contains(searchQuery)))
                    .Select(a => new Category
                    {
                        CategoryId = a.CategoryId,
                        CategoryName = a.CategoryName,
                        Description = a.Description,
                        Events = a.Events
                    .Where(p => p.Name.Contains(searchQuery) ||
                                (p.LongDescription != null && p.LongDescription.Contains(searchQuery)) ||
                                (p.ShortDescription != null && p.ShortDescription.Contains(searchQuery)))
                    .ToList()
                    });
            }
            //return await collection.OrderBy(c => c.CategoryId).ToListAsync();
            var collectionToReturn = await collection.OrderBy(c => c.CategoryId)
               .Skip(pageSize * (pageNumber - 1))
               .Take(pageSize)
               .ToListAsync();
            return collectionToReturn;
        }
    }
}
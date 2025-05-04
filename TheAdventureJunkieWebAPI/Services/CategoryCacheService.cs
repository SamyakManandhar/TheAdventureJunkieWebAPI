using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using TheAdventureJunkieWebAPI.Contracts;
using TheAdventureJunkieWebAPI.Models;

namespace TheAdventureJunkieWebAPI.Services
{
    public class CategoryCacheService : ICategoryCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryCacheService(IDistributedCache cache, ICategoryRepository categoryRepository)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public async Task<IEnumerable<Category>> AllCategoriesAsync()
        {
            string cacheKey = "AllCategoriesWithoutEvents";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
            {
                var categories = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Category>>(cached);
                return categories;
            }
            else
            {
                var categories = await _categoryRepository.AllCategoriesAsync();
                var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(60)).SetAbsoluteExpiration(DateTimeOffset.UtcNow.AddHours(6));
                await _cache.SetStringAsync(cacheKey, Newtonsoft.Json.JsonConvert.SerializeObject(categories), options);
                return categories;
            }
        }

        public async Task<IEnumerable<Category>> AllCategoriesAsync(string? filterName, string? searchQuery, int pageNumber, int pageSize)
        {
            var categories = new List<Category>();
            string cacheKey = "AllCategoriesWithEvents";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
            {
                categories = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Category>>(cached)?.ToList() ?? new List<Category>();
            }
            else
            {
                categories = await _categoryRepository.AllCategoriesWithEventsAsync();
                var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(60)).SetAbsoluteExpiration(DateTimeOffset.UtcNow.AddHours(6));
                await _cache.SetStringAsync(cacheKey, Newtonsoft.Json.JsonConvert.SerializeObject(categories, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }), options);
            }
            IQueryable<Category> collection = categories.AsQueryable();
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
            var collectionToReturn = collection.Skip(pageSize * (pageNumber - 1))
               .Take(pageSize)
               .ToList();
            return collectionToReturn;
        }
    }
}


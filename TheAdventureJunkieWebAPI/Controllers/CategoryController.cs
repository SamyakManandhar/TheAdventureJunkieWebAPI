using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TheAdventureJunkieWebAPI.Contracts;
using TheAdventureJunkieWebAPI.Models;
using TheAdventureJunkieWebAPI.Models.DtoModels;
using TheAdventureJunkieWebAPI.Services;

namespace TheAdventureJunkieWebAPI.Controllers
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion(1)]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        const int maxPageSize = 20;


        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }
        /// <summary>
        /// Get all categories with corresponding events. Includes Query parameters for Filtering, Searching and Pagination.
        /// </summary>
        /// <param name="includeEvents"></param>
        /// <param name="filterName"></param>
        /// <param name="searchQuery"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryDto>> GetAllCategories([FromQuery] bool includeEvents = true, string? filterName = null, string? searchQuery = null, int pageNumber = 1, int pageSize = 10)
        {
            if (pageSize > maxPageSize)
            {
                pageSize = maxPageSize;
            }
            if (includeEvents)
            {
                var categories = await _categoryRepository.AllCategoriesAsync(filterName, searchQuery, pageNumber, pageSize);
                if (categories.Count() < 1)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<IEnumerable<CategoryDto>>(categories));
            }
            else
            {
                var categories = await _categoryRepository.AllCategoriesAsync();
                return Ok(_mapper.Map<IEnumerable<CategoryDtoWithoutEvents>>(categories));
            }
        }

    }
}

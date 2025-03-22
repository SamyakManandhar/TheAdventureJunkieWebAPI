using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TheAdventureJunkieWebAPI.Contracts;
using TheAdventureJunkieWebAPI.Models;
using TheAdventureJunkieWebAPI.Models.DtoModels;
using TheAdventureJunkieWebAPI.Services;

namespace TheAdventureJunkieWebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCategories(bool includeEvents = false)
        {
            if (includeEvents)
            {
                var categories = await _categoryRepository.AllCategoriesWithEventsAsync();
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

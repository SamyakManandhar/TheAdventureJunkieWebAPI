namespace TheAdventureJunkieWebAPI.Models.DtoModels
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = null!;

        public string? Description { get; set; }

        public ICollection<EventDto> Events { get; set; } = new List<EventDto>();
    }
}

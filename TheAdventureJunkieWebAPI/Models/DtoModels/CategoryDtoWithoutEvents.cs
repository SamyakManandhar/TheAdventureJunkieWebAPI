namespace TheAdventureJunkieWebAPI.Models.DtoModels
{
    public class CategoryDtoWithoutEvents
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = null!;

        public string? Description { get; set; }
    }
}

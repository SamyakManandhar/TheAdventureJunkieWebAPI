using System.ComponentModel.DataAnnotations;

namespace TheAdventureJunkieWebAPI.Models.DtoModels
{
    public class EventDtoForUpdates
    {

        [Required(ErrorMessage = "You should provide a name value.")]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        public string Name { get; set; } = null!;

        [StringLength(200, ErrorMessage = "Short description cannot be longer than 200 characters.")]
        public string? ShortDescription { get; set; }

        [StringLength(1000, ErrorMessage = "Long description cannot be longer than 1000 characters.")]
        public string? LongDescription { get; set; }

        [Range(0, 999.99, ErrorMessage = "Price must be between 0 and 999.99.")]
        public decimal Price { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string? ImageUrl { get; set; }

        [StringLength(100, ErrorMessage = "Location cannot be longer than 100 characters.")]
        public string? EventLocation { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Event Date & Time")]
        public DateTime EventDateTime { get; set; }

        [Required(ErrorMessage = "Category ID is required.")]
        [Range(1, 3, ErrorMessage = "Category ID must be greater than zero.")]
        public int CategoryId { get; set; }
    }
}

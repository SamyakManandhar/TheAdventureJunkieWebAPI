using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheAdventureJunkieWebAPI.Models.DtoModels
{/// <summary>
/// Data Transfer Object for Category without Events.
/// </summary>
    public class CategoryDtoWithoutEvents
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "You should provide a name value.")]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        public string CategoryName { get; set; } = null!;
        [StringLength(200, ErrorMessage = "Description cannot be longer than 200 characters.")]
        public string? Description { get; set; }
    }
}

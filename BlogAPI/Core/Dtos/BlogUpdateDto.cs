using System.ComponentModel.DataAnnotations;

namespace BlogAPI.Core.Dtos
{
    public class BlogUpdateDto
    {
        [Required(ErrorMessage = "The title is required.")]
        [MaxLength(200, ErrorMessage = "The title cannot exceed 200 characters.")]
        public string Title { get; set; }

        [Required]
        [MinLength(10)]
        public string Body { get; set; }
    }
}

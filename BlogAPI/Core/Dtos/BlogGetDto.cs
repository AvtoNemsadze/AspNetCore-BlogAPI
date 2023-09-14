using System.ComponentModel.DataAnnotations;

namespace BlogAPI.Core.Dtos
{
    public class BlogGetDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The title is required.")]
        [MaxLength(200, ErrorMessage = "The title cannot exceed 200 characters.")]
        public string Title { get; set; }

        [Required]
        [MinLength(10)]
        public string Body { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PublishDate { get; set; }
    }
}

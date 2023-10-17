using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SuperMarket.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        [MaxLength(30)]
        [DisplayName("Category Name")]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1, 50, ErrorMessage = "Display Order must be between 1-50")]
        public int DisplayOrder { get; set; }
    }
}

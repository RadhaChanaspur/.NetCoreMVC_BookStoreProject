using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bulky.Models.Models
{
    public class Category
    {
        [Key] // this annotation is added to let Entity Framework know that this is PK
        public int Id { get; set; }
        [Required] //this annotation is added to let EF know that this is non nullable column when it creates a table for this model
        [DisplayName("Category Name")]
        [MaxLength(20)]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(0, 100)]
        public int DisplayOrder { get; set; }
    }
}

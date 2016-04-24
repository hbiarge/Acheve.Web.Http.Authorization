using System.ComponentModel.DataAnnotations;

namespace Sample.Api.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Category { get; set; }

        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Display(Name = "Product type")]
        public ProductType ProductType { get; set; }
    }
}

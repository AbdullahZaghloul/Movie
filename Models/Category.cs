using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Movies.Models
{
    public class Category
    { 
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        [MinLength(3)]
        public string Name { get; set; }
        [ValidateNever]
        public List<Movie> Movies { get; set; }
    }
}

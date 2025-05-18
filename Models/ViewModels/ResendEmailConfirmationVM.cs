using System.ComponentModel.DataAnnotations;

namespace Movies.Models.ViewModels
{
    public class ResendEmailConfirmationVM
    {
        [Required]
        public string UserNameOREmail { get; set; } = null!;
    }
}

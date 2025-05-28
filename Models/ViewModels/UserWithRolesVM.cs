using Microsoft.AspNetCore.Identity;

namespace Movies.Models.ViewModels
{
    public class UserWithRolesVM
    {
        public ApplicationUser ApplicationUser { get; set; }
        public List<IdentityRole>? IdentityRoles { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GeziRotasi.API.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [StringLength(300)]
        public string? AboutMe { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int AvatarIndex { get; set; } = 0;

    }
}

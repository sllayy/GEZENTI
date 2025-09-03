namespace loginpage.Models
{
    public class UpdateProfileDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int AvatarIndex { get; set; }
        public string? AboutMe { get; set; }
    }
}
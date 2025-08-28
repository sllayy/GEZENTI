using GeziRotasi.API.Entities;

namespace GeziRotasi.API.Entities
{
    public class PasswordHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }                 // <--- int
        public string PasswordHash { get; set; } = null!;
        public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

        public AppUser User { get; set; } = null!;
    }
}

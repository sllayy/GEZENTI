using GeziRotasi.API.Entities;
using System;

namespace GeziRotasi.API.Entities
{
    public enum CodePurpose { ConfirmEmail = 0, ResetPassword = 1, UnlockAccount = 2 }

    public class EmailCode
    {
        public int Id { get; set; }
        public int UserId { get; set; }                 // <<< string DEĞİL, int
        public CodePurpose Purpose { get; set; }
        public string Code { get; set; } = null!;
        public DateTimeOffset CreatedAtUtc { get; set; }
        public DateTimeOffset ExpiresAtUtc { get; set; }
        public bool Used { get; set; }

        public AppUser User { get; set; } = null!;
    }
}

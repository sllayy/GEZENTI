using System.ComponentModel.DataAnnotations;

namespace loginpage.Models
{
    public class RegisterDto
    {
        [Required, StringLength(60)] public string FirstName { get; set; } = null!;
        [Required, StringLength(60)] public string LastName { get; set; } = null!;
        [Required, EmailAddress] public string Email { get; set; } = null!;
        [Required, MinLength(6)] public string Password { get; set; } = null!;
    }

    public class ConfirmEmailDto
    {
        [Required, EmailAddress] public string Email { get; set; } = null!;
        [Required, StringLength(6, MinimumLength = 6)]
        public string Code { get; set; } = null!; // 6 hane
    }

    public class LoginDto
    {
        [Required, EmailAddress] public string Email { get; set; } = null!;
        [Required] public string Password { get; set; } = null!;
    }

    public class ForgotPasswordDto
    {
        [Required, EmailAddress] public string Email { get; set; } = null!;
    }

    public class ResetPasswordDto
    {
        [Required, EmailAddress] public string Email { get; set; } = null!;
        [Required, StringLength(6, MinimumLength = 6)] public string Code { get; set; } = null!;
        [Required, MinLength(6)] public string NewPassword { get; set; } = null!;
    }

    public class ChangePasswordDto
    {
        [Required, MinLength(6)] public string OldPassword { get; set; } = null!;
        [Required, MinLength(6)] public string NewPassword { get; set; } = null!;
    }

    public class MeDto
    {
        public string Id { get; set; } = null!;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; } = null!;
    }

    public class TokenResponse
    {
        public string AccessToken { get; set; } = null!;
        public DateTime ExpiresAtUtc { get; set; }
    }
    public class PasswordConfirmDto
    {
        public string Password { get; set; } = null!;
    }

    public class ResendCodeDto
    {
        public string Email { get; set; } = null!;
        // "ConfirmEmail" | "ResetPassword" | "UnlockAccount"
        public string Purpose { get; set; } = null!;
    }
}

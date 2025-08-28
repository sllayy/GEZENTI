using GeziRotasi.API.Data;
using GeziRotasi.API.Entities;
using GeziRotasi.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace GeziRotasi.API.Services
{
    public interface ICodeService
    {
        Task<string> CreateAndSendAsync(AppUser user, CodePurpose purpose, TimeSpan lifetime, Func<string, string> bodyFactory);
        Task<bool> ValidateAsync(int userId, CodePurpose purpose, string code); // <-- int!
    }

    public class CodeService : ICodeService
    {
        private readonly AppDbContext _db;
        private readonly IEmailSender _email;

        public CodeService(AppDbContext db, IEmailSender email)
        {
            _db = db; _email = email;
        }

        // 6 haneli kod
        private static string Generate6Digit() =>
            RandomNumberGenerator.GetInt32(100000, 999999).ToString();

        // SHA-256 hash
        private static string Hash(string code)
        {
            using var sha = SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(code)));
        }

        public async Task<string> CreateAndSendAsync(
            AppUser user,
            CodePurpose purpose,
            TimeSpan lifetime,
            Func<string, string> bodyFactory)
        {
            var now = DateTime.UtcNow;

            // Aynı amaç için süresi dolmamış önceki kodları iptal et
            var olds = await _db.EmailCodes
                .Where(x => x.UserId == user.Id && x.Purpose == purpose && !x.Used && x.ExpiresAtUtc > now)
                .ToListAsync();

            if (olds.Count > 0)
                foreach (var x in olds) x.Used = true;

            var code = Generate6Digit();

            _db.EmailCodes.Add(new EmailCode
            {
                UserId = user.Id,
                Purpose = purpose,
                Code = Hash(code),
                ExpiresAtUtc = now.Add(lifetime),
                Used = false
            });

            await _db.SaveChangesAsync();

            var subject = purpose switch
            {
                CodePurpose.ConfirmEmail => "E-posta Doğrulama Kodu",
                CodePurpose.ResetPassword => "Şifre Sıfırlama Kodu",
                CodePurpose.UnlockAccount => "Hesap Kilidi Açma Kodu",
                _ => "Doğrulama Kodu"
            };

            await _email.SendAsync(user.Email!, subject, bodyFactory(code));
            return code; // istersen loglayabilirsin/testte kullanırsın
        }

        public async Task<bool> ValidateAsync(int userId, CodePurpose purpose, string code)
        {
            var now = DateTime.UtcNow;
            var hash = Hash(code);

            var rec = await _db.EmailCodes
                .Where(x => x.UserId == userId && x.Purpose == purpose && !x.Used && x.ExpiresAtUtc > now)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (rec is null) return false;

            var ok = string.Equals(rec.Code, hash, StringComparison.Ordinal);
            if (ok)
            {
                rec.Used = true;
                await _db.SaveChangesAsync();
            }

            return ok;
        }
    }
}


using FirebaseAdmin.Auth;
using Google.Apis.Auth;
using GeziRotasi.API.Data;
using GeziRotasi.API.Entities;
using GeziRotasi.API.Configurations;
using GeziRotasi.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using GeziRotasi.API.Models;
using GeziRotasi.API.Dtos;
using GeziRotasi.Services;
using loginpage.Models;

namespace GeziRotasi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // api/auth
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _users;
        private readonly SignInManager<AppUser> _signIn;
        private readonly ITokenService _tokens;
        private readonly ICodeService _codes;
        private readonly AppDbContext _db;
        private readonly IEmailSender _email;

        public AuthController(UserManager<AppUser> users, SignInManager<AppUser> signIn, ITokenService tokens, ICodeService codes, AppDbContext db, IEmailSender email)
        {
            _users = users; _signIn = signIn; _tokens = tokens; _codes = codes; _db = db; _email = email;
        }

        // POST api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var exists = await _users.FindByEmailAsync(dto.Email);
            if (exists is not null)
                return BadRequest(new { message = "Bu e-posta zaten kayıtlı." });

            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };

            var res = await _users.CreateAsync(user, dto.Password);
            if (!res.Succeeded) return BadRequest(new { errors = res.Errors });

            // doğrulama kodu oluşturup e-posta gönder
            await _codes.CreateAndSendAsync(user, CodePurpose.ConfirmEmail, TimeSpan.FromMinutes(15),
                code => $"<p>Doğrulama kodunuz: <b>{code}</b></p><p>15 dakika içinde kullanın.</p>");

            return Ok(new { message = "Kayıt oluşturuldu. E-posta doğrulama kodu gönderildi." });
        }

        // POST api/auth/confirm-email
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto dto)
        {
            var user = await _users.FindByEmailAsync(dto.Email);
            if (user == null) return BadRequest(new { message = "Kullanıcı bulunamadı." });
            if (user.EmailConfirmed) return Ok(new { message = "E-posta zaten doğrulanmış." });

            var ok = await _codes.ValidateAsync(user.Id, CodePurpose.ConfirmEmail, dto.Code);
            if (!ok) return BadRequest(new { message = "Kod geçersiz veya süresi dolmuş." });

            user.EmailConfirmed = true;
            var r = await _users.UpdateAsync(user);
            if (!r.Succeeded) return BadRequest(new { errors = r.Errors });

            // email onaylandıktan sonra hoş geldin maili gönder
            try
            {
                var when = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm 'UTC'");
                await _email.SendAsync(user.Email!, "Kaydınız başarıyla oluşturuldu.",
                    $@"<p>Merhaba {user.FirstName},</p>
               <p>E-posta adresiniz <b>başarıyla doğrulandı</b>.</p>
               <p><b>Gezenti ile keşfetmeye başlayın.</b>.</p>
               <p>Tarih: {when}</p>");
            }

            catch {  }

            return Ok(new { message = "E-posta doğrulandı." });
        }

        // POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _users.FindByEmailAsync(dto.Email);
            if (user == null) return Unauthorized(new { message = "Hatalı bilgiler." });
            if (!user.EmailConfirmed) return Unauthorized(new { message = "Lütfen e-postanızı doğrulayın." });

            var check = await _signIn.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);

            if (check.IsLockedOut)
            {
                // kilit çok ileri tarihe ayarlanıyor
                await _users.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));

                try
                {
                    await _codes.CreateAndSendAsync(
                        user,
                        CodePurpose.UnlockAccount,
                        TimeSpan.FromMinutes(15),
                        code => $@"<p>Hesabınız 5 hatalı deneme nedeniyle kilitlendi.</p>
                           <p>Kilidi açma kodunuz: <b>{code}</b> (15 dk)</p>");
                }
                catch (Exception)
                {
                    
                }

                return StatusCode(423, new { message = "Hesabınız kilitlendi. E-postaya gelen kodla açın." });
            }

            if (!check.Succeeded) return Unauthorized(new { message = "Hatalı bilgiler." });

            try
            {
                var (token, exp) = _tokens.Create(user);
                return Ok(new
                {
                    token,
                    expiresAtUtc = exp,
                    user = new { user.Id, user.FirstName, user.LastName, Email = user.Email }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "JWT creation failed", detail = ex.Message });
            }
        }

        // POST api/auth/unlock-account
        [HttpPost("unlock-account")]
        public async Task<IActionResult> UnlockAccount(ConfirmEmailDto dto)  
        {
            var user = await _users.FindByEmailAsync(dto.Email);
            if (user == null) return BadRequest(new { message = "Kullanıcı bulunamadı." });

            var ok = await _codes.ValidateAsync(user.Id, CodePurpose.UnlockAccount, dto.Code);
            if (!ok) return BadRequest(new { message = "Kod geçersiz veya süresi dolmuş." });

            await _users.SetLockoutEndDateAsync(user, null);
            await _users.ResetAccessFailedCountAsync(user);

            return Ok(new { message = "Hesap kilidi kaldırıldı. Şimdi giriş yapabilirsiniz." });
        }

        // POST api/auth/change-password
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var user = await _users.GetUserAsync(User);
            if (user is null) return Unauthorized();

            // Yeni şifre mevcutla aynı mı?
            var sameAsCurrent = _users.PasswordHasher
                .VerifyHashedPassword(user, user.PasswordHash!, dto.NewPassword)
                == PasswordVerificationResult.Success;
            if (sameAsCurrent)
                return BadRequest(new { message = "Yeni şifre mevcut şifreyle aynı olamaz." });

            // Son 3 şifre ile aynı mı?
            var last3 = await _db.PasswordHistories
                .Where(h => h.UserId == user.Id)
                .OrderByDescending(h => h.CreatedAtUtc)
                .Take(3)
                .ToListAsync();

            foreach (var h in last3)
            {
                var ok = _users.PasswordHasher.VerifyHashedPassword(user, h.PasswordHash, dto.NewPassword);
                if (ok == PasswordVerificationResult.Success)
                    return BadRequest(new { message = "Yeni şifre son 3 şifre ile aynı olamaz." });
            }

            // Değiştir → eski hash'i yaz
            var oldHash = user.PasswordHash;
            var res = await _users.ChangePasswordAsync(user!, dto.OldPassword, dto.NewPassword);
            if (!res.Succeeded) return BadRequest(new { errors = res.Errors });

            _db.PasswordHistories.Add(new PasswordHistory { UserId = user.Id, PasswordHash = oldHash! });
            await _db.SaveChangesAsync();

            // Yalnızca son 3 kaydı tut
            var extraIds = await _db.PasswordHistories
                .Where(h => h.UserId == user.Id)
                .OrderByDescending(h => h.CreatedAtUtc)
                .Skip(3).Select(x => x.Id).ToListAsync();
            if (extraIds.Count > 0)
            {
                _db.PasswordHistories.RemoveRange(_db.PasswordHistories.Where(x => extraIds.Contains(x.Id)));
                await _db.SaveChangesAsync();
            }

            var when = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm 'UTC'");
            await _email.SendAsync(user!.Email!, "Şifreniz Değiştirildi",
                $"<p>Merhaba {user.FirstName},</p>" +
                $"<p>Hesabınızın şifresi <b>{when}</b> tarihinde değiştirildi.</p>" +
                $"<p>Bu işlemi siz yapmadıysanız hemen şifrenizi sıfırlayın.</p>");

            return Ok(new { message = "Şifre değiştirildi." });
        }

        // Frontend'in beklediği /api/account/me alias'ı
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<MeDto>> Me()
        {
            var user = await _users.GetUserAsync(User);
            return new MeDto { Id = user!.Id.ToString(), FirstName = user.FirstName, LastName = user.LastName, Email = user.Email! };
        }

        // alias: /api/account/me (aynı işi yapıyormuş)
        [Authorize]
        [HttpGet("/api/account/me")]
        public Task<ActionResult<MeDto>> MeAlias() => Me();

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            var user = await _users.FindByEmailAsync(dto.Email);
            if (user == null) return Ok(new { message = "Eğer kayıtlıysa mail gönderildi." }); // gizlilik

            await _codes.CreateAndSendAsync(user, CodePurpose.ResetPassword, TimeSpan.FromMinutes(10),
                code => $"<p>Şifre sıfırlama kodunuz: <b>{code}</b></p>");

            return Ok(new { message = "Eğer kayıtlıysa mail gönderildi." });
        }

        // POST api/auth/delete-account
        [Authorize]
        [HttpPost("delete-account")]
        public async Task<IActionResult> DeleteAccount([FromBody] PasswordConfirmDto dto)
        {
            var user = await _users.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var passOk = await _users.CheckPasswordAsync(user, dto.Password);
            if (!passOk) return Unauthorized(new { message = "Şifre hatalı." });

            var res = await _users.DeleteAsync(user);
            if (!res.Succeeded) return BadRequest(new { errors = res.Errors });

            return Ok(new { message = "Hesabınız kalıcı olarak silindi." });
        }

        // POST api/auth/update-profile
        [Authorize]
        [HttpPost("update-profile")]
        public async Task<ActionResult<MeDto>> UpdateProfile(UpdateProfileDto dto)
        {
            var user = await _users.GetUserAsync(User);
            if (user is null) return Unauthorized();

            user.FirstName = (dto.FirstName ?? "").Trim();
            user.LastName = (dto.LastName ?? "").Trim();

            var res = await _users.UpdateAsync(user);
            if (!res.Succeeded) return BadRequest(new { errors = res.Errors });

            return new MeDto { Id = user.Id.ToString(), FirstName = user.FirstName, LastName = user.LastName, Email = user.Email! };
        }

        // POST api/auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var user = await _users.FindByEmailAsync(dto.Email);
            if (user == null) return BadRequest(new { message = "Kullanıcı bulunamadı." });

            var ok = await _codes.ValidateAsync(user.Id, CodePurpose.ResetPassword, dto.Code);
            if (!ok) return BadRequest(new { message = "Kod geçersiz veya süresi dolmuş." });

            // mevcutla aynı mı?
            var sameAsCurrent = _users.PasswordHasher
                .VerifyHashedPassword(user, user.PasswordHash!, dto.NewPassword)
                == PasswordVerificationResult.Success;
            if (sameAsCurrent)
                return BadRequest(new { message = "Yeni şifre mevcut şifreyle aynı olamaz." });

            // son 3 ile aynı mı?
            var last3 = await _db.PasswordHistories
                .Where(h => h.UserId == user.Id)
                .OrderByDescending(h => h.CreatedAtUtc)
                .Take(3).ToListAsync();
            foreach (var h in last3)
            {
                var v = _users.PasswordHasher.VerifyHashedPassword(user, h.PasswordHash, dto.NewPassword);
                if (v == PasswordVerificationResult.Success)
                    return BadRequest(new { message = "Yeni şifre son 3 şifre ile aynı olamaz." });
            }

            var oldHash = user.PasswordHash;
            var token = await _users.GeneratePasswordResetTokenAsync(user);
            var res = await _users.ResetPasswordAsync(user, token, dto.NewPassword);
            if (!res.Succeeded) return BadRequest(new { errors = res.Errors });

            _db.PasswordHistories.Add(new PasswordHistory { UserId = user.Id, PasswordHash = oldHash! });
            await _db.SaveChangesAsync();

            var extraIds = await _db.PasswordHistories
                .Where(h => h.UserId == user.Id)
                .OrderByDescending(h => h.CreatedAtUtc)
                .Skip(3).Select(x => x.Id).ToListAsync();
            if (extraIds.Count > 0)
            {
                _db.PasswordHistories.RemoveRange(_db.PasswordHistories.Where(x => extraIds.Contains(x.Id)));
                await _db.SaveChangesAsync();
            }

            var when = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm 'UTC'");
            await _email.SendAsync(user.Email!, "Şifreniz Sıfırlandı",
                $"<p>Merhaba {user.FirstName},</p>" +
                $"<p>Hesabınızın şifresi <b>{when}</b> tarihinde sıfırlandı.</p>" +
                $"<p>Bu işlemi siz yapmadıysanız lütfen destek ile iletişime geçin.</p>");

            return Ok(new { message = "Şifre sıfırlandı." });
            }

        // POST api/auth/google-firebase
        [HttpPost("google-firebase")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleFirebase(GoogleFirebaseDto dto)
        {
            FirebaseToken decoded;
            try
            {
                decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(dto.IdToken);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Firebase token geçersiz", detail = ex.Message });
            }

            decoded.Claims.TryGetValue("email", out var emailObj);
            var email = emailObj?.ToString();
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized(new { message = "Email bulunamadı." });

            // Kullanıcı var mı bak
            var user = await _users.FindByEmailAsync(email);
            if (user == null)
            {
                user = new AppUser { UserName = email, Email = email, EmailConfirmed = true };
                await _users.CreateAsync(user);
            }

            // JWT üret
            var (token, exp) = _tokens.Create(user);
            return Ok(new { token, user = new { user.Id, user.FirstName, user.LastName, user.Email }});
        }
    }
}

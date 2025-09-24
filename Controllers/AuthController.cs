//using AuthApi.Data;
//using AuthApi.DTOs;
//using AuthApi.Models;
//using AuthApi.Services;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace AuthApi.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthController : ControllerBase
//    {
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly ApplicationDbContext _context;
//        private readonly ITokenservice _tokenService;

//        public AuthController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ITokenservice tokenService)
//        {
//            _userManager = userManager;
//            _context = context;
//            _tokenService = tokenService;
//        }

//        //[HttpPost("register")]
//        //public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
//        //{
//        //    var user = new ApplicationUser
//        //    {
//        //        UserName = dto.Username,
//        //        Email = dto.Email,
//        //    };

//        //    var result = await _userManager.CreateAsync(user, dto.Password);

//        //    if (!result.Succeeded)
//        //    {
//        //        return BadRequest(result.Errors);
//        //    }

//        //    return Ok(new { Message = "User registered successfully." });
//        //}
//        [HttpPost("register")]
//        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
//        {
//            var user = new ApplicationUser
//            {
//                UserName = dto.Username,
//                Email = dto.Email,
//            };

//            var result = await _userManager.CreateAsync(user, dto.Password);

//            if (!result.Succeeded)
//            {
//                return BadRequest(result.Errors);
//            }

//            // ubah "Message" jadi "message"
//            return Ok(new { message = "User registered successfully." });
//        }

//        //[HttpPost("login")]
//        //public async Task<IActionResult> Login([FromBody] LoginDTO dto)
//        //{
//        //    var user = await _userManager.Users
//        //        .Include(u => u.RefreshTokens)
//        //        .SingleOrDefaultAsync(u => u.Email == dto.Email);

//        //    if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
//        //    {
//        //        return Unauthorized(new { Message = "Invalid email or password." });
//        //    }

//        //    var roles = await _userManager.GetRolesAsync(user);
//        //    var accessToken = _tokenService.CreateAccessToken(user, roles);

//        //    var refreshToken = _tokenService.CreateRefreshToken(GetIpAddress());

//        //    user.RefreshTokens.Add(refreshToken);
//        //    await _userManager.UpdateAsync(user);

//        //    return Ok(new TokenRequestDTO(accessToken, refreshToken.Token));
//        //}


//        //v2

//        //[HttpPost("login")]
//        //public async Task<IActionResult> Login([FromBody] LoginDTO dto)
//        //{
//        //    var user = await _userManager.Users
//        //        .Include(u => u.RefreshTokens)
//        //        .SingleOrDefaultAsync(u => u.Email == dto.Email);

//        //    if (user == null)
//        //    {
//        //        return Unauthorized(new { Message = "Invalid email or password." });
//        //    }

//        //    // cek apakah user sedang di-ban / lockout
//        //    if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
//        //    {
//        //        return Unauthorized(new { Message = "Your account is banned due to multiple failed login attempts. Please contact admin." });
//        //    }

//        //    // validasi password
//        //    if (!await _userManager.CheckPasswordAsync(user, dto.Password))
//        //    {
//        //        await _userManager.AccessFailedAsync(user); // increment gagal login

//        //        if (user.AccessFailedCount >= 5) // jika gagal >= 5 kali
//        //        {
//        //            // ban user (misal permanen, bisa juga set waktu tertentu)
//        //            user.LockoutEnd = DateTime.UtcNow.AddYears(100); // dianggap permanent ban
//        //            await _userManager.UpdateAsync(user);

//        //            return Unauthorized(new { Message = "Your account has been banned after 5 failed login attempts." });
//        //        }

//        //        return Unauthorized(new { Message = "Invalid email or password." });
//        //    }

//        //    // reset gagal login setelah berhasil login
//        //    await _userManager.ResetAccessFailedCountAsync(user);

//        //    var roles = await _userManager.GetRolesAsync(user);
//        //    var accessToken = _tokenService.CreateAccessToken(user, roles);

//        //    var refreshToken = _tokenService.CreateRefreshToken(GetIpAddress());

//        //    user.RefreshTokens.Add(refreshToken);
//        //    await _userManager.UpdateAsync(user);

//        //    return Ok(new TokenRequestDTO(accessToken, refreshToken.Token));
//        //}
//        // v3

//        [HttpPost("login")]
//        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
//        {
//            var user = await _userManager.Users
//                .Include(u => u.RefreshTokens)
//                .SingleOrDefaultAsync(u => u.Email == dto.Email);

//            if (user == null)
//                return Unauthorized(new { Message = "Invalid email or password." });

//            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
//                return Unauthorized(new { Message = "Your account is banned. Please contact the administrator." });

//            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
//            {
//                await _userManager.AccessFailedAsync(user);
//                var attemptsLeft = 5 - user.AccessFailedCount;

//                if (user.AccessFailedCount >= 5)
//                {
//                    user.LockoutEnd = DateTime.UtcNow.AddYears(100);
//                    await _userManager.UpdateAsync(user);

//                    return Unauthorized(new { Message = "Your account has been banned after 5 failed login attempts." });
//                }

//                return Unauthorized(new { Message = $"Incorrect password. {attemptsLeft} attempt(s) left before ban." });
//            }

//            await _userManager.ResetAccessFailedCountAsync(user);

//            var roles = await _userManager.GetRolesAsync(user);
//            var accessToken = _tokenService.CreateAccessToken(user, roles);
//            var refreshToken = _tokenService.CreateRefreshToken(GetIpAddress());

//            user.RefreshTokens.Add(refreshToken);
//            await _userManager.UpdateAsync(user);

//            return Ok(new TokenRequestDTO(accessToken, refreshToken.Token));
//        }

//        [HttpGet("banned-users")]
//        [Authorize(Roles = "Admin")] 
//        public async Task<IActionResult> GetBannedUsers()
//        {
//            var bannedUsers = await _userManager.Users
//                .Where(u => u.LockoutEnd.HasValue && u.LockoutEnd.Value > DateTime.UtcNow)
//                .Select(u => new
//                {
//                    u.Id,
//                    u.UserName,
//                    u.Email,
//                    u.LockoutEnd
//                })
//                .ToListAsync();

//            return Ok(bannedUsers);
//        }

//        private string GetIpAddress()
//        {
//            if (Request.Headers.ContainsKey("X-Forwarded-For"))
//                return Request.Headers["X-Forwarded-For"].ToString();

//            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
//        }
//    }
//}

using AuthApi.Data;
using AuthApi.DTOs;
using AuthApi.Models;
using AuthApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ITokenservice _tokenService;

        public AuthController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ITokenservice tokenService)
        {
            _userManager = userManager;
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Username,
                Email = dto.Email,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return Unauthorized(new { Message = "Invalid email or password." });

            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
                return Unauthorized(new { Message = "Your account is banned. Please contact the administrator." });

            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                await _userManager.AccessFailedAsync(user);
                var attemptsLeft = 5 - user.AccessFailedCount;

                if (user.AccessFailedCount >= 5)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddYears(100);
                    await _userManager.UpdateAsync(user);

                    return Unauthorized(new { Message = "Your account has been banned after 5 failed login attempts." });
                }

                return Unauthorized(new { Message = $"Incorrect password. {attemptsLeft} attempt(s) left before ban." });
            }

            await _userManager.ResetAccessFailedCountAsync(user);

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.CreateAccessToken(user, roles);
            var refreshToken = _tokenService.CreateRefreshToken(GetIpAddress());

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            // ✅ PERBAIKAN: Kembalikan response dengan format yang benar
            var roleString = roles.FirstOrDefault() ?? "User"; // default role jika tidak ada role

            return Ok(new
            {
                accessToken = accessToken,
                refreshToken = refreshToken.Token,
                username = user.UserName,
                role = roleString
            });
        }

        [HttpGet("banned-users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetBannedUsers()
        {
            var bannedUsers = await _userManager.Users
                .Where(u => u.LockoutEnd.HasValue && u.LockoutEnd.Value > DateTime.UtcNow)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.LockoutEnd
                })
                .ToListAsync();

            return Ok(bannedUsers);
        }

        private string GetIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"].ToString();

            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleERP.API.Services;
using SimpleERP.API.Data;

namespace SimpleERP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _context;

        public AuthController(IAuthService authService, ApplicationDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username) || 
                    string.IsNullOrWhiteSpace(request.Password) || 
                    string.IsNullOrWhiteSpace(request.Email))
                {
                    return BadRequest(new { message = "모든 필드를 입력해주세요." });
                }

                if (request.Password.Length < 6)
                {
                    return BadRequest(new { message = "비밀번호는 최소 6자 이상이어야 합니다." });
                }

                var user = await _authService.RegisterAsync(request.Username, request.Email, request.Password);

                return Ok(new 
                { 
                    message = "회원가입이 완료되었습니다.",
                    user = new 
                    {
                        user.Id,
                        user.Username,
                        user.Email,
                        user.CreatedAt
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "서버 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new { message = "사용자명과 비밀번호를 입력해주세요." });
                }

                var token = await _authService.LoginAsync(request.Username, request.Password);

                return Ok(new 
                { 
                    message = "로그인 성공",
                    token = token,
                    tokenType = "Bearer"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "서버 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("me")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult GetCurrentUser()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "유효하지 않은 토큰입니다." });
                }

                var userId = int.Parse(userIdClaim.Value);
                
                return Ok(new 
                { 
                    message = "사용자 정보 조회 성공",
                    userId = userId,
                    username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "서버 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _context.Users
                    .Select(u => new 
                    {
                        u.Id,
                        u.Username,
                        u.Email,
                        u.CreatedAt,
                        u.IsActive,
                        // 보안상 실제 해시는 보여주지 않고 첫 10자만 표시
                        PasswordHash = u.PasswordHash.Substring(0, Math.Min(u.PasswordHash.Length, 10)) + "..."
                    })
                    .ToListAsync();

                return Ok(new 
                { 
                    message = "사용자 목록 조회 성공",
                    count = users.Count,
                    users = users
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "서버 오류가 발생했습니다.", error = ex.Message });
            }
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> LookupDatabase()
        {
            try
            {
                var userCount = await _context.Users.CountAsync();
                var activeUserCount = await _context.Users.CountAsync(u => u.IsActive);
                var recentUsers = await _context.Users
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(3)
                    .Select(u => new 
                    {
                        u.Id,
                        u.Username,
                        u.Email,
                        u.CreatedAt,
                        u.IsActive
                    })
                    .ToListAsync();
                
                return Ok(new 
                { 
                    message = "DB 조회 성공",
                    summary = new
                    {
                        totalUsers = userCount,
                        activeUsers = activeUserCount,
                        inactiveUsers = userCount - activeUserCount
                    },
                    recentUsers = recentUsers,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "DB 조회 실패", error = ex.Message });
            }
        }
    }

    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
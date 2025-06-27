using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;




// ===== AuthService.cs (구현체) =====
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using SimpleERP.API.Data;
using SimpleERP.API.Models;

namespace SimpleERP.API.Services
{
    /// <summary>
    /// 인증 서비스 구현체
    /// 회원가입, 로그인, JWT 토큰 생성 등의 비즈니스 로직 처리
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;  // 데이터베이스 컨텍스트
        private readonly IConfiguration _configuration;   // appsettings.json 설정값 접근

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// 회원가입 처리
        /// 1. 중복 사용자 체크
        /// 2. 비밀번호 해싱
        /// 3. DB에 사용자 저장
        /// </summary>
        public async Task<User> RegisterAsync(string username, string email, string password)
        {
            // 중복 사용자명 체크
            if (await _context.Users.AnyAsync(u => u.Username == username))
            {
                throw new InvalidOperationException("사용자명이 이미 존재합니다.");
            }

            // 중복 이메일 체크
            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                throw new InvalidOperationException("이메일이 이미 존재합니다.");
            }

            // BCrypt를 사용한 비밀번호 해싱 (보안강화)
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            // 새 사용자 객체 생성
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // 데이터베이스에 사용자 추가
            _context.Users.Add(user);
            await _context.SaveChangesAsync(); // 실제 DB에 저장

            return user;
        }

        /// <summary>
        /// 로그인 처리
        /// 1. 사용자 존재 여부 확인
        /// 2. 비밀번호 검증
        /// 3. JWT 토큰 생성 및 반환
        /// </summary>
        public async Task<string> LoginAsync(string username, string password)
        {
            // 사용자명으로 사용자 찾기
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            
            if (user == null)
            {
                throw new UnauthorizedAccessException("사용자명 또는 비밀번호가 올바르지 않습니다.");
            }

            // BCrypt를 사용한 비밀번호 검증
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("사용자명 또는 비밀번호가 올바르지 않습니다.");
            }

            // 로그인 성공 시 JWT 토큰 생성
            return GenerateJwtToken(user);
        }

        /// <summary>
        /// JWT 토큰 생성
        /// 사용자 정보를 포함한 보안 토큰 생성
        /// </summary>
        public string GenerateJwtToken(User user)
        {
            // appsettings.json에서 JWT 설정값들 가져오기
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey가 설정되지 않았습니다.");
            var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer가 설정되지 않았습니다.");
            var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience가 설정되지 않았습니다.");
            var expiryInMinutes = int.Parse(jwtSettings["ExpiryInMinutes"] ?? "60"); // 기본값 60분

            // JWT 토큰에 포함될 클레임(사용자 정보) 설정
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // 사용자 ID
                new Claim(ClaimTypes.Name, user.Username),                // 사용자명
                new Claim(ClaimTypes.Email, user.Email),                  // 이메일
                new Claim("jti", Guid.NewGuid().ToString())               // 토큰 고유 ID
            };

            // 서명키 생성
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // JWT 토큰 생성
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryInMinutes), // 만료시간 설정
                signingCredentials: credentials
            );

            // 토큰을 문자열로 변환하여 반환
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
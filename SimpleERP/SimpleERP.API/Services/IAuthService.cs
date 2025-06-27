using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// ===== IAuthService.cs (인터페이스) =====
using SimpleERP.API.Models;

namespace SimpleERP.API.Services
{
    /// <summary>
    /// 인증 관련 서비스 인터페이스
    /// 의존성 주입을 위해 인터페이스와 구현체를 분리
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// 사용자 회원가입
        /// </summary>
        Task<User> RegisterAsync(string username, string email, string password);
        
        /// <summary>
        /// 사용자 로그인 및 JWT 토큰 생성
        /// </summary>
        Task<string> LoginAsync(string username, string password);
        
        /// <summary>
        /// JWT 토큰 생성
        /// </summary>
        string GenerateJwtToken(User user);
    }
}
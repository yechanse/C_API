using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// ===== IUserService.cs (인터페이스) =====
using SimpleERP.API.Models;

namespace SimpleERP.API.Services
{
    /// <summary>
    /// 사용자 관리 서비스 인터페이스
    /// 사용자 정보 조회, 수정 등의 기능 정의
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// ID로 사용자 조회
        /// </summary>
        Task<User?> GetUserByIdAsync(int id);
        
        /// <summary>
        /// 사용자명으로 사용자 조회
        /// </summary>
        Task<User?> GetUserByUsernameAsync(string username);
        
        /// <summary>
        /// 모든 사용자 목록 조회 (관리자용)
        /// </summary>
        Task<List<User>> GetAllUsersAsync();
        
        /// <summary>
        /// 사용자 정보 업데이트
        /// </summary>
        Task<User> UpdateUserAsync(User user);
        
        /// <summary>
        /// 사용자 비활성화 (소프트 삭제)
        /// </summary>
        Task<bool> DeactivateUserAsync(int id);
    }
}
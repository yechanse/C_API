using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


// ===== UserService.cs (구현체) =====
using Microsoft.EntityFrameworkCore;
using SimpleERP.API.Data;
using SimpleERP.API.Models;

namespace SimpleERP.API.Services
{
    /// <summary>
    /// 사용자 관리 서비스 구현체
    /// 사용자 CRUD 작업 처리
    /// </summary>
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// ID로 사용자 조회
        /// 비동기 처리로 DB 성능 최적화
        /// </summary>
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Where(u => u.IsActive) // 활성 사용자만 조회
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        /// <summary>
        /// 사용자명으로 사용자 조회
        /// 로그인 등에서 사용
        /// </summary>
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Where(u => u.IsActive) // 활성 사용자만 조회
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        /// <summary>
        /// 모든 활성 사용자 목록 조회
        /// 관리자 페이지에서 사용
        /// </summary>
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.CreatedAt) // 생성일 순으로 정렬
                .ToListAsync();
        }

        /// <summary>
        /// 사용자 정보 업데이트
        /// 프로필 수정 등에 사용
        /// </summary>
        public async Task<User> UpdateUserAsync(User user)
        {
            // Entity Framework가 자동으로 변경사항 추적
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// 사용자 비활성화 (소프트 삭제)
        /// 실제 데이터는 보존하되 IsActive를 false로 설정
        /// </summary>
        public async Task<bool> DeactivateUserAsync(int id)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null)
            {
                return false; // 사용자를 찾을 수 없음
            }

            user.IsActive = false; // 비활성화
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
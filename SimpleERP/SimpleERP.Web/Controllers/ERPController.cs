using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SimpleERP.Web.Controllers
{
    /// <summary>
    /// ERP 대시보드 컨트롤러 (로그인 후 접근)
    /// </summary>
    [Authorize] // 로그인된 사용자만 접근 가능
    public class ERPController : Controller
    {
        /// <summary>
        /// ERP 대시보드 메인 페이지
        /// GET /ERP/Dashboard
        /// </summary>
        public IActionResult Dashboard()
        {
            // 현재 로그인된 사용자 정보
            ViewBag.Username = User.Identity?.Name;
            ViewBag.LoginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
            return View();
        }
    }
}
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json;
using SimpleERP.Web.Models;

namespace SimpleERP.Web.Controllers
{
    /// <summary>
    /// Customer Portal 인증 컨트롤러
    /// 로그인, 회원가입, 로그아웃 페이지 및 처리
    /// </summary>
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IHttpClientFactory httpClientFactory, ILogger<AccountController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("SimpleERP.API");
            _logger = logger;
        }

        /// <summary>
        /// 로그인 페이지 표시
        /// GET /Account/Login
        /// </summary>
        [HttpGet]
        public IActionResult Login()
        {
            // 이미 로그인된 사용자는 ERP 대시보드로 리다이렉트
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Dashboard", "ERP");
            }

            return View();
        }

        /// <summary>
        /// 로그인 처리
        /// POST /Account/Login
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                // 입력값 검증
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // API 서버에 로그인 요청
                var loginRequest = new
                {
                    username = model.Username,
                    password = model.Password
                };

                var json = JsonConvert.SerializeObject(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/auth/login", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // 로그인 성공 - API에서 받은 응답 파싱
                    var loginResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    string token = loginResponse?.token ?? "";

                    // 쿠키 기반 인증 설정 (JWT 토큰을 쿠키에 저장)
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Username),
                        new Claim("JwtToken", token) // JWT 토큰을 클레임에 저장
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    // 쿠키 인증 로그인 처리
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        claimsPrincipal);

                    _logger.LogInformation($"사용자 {model.Username} 로그인 성공");

                    // ERP 대시보드로 리다이렉트
                    return RedirectToAction("Dashboard", "ERP");
                }
                else
                {
                    // 로그인 실패
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    ModelState.AddModelError("", errorResponse?.message?.ToString() ?? "Login failed.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "로그인 처리 중 오류 발생");
                ModelState.AddModelError("", "A server error occurred. Please try again later.");
                return View(model);
            }
        }

        /// <summary>
        /// 회원가입 페이지 표시
        /// GET /Account/Register
        /// </summary>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// 회원가입 처리
        /// POST /Account/Register
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                // 입력값 검증
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // 비밀번호 확인
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("", "Passwords do not match.");
                    return View(model);
                }

                // API 서버에 회원가입 요청
                var registerRequest = new
                {
                    username = model.Username,
                    email = model.Email,
                    password = model.Password
                };

                var json = JsonConvert.SerializeObject(registerRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/auth/register", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // 회원가입 성공
                    _logger.LogInformation($"사용자 {model.Username} 회원가입 성공");
                    
                    // 성공 메시지와 함께 로그인 페이지로 리다이렉트
                    TempData["SuccessMessage"] = "Registration completed successfully. Please sign in.";
                    return RedirectToAction("Login");
                }
                else
                {
                    // 회원가입 실패
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    ModelState.AddModelError("", errorResponse?.message?.ToString() ?? "Registration failed.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "회원가입 처리 중 오류 발생");
                ModelState.AddModelError("", "A server error occurred. Please try again later.");
                return View(model);
            }
        }

        /// <summary>
        /// 로그아웃 처리
        /// POST /Account/Logout
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // 쿠키 인증 로그아웃
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            _logger.LogInformation($"사용자 {User.Identity?.Name} 로그아웃");
            
            return RedirectToAction("Login");
        }
    }
}
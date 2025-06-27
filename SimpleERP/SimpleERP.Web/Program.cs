using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// ===== 1. 서비스 등록 =====

// MVC 패턴 서비스 추가 (Model-View-Controller)
builder.Services.AddControllersWithViews();

// 쿠키 기반 인증 설정 (Customer Portal용)
// JWT 대신 쿠키를 사용하여 브라우저에서 세션 관리
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";        // 로그인 페이지 경로
        options.LogoutPath = "/Account/Logout";      // 로그아웃 경로
        options.AccessDeniedPath = "/Account/AccessDenied"; // 접근 거부 페이지
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);  // 쿠키 만료시간 60분
        options.SlidingExpiration = true;            // 활동시 만료시간 연장
        options.Cookie.HttpOnly = true;              // XSS 공격 방지
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // HTTPS 환경에서 보안 강화
    });

// HTTP 클라이언트 서비스 등록 (API 서버와 통신용)
builder.Services.AddHttpClient("SimpleERP.API", client =>
{
    // API 서버 기본 주소 설정 (appsettings.json에서 읽어옴)
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001");
    client.DefaultRequestHeaders.Add("User-Agent", "SimpleERP-CustomerPortal");
});

// 세션 상태 관리 서비스 추가
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // 세션 타임아웃 30분
    options.Cookie.HttpOnly = true;                 // 보안 강화
    options.Cookie.IsEssential = true;              // GDPR 규정 준수
});

var app = builder.Build();

// ===== 2. 미들웨어 파이프라인 설정 =====

// 개발 환경에서만 상세 에러 페이지 표시
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // 프로덕션 에러 페이지
    app.UseHsts(); // HTTP Strict Transport Security (보안 강화)
}

// HTTPS 리다이렉션 (보안을 위해 HTTP를 HTTPS로 강제 전환)
app.UseHttpsRedirection();

// 정적 파일 서비스 (CSS, JS, 이미지 등)
app.UseStaticFiles();

// 라우팅 미들웨어
app.UseRouting();

// 세션 미들웨어 (인증보다 먼저 와야 함)
app.UseSession();

// 인증 미들웨어 (쿠키 기반 인증)
app.UseAuthentication();

// 권한 검증 미들웨어
app.UseAuthorization();

// ===== 3. 라우팅 설정 =====

// 기본 라우팅: Customer Portal 접속시 바로 로그인 페이지로 이동
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"); // 기본 경로를 Account/Login으로 설정

// ===== 4. 애플리케이션 실행 =====
app.Run();
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SimpleERP.API.Data;
using SimpleERP.API.Services;

var builder = WebApplication.CreateBuilder(args);

// ===== 1. 서비스 등록 섹션 =====

// 컨트롤러 서비스 추가 (API 엔드포인트를 위해 필요)
builder.Services.AddControllers();

// Entity Framework 및 SQL Server 연결 설정
// appsettings.json의 DefaultConnection 문자열을 사용하여 DB 연결
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT 인증 설정
// appsettings.json의 JwtSettings 섹션에서 설정값들을 가져옴
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey가 설정되지 않았습니다.");

// JWT 토큰 검증을 위한 인증 서비스 등록
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,          // 토큰 발행자 검증
            ValidateAudience = true,        // 토큰 수신자 검증
            ValidateLifetime = true,        // 토큰 만료시간 검증
            ValidateIssuerSigningKey = true, // 토큰 서명키 검증
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

// 비즈니스 로직을 처리할 서비스들을 의존성 주입 컨테이너에 등록
builder.Services.AddScoped<IAuthService, AuthService>(); // 인증 관련 서비스
builder.Services.AddScoped<IUserService, UserService>(); // 사용자 관리 서비스

// CORS 정책 설정 (프론트엔드에서 API 호출을 허용하기 위해)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp", policy =>
    {
        policy.WithOrigins("https://localhost:5001", "http://localhost:5000", "https://localhost:5192", "http://localhost:5192") // Web 프로젝트 URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Swagger/OpenAPI 문서 생성 (API 테스트용)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ===== 2. 애플리케이션 빌드 =====
var app = builder.Build();

// ===== 3. 미들웨어 파이프라인 설정 =====

// 개발 환경에서만 Swagger UI 활성화
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS 리다이렉션 (보안을 위해)
app.UseHttpsRedirection();

// CORS 미들웨어 적용
app.UseCors("AllowWebApp");

// 인증 미들웨어 (JWT 토큰 검증)
app.UseAuthentication();

// 권한 검증 미들웨어
app.UseAuthorization();

// 컨트롤러 라우팅 매핑
app.MapControllers();

// ===== 4. 애플리케이션 실행 =====
app.Run();
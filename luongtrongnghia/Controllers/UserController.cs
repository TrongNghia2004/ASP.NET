using luongtrongnghia.Model;
using luongtrongnghia.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using luongtrongnghia.Services;
using BCrypt.Net;

namespace luongtrongnghia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext pro;
        private readonly JwtService _jwtService;

        public UserController(AppDbContext context, JwtService jwtService)
        {
            pro = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            if (pro.Users.Any(u => u.Email == model.Email))
                return BadRequest("Email đã được sử dụng");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Password = hashedPassword,
                Address = model.Address,
                Description = model.Description
            };

            pro.Users.Add(user);
            await pro.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user.Email);

            return Ok(new { message = "Đăng ký thành công", token, user.Name, user.Email });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Login model)
        {
            // Lấy token từ header
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader))
                return Unauthorized("Bạn cần cung cấp token");

            var token = authHeader.StartsWith("Bearer ") ? authHeader.Substring("Bearer ".Length) : authHeader;

            var emailFromToken = _jwtService.ValidateToken(token);
            if (emailFromToken == null)
                return Unauthorized("Token không hợp lệ");

            // So sánh token với email người dùng đang nhập
            if (emailFromToken != model.Email)
                return Unauthorized("Token không khớp với email đăng nhập");

            // Kiểm tra user + mật khẩu
            var user = pro.Users.SingleOrDefault(u => u.Email == model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                return Unauthorized("Sai email hoặc mật khẩu");

            // Tạo token mới
            var newToken = _jwtService.GenerateToken(user.Email);

            return Ok(new { message = "Đăng nhập thành công", token = newToken, user.Name, user.Email });
        }



    }
}

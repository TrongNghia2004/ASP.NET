using luongtrongnghia.Model;
using luongtrongnghia.Data;
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

        // ------------------ AUTH ------------------

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
            // var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            // if (string.IsNullOrEmpty(authHeader))
            //     return Unauthorized("Bạn cần cung cấp token");

            // var token = authHeader.StartsWith("Bearer ") ? authHeader.Substring("Bearer ".Length) : authHeader;
            // var emailFromToken = _jwtService.ValidateToken(token);
            // if (emailFromToken == null)
            //     return Unauthorized("Token không hợp lệ");

            // if (emailFromToken != model.Email)
            //     return Unauthorized("Token không khớp với email đăng nhập");

            var user = pro.Users.SingleOrDefault(u => u.Email == model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                return Unauthorized("Sai email hoặc mật khẩu");

              var token = _jwtService.GenerateToken(user.Email);

            return Ok(new { message = "Đăng nhập thành công", token, user.Name,user.Id, user.Email });
        }

        // ------------------ CRUD ------------------

        // GET: api/user
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = pro.Users.ToList();
            return Ok(new { users });
        }

        // GET: api/user/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = pro.Users.Find(id);
            if (user == null) return NotFound("Không tìm thấy người dùng");
            return Ok(user);
        }

        // // POST: api/user
        // [HttpPost]
        // public async Task<IActionResult> Create([FromBody] User model)
        // {
        //     if (pro.Users.Any(u => u.Email == model.Email))
        //         return BadRequest("Email đã tồn tại");

        //     model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);

        //     pro.Users.Add(model);
        //     await pro.SaveChangesAsync();
        //     return Ok(new { message = "Thêm người dùng thành công", user = model });
        // }

        // PUT: api/user/{id}
       [HttpPut("{id}")]
public async Task<IActionResult> Update(int id, [FromBody] User model)
{
    var user = pro.Users.Find(id);
    if (user == null) return NotFound("Không tìm thấy người dùng");

    user.Name = model.Name;
    user.Email = model.Email;
    user.Phone = model.Phone;
    user.Address = model.Address;
    user.Description = model.Description;

    // Chỉ hash password nếu nó không rỗng
    if (!string.IsNullOrEmpty(model.Password))
        user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);

    await pro.SaveChangesAsync();
    return Ok(new { message = "Cập nhật người dùng thành công", user });
}



        // DELETE: api/user/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await pro.Users.FindAsync(id);
            if (user == null) return NotFound("Không tìm thấy người dùng");

            pro.Users.Remove(user);
            await pro.SaveChangesAsync();
            return Ok(new { message = "Xóa người dùng thành công" });
        }
    }
}

// using luongtrongnghia.Model;
// using luongtrongnghia.Data;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using luongtrongnghia.Services;
// using BCrypt.Net;

// namespace luongtrongnghia.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class UserController : ControllerBase
//     {
//         private readonly AppDbContext pro;
//         private readonly JwtService _jwtService;

//         public UserController(AppDbContext context, JwtService jwtService)
//         {
//             pro = context;
//             _jwtService = jwtService;
//         }

//         [HttpPost("register")]
//         public async Task<IActionResult> Register([FromBody] Register model)
//         {
//             if (pro.Users.Any(u => u.Email == model.Email))
//                 return BadRequest("Email đã được sử dụng");

//             var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

//             var user = new User
//             {
//                 Name = model.Name,
//                 Email = model.Email,
//                 Phone = model.Phone,
//                 Password = hashedPassword,
//                 Address = model.Address,
//                 Description = model.Description
//             };

//             pro.Users.Add(user);
//             await pro.SaveChangesAsync();

//             var token = _jwtService.GenerateToken(user.Email);

//             return Ok(new { message = "Đăng ký thành công", token, user.Name, user.Email });
//         }

//         [HttpPost("login")]
//         public IActionResult Login([FromBody] Login model)
//         {
//             // Lấy token từ header
//             var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
//             if (string.IsNullOrEmpty(authHeader))
//                 return Unauthorized("Bạn cần cung cấp token");

//             var token = authHeader.StartsWith("Bearer ") ? authHeader.Substring("Bearer ".Length) : authHeader;

//             var emailFromToken = _jwtService.ValidateToken(token);
//             if (emailFromToken == null)
//                 return Unauthorized("Token không hợp lệ");

//             // So sánh token với email người dùng đang nhập
//             if (emailFromToken != model.Email)
//                 return Unauthorized("Token không khớp với email đăng nhập");

//             // Kiểm tra user + mật khẩu
//             var user = pro.Users.SingleOrDefault(u => u.Email == model.Email);
//             if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
//                 return Unauthorized("Sai email hoặc mật khẩu");

//             // Tạo token mới
//             var newToken = _jwtService.GenerateToken(user.Email);

//             return Ok(new { message = "Đăng nhập thành công", token = newToken, user.Name, user.Email });
//         }



//     }
// }

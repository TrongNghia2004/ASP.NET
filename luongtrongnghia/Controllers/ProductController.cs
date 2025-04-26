using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using luongtrongnghia.Data;
using luongtrongnghia.Model;

namespace luongtrongnghia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync(); //  Không Include Category
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id); //  Không Include

            if (product == null)
                return NotFound();

            return product;
        }

        // // GET: api/Product/ByCategory/3
        // [HttpGet("ByCategory/{categoryId}")]
        // public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(int categoryId)
        // {
        //     return await _context.Products
        //                          .Where(p => p.CategoryId == categoryId)
        //                          .ToListAsync(); // ❌ Không Include
        // }

        // POST: api/Product
        // [HttpPost]
        // public async Task<ActionResult<Product>> PostProduct(Product product)
        // {
        //     _context.Products.Add(product);
        //     await _context.SaveChangesAsync();

        //     return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        // }

        
        [HttpPost]
public async Task<ActionResult<Product>> PostProduct(
    [FromForm] string name,
    [FromForm] string description,
    [FromForm] double price,
    [FromForm] int quantity,
    [FromForm] int categoryId,
    [FromForm] IFormFile imageFile)
{
    if (imageFile == null || imageFile.Length == 0)
    {
        return BadRequest("Ảnh không hợp lệ.");
    }

    // Tạo thư mục images nếu chưa có
    var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
    if (!Directory.Exists(imageFolder))
    {
        Directory.CreateDirectory(imageFolder);
    }

    // Lưu ảnh vào thư mục wwwroot/images
    var fileName = Path.GetFileName(imageFile.FileName);
    var filePath = Path.Combine(imageFolder, fileName);

    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await imageFile.CopyToAsync(stream);
    }

    // Tạo đường dẫn URL tương đối để lưu vào DB
    var imageUrl = fileName;

    // Tạo sản phẩm mới
    var product = new Product
    {
        Name = name,
        Description = description,
        Price = price,
        Quantity=quantity,
        CategoryId = categoryId,
        ImageUrl = imageUrl
    };

    _context.Products.Add(product);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
}


        // PUT: api/Product/5
        // [HttpPut("{id}")]
        // public async Task<IActionResult> PutProduct(int id, Product product)
        // {
        //     if (id != product.Id)
        //         return BadRequest();

        //     _context.Entry(product).State = EntityState.Modified;

        //     try
        //     {
        //         await _context.SaveChangesAsync();
        //     }
        //     catch (DbUpdateConcurrencyException)
        //     {
        //         if (!_context.Products.Any(p => p.Id == id))
        //             return NotFound();
        //         else
        //             throw;
        //     }

        //     return NoContent();
        // }
        [HttpPut("{id}")]
public async Task<IActionResult> PutProduct(int id, 
    [FromForm] string name, 
    [FromForm] string description, 
    [FromForm] double price, 
    [FromForm] int quantity, 
    [FromForm] int categoryId, 
    [FromForm] IFormFile? imageFile) // imageFile có thể là null
{
    var product = await _context.Products.FindAsync(id);
    if (product == null)
    {
        return NotFound();
    }

    product.Name = name;
    product.Description = description;
    product.Price = price;
    product.Quantity = quantity;
    product.CategoryId = categoryId;

    // Chỉ xử lý ảnh khi có ảnh mới
    if (imageFile != null && imageFile.Length > 0)
    {
        // Xử lý ảnh mới, lưu vào thư mục và cập nhật đường dẫn
        var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
        if (!Directory.Exists(imageFolder))
        {
            Directory.CreateDirectory(imageFolder);
        }

        var fileName = Path.GetFileName(imageFile.FileName);
        var filePath = Path.Combine(imageFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        product.ImageUrl = fileName;
    }

    await _context.SaveChangesAsync();

    return NoContent(); // Trả về status 204 khi cập nhật thành công
}




        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using luongtrongnghia.Data;
// using luongtrongnghia.Model;
// using System.IO;

// namespace luongtrongnghia.Controllers
// {
//     [Route("api/[controller]")]
//     [ApiController]
//     public class ProductController : ControllerBase
//     {
//         private readonly AppDbContext _context;
//         private readonly string _uploadFolder;

//         public ProductController(AppDbContext context)
//         {
//             _context = context;
//             _uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
//             if (!Directory.Exists(_uploadFolder))
//             {
//                 Directory.CreateDirectory(_uploadFolder);
//             }
//         }

//         // GET: api/Product
//         [HttpGet]
//         public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
//         {
//             return await _context.Products.ToListAsync();
//         }

//         // GET: api/Product/5
//         [HttpGet("{id}")]
//         public async Task<ActionResult<Product>> GetProduct(int id)
//         {
//             var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

//             if (product == null)
//                 return NotFound();

//             return product;
//         }

//         // POST: api/Product
//         [HttpPost]
//         public async Task<ActionResult<Product>> PostProduct(Product product)
//         {
//             _context.Products.Add(product);
//             await _context.SaveChangesAsync();

//             return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
//         }

//         // PUT: api/Product/5
//         [HttpPut("{id}")]
//         public async Task<IActionResult> PutProduct(int id, Product product)
//         {
//             if (id != product.Id)
//                 return BadRequest();

//             _context.Entry(product).State = EntityState.Modified;

//             try
//             {
//                 await _context.SaveChangesAsync();
//             }
//             catch (DbUpdateConcurrencyException)
//             {
//                 if (!_context.Products.Any(p => p.Id == id))
//                     return NotFound();
//                 else
//                     throw;
//             }

//             return NoContent();
//         }

//         // DELETE: api/Product/5
//         [HttpDelete("{id}")]
//         public async Task<IActionResult> DeleteProduct(int id)
//         {
//             var product = await _context.Products.FindAsync(id);
//             if (product == null)
//                 return NotFound();

//             _context.Products.Remove(product);
//             await _context.SaveChangesAsync();

//             return NoContent();
//         }

//         // POST: api/Product/UploadImage/5
//         [HttpPost("UploadImage/{id}")]
// public async Task<IActionResult> UploadImage(int id, IFormFile file)
// {
//     if (file == null || file.Length == 0)
//         return BadRequest("No file uploaded.");

//     var product = await _context.Products.FindAsync(id);
//     if (product == null)
//         return NotFound();

//     var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
//     var filePath = Path.Combine(_uploadFolder, fileName);

//     using (var fileStream = new FileStream(filePath, FileMode.Create))
//     {
//         await file.CopyToAsync(fileStream);
//     }

//     // ✅ Lưu URL ảnh hoàn chỉnh (dùng / thay vì \)
//     product.ImageUrl = $"{Request.Scheme}://{Request.Host}/images/{fileName}";
//     _context.Products.Update(product);
//     await _context.SaveChangesAsync();

//     return Ok(new { imageUrl = product.ImageUrl });
// }


//         // GET: api/Product/5/Image
//         [HttpGet("{id}/Image")]
//         public IActionResult GetImage(int id)
//         {
//             var product = _context.Products.FirstOrDefault(p => p.Id == id);
//             if (product == null || string.IsNullOrEmpty(product.ImageUrl))
//                 return NotFound();

//             var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.ImageUrl);
//             if (!System.IO.File.Exists(filePath))
//                 return NotFound();

//             var fileBytes = System.IO.File.ReadAllBytes(filePath);
//             return File(fileBytes, "image/jpeg");  // Đảm bảo trả về đúng kiểu MIME của tệp (jpeg, png, ...)
//         }
//     }
// }

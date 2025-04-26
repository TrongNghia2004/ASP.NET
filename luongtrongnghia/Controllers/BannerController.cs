using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using luongtrongnghia.Data;
using luongtrongnghia.Model;

namespace luongtrongnghia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private readonly AppDbContext pro;

        public BannerController(AppDbContext context)
        {
            pro = context;
        }

        // GET: api/Banner
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Banner>>> GetBanners()
        {
            return await pro.Banners.ToListAsync();
        }

        // GET: api/Banner/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Banner>> GetBanner(int id)
        {
            var banner = await pro.Banners.FindAsync(id);

            if (banner == null)
                return NotFound();

            return banner;
        }

        // POST: api/Banner
        [HttpPost]
        public async Task<ActionResult<Banner>> PostBanner(
            [FromForm] string name,
            [FromForm] string description,
            [FromForm] IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("Ảnh không hợp lệ.");

            var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            if (!Directory.Exists(imageFolder))
                Directory.CreateDirectory(imageFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(imageFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            var banner = new Banner
            {
                Name = name,
                Description = description,
                ImageUrl = fileName
            };

            pro.Banners.Add(banner);
            await pro.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBanner), new { id = banner.Id }, banner);
        }

        // PUT: api/Banner/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBanner(int id,
            [FromForm] string name,
            [FromForm] string description,
            [FromForm] IFormFile? imageFile)
        {
            var banner = await pro.Banners.FindAsync(id);
            if (banner == null)
                return NotFound();

            banner.Name = name;
            banner.Description = description;

            if (imageFile != null && imageFile.Length > 0)
            {
                var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(imageFolder))
                    Directory.CreateDirectory(imageFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(imageFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                banner.ImageUrl = fileName;
            }

            await pro.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Banner/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBanner(int id)
        {
            var banner = await pro.Banners.FindAsync(id);
            if (banner == null)
                return NotFound();

            pro.Banners.Remove(banner);
            await pro.SaveChangesAsync();

            return NoContent();
        }
    }
}

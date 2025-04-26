using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using luongtrongnghia.Data;
using luongtrongnghia.Model;

namespace luongtrongnghia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext pro;

        public CartController(AppDbContext context)
        {
            pro = context;
        }

        // GET: api/Cart
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCarts()
        {
            return await pro.Carts.ToListAsync();
        }

        // GET: api/Cart/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCart(int userId)
        {
            var carts = await pro.Carts
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return Ok(carts);
        }

        // POST: api/Cart
        [HttpPost]
        public async Task<ActionResult<Cart>> PostCart([FromBody] Cart cart)
        {
            try
            {
                var existingCartItem = await pro.Carts
                    .FirstOrDefaultAsync(c => c.UserId == cart.UserId && c.ProductId == cart.ProductId);

                if (existingCartItem != null)
                {
                    // Nếu sản phẩm đã có trong giỏ, tăng số lượng
                    existingCartItem.Quantity += cart.Quantity;
                    pro.Entry(existingCartItem).State = EntityState.Modified;
                }
                else
                {
                    // Nếu chưa có, thêm mới
                    pro.Carts.Add(cart);
                }

                await pro.SaveChangesAsync();
                return Ok(cart);
            }
            catch (DbUpdateException dbEx)
            {
                Console.Error.WriteLine(dbEx.InnerException?.Message ?? dbEx.Message);
                return BadRequest(new { error = dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // PUT: api/Cart/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCart(int id, Cart cart)
        {
            if (id != cart.Id)
                return BadRequest();

            pro.Entry(cart).State = EntityState.Modified;

            try
            {
                await pro.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!pro.Carts.Any(c => c.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Cart/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            var cart = await pro.Carts.FindAsync(id);
            if (cart == null)
                return NotFound();

            pro.Carts.Remove(cart);
            await pro.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Cart/user/{userId} - Xoá toàn bộ giỏ hàng theo user
        [HttpDelete("user/{userId}")]
        public async Task<IActionResult> DeleteCartByUser(int userId)
        {
            var userCarts = await pro.Carts
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (userCarts.Count == 0)
                return NotFound();

            pro.Carts.RemoveRange(userCarts);
            await pro.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("remove")]
public async Task<IActionResult> RemoveMultiple([FromBody] RemoveCartItemsRequest request)
{
    if (request == null || request.ProductIds == null || request.ProductIds.Count == 0)
        return BadRequest("Danh sách sản phẩm không hợp lệ.");

    var itemsToRemove = await pro.Carts
        .Where(c => c.UserId == request.UserId && request.ProductIds.Contains(c.ProductId))
        .ToListAsync();

    if (itemsToRemove.Count == 0)
        return NotFound("Không tìm thấy sản phẩm để xóa.");

    pro.Carts.RemoveRange(itemsToRemove);
    await pro.SaveChangesAsync();

    return Ok(new { message = "Đã xóa thành công các sản phẩm khỏi giỏ hàng." });
}

    }
}

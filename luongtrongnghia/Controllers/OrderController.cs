using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using luongtrongnghia.Model; // Thêm namespace của Model nếu cần
using luongtrongnghia.Data;  // Thêm namespace của DbContext nếu cần

namespace luongtrongnghia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext pro;

        public OrderController(AppDbContext context)
        {
            pro = context;
        }

        // GET: api/Order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await pro.Orders
                /*.Include(o => o.OrderDetails)*/ // Bao gồm các chi tiết đơn hàng
                .ToListAsync();
        }


        // GET: api/Order/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await pro.Orders
                /*.Include(o => o.OrderDetails)*/ // Bao gồm các chi tiết đơn hàng
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // POST: api/Order
        [HttpPost]
// Thêm đơn hàng cùng với các chi tiết đơn hàng và trừ số lượng sản phẩm
public async Task<ActionResult<Order>> PostOrder(Order order)
{
    // Kiểm tra các trường thông tin khách hàng
    if (string.IsNullOrEmpty(order.Name) || string.IsNullOrEmpty(order.Phone) ||
        string.IsNullOrEmpty(order.Email) || string.IsNullOrEmpty(order.Address))
    {
        return BadRequest("Các trường Name, Phone, Email và Address là bắt buộc.");
    }

    // Kiểm tra danh sách chi tiết đơn hàng
    if (order.OrderDetails == null || !order.OrderDetails.Any())
    {
        return BadRequest("Đơn hàng phải có ít nhất một sản phẩm.");
    }

    // Duyệt qua từng OrderDetail
    foreach (var detail in order.OrderDetails)
    {
        var product = await pro.Products.FindAsync(detail.ProductId);
        if (product == null)
        {
            return BadRequest($"Sản phẩm với ID {detail.ProductId} không tồn tại.");
        }

        if (product.Quantity < detail.Quantity)
        {
            return BadRequest($"Sản phẩm '{product.Name}' không đủ hàng. Hiện có: {product.Quantity}, cần: {detail.Quantity}");
        }

        // Trừ số lượng sản phẩm
        product.Quantity -= detail.Quantity;
        pro.Entry(product).State = EntityState.Modified;
    }

    // Lưu đơn hàng và chi tiết đơn hàng
    pro.Orders.Add(order);
    await pro.SaveChangesAsync();

    return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
}









        // PUT: api/Order/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            pro.Entry(order).State = EntityState.Modified;

            try
            {
                await pro.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Order/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await pro.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            pro.Orders.Remove(order);
            await pro.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return pro.Orders.Any(e => e.Id == id);
        }

        // 1. GET: api/Order/GetOrdersByUserId/{userId}
        [HttpGet("GetOrdersByUserId/{userId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByUserId(int userId)
        {
            var orders = await pro.Orders
                .Where(o => o.UserId == userId) // Lọc đơn hàng theo userId
                .Include(o => o.OrderDetails)   // Bao gồm các chi tiết đơn hàng
                .ToListAsync();

            if (orders == null || !orders.Any())
            {
                return NotFound("Không có đơn hàng nào của người dùng này.");
            }

            return orders;
        }

        // 2. GET: api/Order/GetOrderDetail/{orderId}
        [HttpGet("GetOrderDetail/{orderId}")]
        public async Task<ActionResult<Order>> GetOrderDetail(int orderId)
        {
            var order = await pro.Orders
                .Include(o => o.OrderDetails) // Bao gồm chi tiết đơn hàng
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound($"Đơn hàng với ID {orderId} không tồn tại.");
            }

            return order; // Trả về đơn hàng với chi tiết sản phẩm
        }
    }

}

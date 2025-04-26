using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using luongtrongnghia.Model;
using luongtrongnghia.Data;

namespace luongtrongnghia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly AppDbContext pro;

        public OrderDetailController(AppDbContext context)
        {
            pro = context;
        }

        // GET: api/OrderDetail
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetail>>> GetOrderDetails()
        {
            return await pro.OrderDetails
                /*.Include(od => od.Order) */ // Bao gồm thông tin Order
                //.Include(od => od.Product) // Bao gồm thông tin Product
                .ToListAsync();
        }

        // GET: api/OrderDetail/5
       [HttpGet("{id}")]
public async Task<ActionResult<OrderDetail>> GetOrderDetail(int id)
{
    if (id <= 0)
    {
        return BadRequest("ID không hợp lệ");
    }

    var orderDetail = await pro.OrderDetails
        .FirstOrDefaultAsync(od => od.Id == id);

    if (orderDetail == null)
    {
        return NotFound();
    }

    return orderDetail;
}


        // POST: api/OrderDetail
       [HttpPost]
public async Task<ActionResult<OrderDetail>> PostOrderDetail(OrderDetail orderDetail)
{
    var order = await pro.Orders.FindAsync(orderDetail.OrderId);
    var product = await pro.Products.FindAsync(orderDetail.ProductId);

    if (order == null || product == null)
    {
        return BadRequest("Đơn hàng hoặc sản phẩm không tồn tại.");
    }

    if (product.Quantity < orderDetail.Quantity)
    {
        return BadRequest("Không đủ hàng trong kho.");
    }

    // Trừ số lượng sản phẩm
    product.Quantity -= orderDetail.Quantity;

    // Đảm bảo Entity được đánh dấu là Modified
    pro.Entry(product).State = EntityState.Modified;

    // Thêm OrderDetail
    pro.OrderDetails.Add(orderDetail);

    // Lưu thay đổi
    await pro.SaveChangesAsync();

    return CreatedAtAction(nameof(GetOrderDetail), new { id = orderDetail.Id }, orderDetail);
}



        // PUT: api/OrderDetail/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderDetail(int id, OrderDetail orderDetail)
        {
            if (id != orderDetail.Id)
            {
                return BadRequest();
            }

            // Kiểm tra lại sự tồn tại của OrderId và ProductId
            var order = await pro.Orders.FindAsync(orderDetail.OrderId);
            var product = await pro.Products.FindAsync(orderDetail.ProductId);

            if (order == null || product == null)
            {
                return BadRequest("Đơn hàng hoặc sản phẩm không tồn tại.");
            }

            pro.Entry(orderDetail).State = EntityState.Modified;

            try
            {
                await pro.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderDetailExists(id))
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

        // DELETE: api/OrderDetail/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderDetail(int id)
        {
            var orderDetail = await pro.OrderDetails.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            pro.OrderDetails.Remove(orderDetail);
            await pro.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderDetailExists(int id)
        {
            return pro.OrderDetails.Any(e => e.Id == id);
        }


        [HttpGet("order/{orderId}")]
public async Task<IActionResult> GetByOrderId(int orderId)
{
    var orderDetails = await pro.OrderDetails
        .Where(od => od.OrderId == orderId)
        // .Include(od => od.Product) // nếu bạn cần thông tin sản phẩm
        .ToListAsync();

    return Ok(orderDetails);
}


    }
}

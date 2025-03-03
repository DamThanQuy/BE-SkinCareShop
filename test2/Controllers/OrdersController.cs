using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test2.Models;
using test2.DTOs;

namespace test2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly Test3Context _context;

        public OrdersController(Test3Context context)
        {
            _context = context;
        }

        #region Order Management

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .ToListAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách đơn hàng", error = ex.Message });
            }
        }

        // GET: api/Orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.OrderId == id);

                if (order == null)
                {
                    return NotFound("Không tìm thấy đơn hàng.");
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy thông tin đơn hàng", error = ex.Message });
            }
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            try
            {
                if (order == null)
                {
                    return BadRequest("Dữ liệu đơn hàng không hợp lệ");
                }

                // Thiết lập các giá trị mặc định
                order.OrderDate = DateTime.Now;
                if (string.IsNullOrEmpty(order.OrderStatus))
                    order.OrderStatus = "Pending";
                if (string.IsNullOrEmpty(order.DeliveryStatus))
                    order.DeliveryStatus = "Not Delivered";

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetOrder", new { id = order.OrderId }, order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tạo đơn hàng", error = ex.Message });
            }
        }

        // PUT: api/Orders/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, Order order)
        {
            if (id != order.OrderId)
            {
                return BadRequest("ID đơn hàng không khớp");
            }

            try
            {
                _context.Entry(order).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound("Không tìm thấy đơn hàng");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật đơn hàng", error = ex.Message });
            }
        }

        // DELETE: api/Orders/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    return NotFound("Không tìm thấy đơn hàng");
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Đã xóa đơn hàng thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa đơn hàng", error = ex.Message });
            }
        }

        // POST: api/Orders/checkout
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDto request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Kiểm tra user tồn tại
                var user = await _context.Users.FindAsync(request.UserId);
                if (user == null)
                {
                    return NotFound("Không tìm thấy người dùng.");
                }

                // Lấy giỏ hàng của user
                var cartItems = await _context.Carts
                    .Where(c => c.UserId == request.UserId && c.Status == "Active")
                    .Include(c => c.Product)
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    return BadRequest("Giỏ hàng trống.");
                }

                decimal totalAmount = cartItems.Sum(item => item.Price * item.Quantity);

                // Tạo đơn hàng mới
                var order = new Order
                {
                    UserId = request.UserId,
                    OrderDate = DateTime.Now,
                    OrderStatus = "Pending",
                    DeliveryStatus = "Not Delivered",
                    DeliveryAddress = request.DeliveryAddress,
                    TotalAmount = totalAmount,
                    Note = request.Note
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Chuyển các mục từ giỏ hàng sang order items
                foreach (var cartItem in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        Price = cartItem.Price,
                        ProductName = cartItem.Product.ProductName
                    };
                    _context.OrderItems.Add(orderItem);

                    // Đánh dấu giỏ hàng đã được chuyển thành đơn hàng
                    cartItem.Status = "Ordered";
                    _context.Entry(cartItem).State = EntityState.Modified;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { 
                    message = "Đặt hàng thành công", 
                    orderId = order.OrderId 
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Lỗi khi tạo đơn hàng", error = ex.Message });
            }
        }

        // PUT: api/Orders/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto request)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    return NotFound("Không tìm thấy đơn hàng.");
                }

                order.OrderStatus = request.OrderStatus;
                order.DeliveryStatus = request.DeliveryStatus;

                _context.Entry(order).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Cập nhật trạng thái đơn hàng thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật trạng thái đơn hàng", error = ex.Message });
            }
        }

        #endregion

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}

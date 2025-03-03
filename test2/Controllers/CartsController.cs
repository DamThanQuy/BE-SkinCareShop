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
    public class CartsController : ControllerBase
    {
        private readonly Test3Context _context;

        public CartsController(Test3Context context)
        {
            _context = context;
        }

        // GET: api/Carts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCarts()
        {
            return await _context.Carts.ToListAsync();
        }

        // GET: api/Carts/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Cart>>> GetUserCart(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound("Không tìm thấy người dùng.");
                }

                var cartItems = await _context.Carts
                    .Where(c => c.UserId == userId && c.Status == "Active")
                    .Include(c => c.Product)
                    .ToListAsync();

                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy giỏ hàng", error = ex.Message });
            }
        }

        // GET: api/Carts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cart>> GetCart(int id)
        {
            var cart = await _context.Carts.FindAsync(id);

            if (cart == null)
            {
                return NotFound();
            }

            return cart;
        }

        // PUT: api/Carts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCart(int id, Cart cart)
        {
            if (id != cart.CartItemId)
            {
                return BadRequest();
            }

            _context.Entry(cart).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartExists(id))
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

        // POST: api/Carts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Cart>> PostCart(Cart cart)
        {
            try 
            {
                // Thêm thời gian tạo và cập nhật
                cart.CreatedAt = DateTime.Now;
                cart.UpdatedAt = DateTime.Now;
                
                // Đặt status mặc định nếu chưa có
                if (string.IsNullOrEmpty(cart.Status))
                {
                    cart.Status = "Active";
                }

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetCart", new { id = cart.CartItemId }, cart);
            }
            catch (Exception ex)
            {
                // Log lỗi để debug
                return BadRequest(new { message = "Không thể thêm vào giỏ hàng", error = ex.InnerException?.Message });
            }
        }

        // POST: api/Carts/additem
        [HttpPost("additem")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequestDto request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Dữ liệu không hợp lệ");
                }

                if (request.ProductId <= 0 || request.Quantity <= 0)
                {
                    return BadRequest("ID sản phẩm hoặc số lượng không hợp lệ.");
                }

                // Kiểm tra sản phẩm tồn tại và còn hàng
                var product = await _context.Products.FindAsync(request.ProductId);
                if (product == null)
                {
                    return NotFound("Không tìm thấy sản phẩm.");
                }

                // Kiểm tra user tồn tại
                var user = await _context.Users.FindAsync(request.UserId);
                if (user == null)
                {
                    return NotFound("Không tìm thấy người dùng.");
                }

                // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
                var existingCart = await _context.Carts
                    .FirstOrDefaultAsync(c => c.UserId == request.UserId 
                        && c.ProductId == request.ProductId 
                        && c.Status == "Active");

                if (existingCart != null)
                {
                    // Nếu đã có, cập nhật số lượng
                    existingCart.Quantity += request.Quantity;
                    existingCart.UpdatedAt = DateTime.Now;
                    _context.Entry(existingCart).State = EntityState.Modified;
                }
                else
                {
                    // Nếu chưa có, tạo mới
                    var cart = new Cart
                    {
                        ProductId = request.ProductId,
                        UserId = request.UserId,
                        Quantity = request.Quantity,
                        Price = product.Price,
                        Status = "Active",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.Carts.Add(cart);
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = "Thêm vào giỏ hàng thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi thêm vào giỏ hàng", error = ex.Message });
            }
        }

        // DELETE: api/Carts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Carts/removeitem/{cartItemId}
        [HttpDelete("removeitem/{cartItemId}")]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            try
            {
                var cartItem = await _context.Carts.FindAsync(cartItemId);
                if (cartItem == null)
                {
                    return NotFound("Không tìm thấy sản phẩm trong giỏ hàng.");
                }

                _context.Carts.Remove(cartItem);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Đã xóa sản phẩm khỏi giỏ hàng" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa sản phẩm khỏi giỏ hàng", error = ex.Message });
            }
        }

        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.CartItemId == id);
        }
    }
}

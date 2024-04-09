using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backendaspnet.Models;
using Microsoft.AspNetCore.Cors;
using static NuGet.Packaging.PackagingConstants;

namespace backendaspnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
	[EnableCors("AllowOrigin")]
	public class OrdersController : ControllerBase
    {
        private readonly BackendContext _context;

        public OrdersController(BackendContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
          if (_context.Orders == null)
          {
              return NotFound();
          }
            return await _context.Orders.ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
          if (_context.Orders == null)
          {
              return NotFound();
          }
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
          if (_context.Orders == null)
          {
              return Problem("Entity set 'BackendContext.Orders'  is null.");
          }
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }
		[HttpGet("byemail/{email}")]
		public async Task<ActionResult<List<Order>>> GetOrdersByEmail(string email)
		{
			try
			{
				var ordersByEmail = await _context.Orders
					.Where(o => o.Email == email)
					.ToListAsync();

				if (ordersByEmail == null || ordersByEmail.Count == 0)
				{
					return NotFound("No orders found for the given email");
				}

				return Ok(ordersByEmail);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to get orders: {ex.Message}");
			}
		}
		// DELETE: api/Orders/5
		[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
		[HttpGet("Orders")]
		public async Task<IActionResult> GetOrders(int page = 1, int perPage = 10, string sort = "id", string filter = "{}", string order = "ASC")
		{
			try
			{
				var query = _context.Orders.AsQueryable();

				// Apply filters here if necessary

				var startIndex = (page - 1) * perPage;

				var orders = await query
				                    .OrderBy(o => o.Id)
									.Skip(startIndex)
									.Take(perPage)
									.ToListAsync();

				return Ok(orders);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to get orders: {ex.Message}");
			}
		}
	}
}

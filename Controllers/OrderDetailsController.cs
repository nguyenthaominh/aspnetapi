using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backendaspnet.Models;
using Microsoft.AspNetCore.Cors;

namespace backendaspnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
	[EnableCors("AllowOrigin")]
	public class OrderDetailsController : ControllerBase
    {
        private readonly BackendContext _context;

        public OrderDetailsController(BackendContext context)
        {
            _context = context;
        }

        // GET: api/OrderDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetail>>> GetOrderDetails()
        {
          if (_context.OrderDetails == null)
          {
              return NotFound();
          }
            return await _context.OrderDetails.ToListAsync();
        }

        // GET: api/OrderDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDetail>> GetOrderDetail(int id)
        {
          if (_context.OrderDetails == null)
          {
              return NotFound();
          }
            var orderDetail = await _context.OrderDetails.FindAsync(id);

            if (orderDetail == null)
            {
                return NotFound();
            }

            return orderDetail;
        }

        // PUT: api/OrderDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderDetail(int id, OrderDetail orderDetail)
        {
            if (id != orderDetail.Id)
            {
                return BadRequest();
            }

            _context.Entry(orderDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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

        // POST: api/OrderDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrderDetail>> PostOrderDetail(OrderDetail orderDetail)
        {
          if (_context.OrderDetails == null)
          {
              return Problem("Entity set 'BackendContext.OrderDetails'  is null.");
          }
            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrderDetail", new { id = orderDetail.Id }, orderDetail);
        }

        // DELETE: api/OrderDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderDetail(int id)
        {
            if (_context.OrderDetails == null)
            {
                return NotFound();
            }
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }
		[HttpGet("byorder/{id}")]
		public async Task<ActionResult<List<OrderDetail>>> GetOrderDetailsByOrder(long id)
		{
			try
			{
				var orderDetailsByOrder = await _context.OrderDetails
					.Where(od => od.OrderId == id)
					.ToListAsync();

				if (orderDetailsByOrder == null || orderDetailsByOrder.Count == 0)
				{
					return NotFound("No order details found for the given order ID");
				}

				return Ok(orderDetailsByOrder);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to get order details: {ex.Message}");
			}
		}

		private bool OrderDetailExists(int id)
        {
            return (_context.OrderDetails?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

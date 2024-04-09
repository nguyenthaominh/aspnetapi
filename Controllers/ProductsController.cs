using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backendaspnet.Models;
using Microsoft.AspNetCore.Cors;
using System.Text.Json.Serialization;
using System.Text.Json;
using Azure;
using System.Linq;


namespace backendaspnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
	[EnableCors("AllowOrigin")]
	public class ProductsController : ControllerBase
    {
        private readonly BackendContext _context;

        public ProductsController(BackendContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
          if (_context.Products == null)
          {
              return NotFound();
          }
            return await _context.Products.ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
          if (_context.Products == null)
          {
              return NotFound();
          }
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
          if (_context.Products == null)
          {
              return Problem("Entity set 'BackendContext.Products'  is null.");
          }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
			product.Thumbnail += ".png";
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

		private readonly string uploadDir = "src/main/resources/static/dataImage"; // Set your desired directory path

		[HttpPost("image")]
		public async Task<IActionResult> UploadImage([FromForm] IFormFile file, [FromForm] string customName)
		{
			try
			{
				// Create the directory if it doesn't exist
				Directory.CreateDirectory(uploadDir);

				// Generate a unique filename for the uploaded image (you may need to modify this logic)
				string fileName = customName;

				// Get the file path for saving the uploaded image
				string filePath = Path.Combine(uploadDir, fileName);

				// Save the uploaded image to the specified directory
				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await file.CopyToAsync(fileStream);
				}

				return Ok("Image uploaded successfully");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Failed to upload image: {ex.Message}");
			}
		}

		[HttpGet("image/{imageName}")]
		public IActionResult GetImage(string imageName)
		{
			try
			{
				// Xác định đường dẫn đầy đủ của tệp hình ảnh
				string imagePath = Path.Combine(uploadDir, imageName);

				// Kiểm tra xem tệp hình ảnh có tồn tại không
				if (!System.IO.File.Exists(imagePath))
				{
					return NotFound("Image not found.");
				}

				// Đọc dữ liệu của hình ảnh thành mảng byte
				byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);

				// Xác định loại của hình ảnh (MediaType)
				string contentType = "image/png"; // Đây là một ví dụ, bạn có thể thay đổi tùy thuộc vào định dạng hình ảnh

				// Trả về hình ảnh kèm theo MediaType phù hợp
				return File(imageBytes, contentType);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Failed to retrieve image: {ex.Message}");
			}
		}

		[HttpGet("getall")]
		public async Task<IActionResult> GetAll(int page = 0, int size = 10, string sortType = "asc")
		{
			try
			{
				// Lấy tất cả sản phẩm
				var allProductsResult = await GetProducts();
				var allProducts = allProductsResult.Value;
				if (sortType == "desc")
				{
					allProducts = allProducts.OrderByDescending(p => p.Price);
				}
				else
				{
					allProducts = allProducts.OrderBy(p => p.Price);
				}

				// Tính tổng số lượng sản phẩm
				int totalCount = allProducts.Count();

				// Tính tổng số trang
				int totalPages = (int)Math.Ceiling((double)totalCount / size);

				// Đảm bảo trang hiện tại không vượt quá tổng số trang
				page = Math.Max(page, 0);

				// Tính chỉ số bắt đầu
				int startIndex = page * size;

				// Lấy số lượng sản phẩm cho trang hiện tại
				var products = allProducts.Skip(startIndex).Take(size);

				// Tạo trang dữ liệu
				var result = new
				{
					TotalCount = totalCount,
					PageSize = size,
					PageIndex = page,
					TotalPages = totalPages,
					Products = products
				};

				// Trả về kết quả
				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Failed to get products: {ex.Message}");
			}
		}

		[HttpGet("category/{categoryId}")]
		public async Task<ActionResult<Page<Product>>> GetProductsByCategoryId(
		[FromServices] BackendContext context,
		long categoryId,
		[FromQuery(Name = "page")] int page = 0,
		[FromQuery(Name = "size")] int size = 10)
		{
			try
			{
				var products = await context.Products
					.Where(p => p.CategoryId == categoryId)
					.OrderBy(p => p.Id)
					.Skip(page * size)
					.Take(size)
					.ToListAsync();

				var totalProducts = await context.Products
					.Where(p => p.CategoryId == categoryId)
					.CountAsync();

				var totalPages = (int)Math.Ceiling((double)totalProducts / size);

				var result = new
				{
					TotalCount = totalProducts,
					PageSize = size,
					PageIndex = page,
					TotalPages = totalPages,
					Items = products
				};

				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Failed to get products: {ex.Message}");
			}
		}


		// DELETE: api/Products/5
		[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }

		// GET: api/Products/search?keyword={keyword}
		[HttpGet("search")]
		public async Task<ActionResult<IEnumerable<Product>>> SearchProducts(string keyword)
		{
			try
			{
				// Tìm kiếm sản phẩm dựa trên từ khóa trong tên hoặc mô tả
				var products = await _context.Products
					.Where(p => EF.Functions.Like(p.Title, $"%{keyword}%") || EF.Functions.Like(p.Description, $"%{keyword}%"))
					.ToListAsync();

				if (products == null || products.Count == 0)
				{
					return NotFound("No products found for the given keyword");
				}

				return Ok(products);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Failed to search products: {ex.Message}");
			}
		}
		[HttpGet("paginate")]
		public async Task<ActionResult<IEnumerable<Product>>> GetPaginatedProducts([FromQuery] int? page = 1, [FromQuery] int? perPage = 10)
		{
			var productsQuery = _context.Products.OrderBy(p => p.Id); // Thay đổi thứ tự nếu cần

			if (page != null && perPage != null)
			{
				var totalCount = await productsQuery.CountAsync();
				var totalPages = (int)Math.Ceiling((double)totalCount / perPage.Value);

				var products = await productsQuery
					.Skip((page.Value - 1) * perPage.Value)
					.Take(perPage.Value)
					.ToListAsync();

				var response = new
				{
					totalCount,
					totalPages,
					currentPage = page.Value,
					perPage = perPage.Value,
					data = products
				};

				return Ok(response);
			}
			else
			{
				var products = await productsQuery.ToListAsync();
				return Ok(products);
			}
		}

	}
}

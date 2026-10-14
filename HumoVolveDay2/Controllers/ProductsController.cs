using HumoVolveDay2.Data;
using HumoVolveDay2.DTOs;
using HumoVolveDay2.Models;
using HumoVolveDay2.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HumoVolveDay2.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController:ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(
            int pageNumber = 1,
            int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest("Invalid pagination parameters.");
            }
            var query = _context.Product.AsNoTracking();

            // Use pagination helper extension to create a paged result (caller applies ordering)
            var paged = await query
                .OrderBy(p => p.Id)
                .ToPagedResultAsync(pageNumber, pageSize, p => new ProductReadDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name
                });

            return Ok(paged);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] DTOs.ProductUpdateDto dto, [FromHeader(Name = "If-Match")] string? ifMatch)
        {
            if (id != dto.Id)
                return BadRequest("Id mismatch.");

            if (string.IsNullOrEmpty(ifMatch))
                return BadRequest("If-Match header with RowVersion (base64) is required for concurrency checks.");

            byte[] rowVersion;
            try
            {
                rowVersion = Convert.FromBase64String(ifMatch);
            }
            catch
            {
                return BadRequest("If-Match header is not a valid base64 RowVersion value.");
            }

            var product = await _context.Product.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            // Update only allowed fields
            product.Name = dto.Name;
            product.Price = dto.Price;
            product.CategoryId = dto.CategoryId;

            // Set original RowVersion so EF can detect concurrency conflicts
            _context.Entry(product).Property(p => p.RowVersion).OriginalValue = rowVersion;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("The record you attempted to edit was modified by another user. Please reload and try again.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteProduct(int id)
        {
            var product = await _context.Product.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            product.IsDeleted = true;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("Concurrency conflict while deleting.");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _context.Product
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new ProductReadDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name
                })
                .FirstOrDefaultAsync();

            if (product == null) return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] DTOs.CreateProductDto dto)
        {
            if (dto == null) return BadRequest();

            // check category exists
            var categoryName = await _context.Category
                .Where(c => c.Id == dto.CategoryId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync();

            if (categoryName == null)
                return BadRequest("Category not found.");

            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                CategoryId = dto.CategoryId
            };

            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            var read = new ProductReadDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                CategoryId = product.CategoryId,
                CategoryName = categoryName
            };

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, read);
        }
    }
}

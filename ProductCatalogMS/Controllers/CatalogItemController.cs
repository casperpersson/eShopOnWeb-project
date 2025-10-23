using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCatalogMS.Data;
using ProductCatalogMS.Entities;

namespace ProductCatalogMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogItemController : ControllerBase
{
    private readonly CatalogContext _context;
    private readonly ILogger<CatalogItemController> _logger;

    public CatalogItemController(CatalogContext context, ILogger<CatalogItemController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CatalogItem>>> GetAll()
    {
        try
        {
            var items = await _context.CatalogItems
                .Include(i => i.CatalogBrand)
                .Include(i => i.CatalogType)
                .ToListAsync();
            return Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving catalog items");
            return StatusCode(500, "An error occurred retrieving catalog items");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CatalogItem>> GetById(int id)
    {
        try
        {
            var item = await _context.CatalogItems
                .Include(i => i.CatalogBrand)
                .Include(i => i.CatalogType)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
                return NotFound($"CatalogItem with ID {id} not found.");

            return Ok(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving catalog item with ID {Id}", id);
            return StatusCode(500, "An error occurred retrieving the catalog item");
        }
    }

    [HttpPost]
    public async Task<ActionResult<CatalogItem>> Create(CreateCatalogItemRequest request)
    {
        try
        {
            // Validate that the brand and type exist
            var brandExists = await _context.CatalogBrands.AnyAsync(b => b.Id == request.CatalogBrandId);
            var typeExists = await _context.CatalogTypes.AnyAsync(t => t.Id == request.CatalogTypeId);

            if (!brandExists)
                return BadRequest($"CatalogBrand with ID {request.CatalogBrandId} does not exist.");

            if (!typeExists)
                return BadRequest($"CatalogType with ID {request.CatalogTypeId} does not exist.");

            var catalogItem = new CatalogItem(
                request.CatalogTypeId,
                request.CatalogBrandId,
                request.Description,
                request.Name,
                request.Price,
                request.PictureUri ?? ""
            );

            _context.CatalogItems.Add(catalogItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = catalogItem.Id }, catalogItem);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating catalog item");
            return StatusCode(500, "An error occurred creating the catalog item");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDetails(int id, UpdateCatalogItemDetailsRequest request)
    {
        try
        {
            var item = await _context.CatalogItems.FindAsync(id);
            if (item == null)
                return NotFound($"CatalogItem with ID {id} not found.");

            item.UpdateDetails(request.Name, request.Description, request.Price);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating catalog item with ID {Id}", id);
            return StatusCode(500, "An error occurred updating the catalog item");
        }
    }

    [HttpPut("{id}/brand")]
    public async Task<IActionResult> UpdateBrand(int id, UpdateBrandRequest request)
    {
        try
        {
            var item = await _context.CatalogItems.FindAsync(id);
            if (item == null)
                return NotFound($"CatalogItem with ID {id} not found.");

            // Validate that the brand exists
            var brandExists = await _context.CatalogBrands.AnyAsync(b => b.Id == request.CatalogBrandId);
            if (!brandExists)
                return BadRequest($"CatalogBrand with ID {request.CatalogBrandId} does not exist.");

            item.UpdateBrand(request.CatalogBrandId);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating catalog item brand with ID {Id}", id);
            return StatusCode(500, "An error occurred updating the catalog item brand");
        }
    }

    [HttpPut("{id}/type")]
    public async Task<IActionResult> UpdateType(int id, UpdateTypeRequest request)
    {
        try
        {
            var item = await _context.CatalogItems.FindAsync(id);
            if (item == null)
                return NotFound($"CatalogItem with ID {id} not found.");

            // Validate that the type exists
            var typeExists = await _context.CatalogTypes.AnyAsync(t => t.Id == request.CatalogTypeId);
            if (!typeExists)
                return BadRequest($"CatalogType with ID {request.CatalogTypeId} does not exist.");

            item.UpdateType(request.CatalogTypeId);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating catalog item type with ID {Id}", id);
            return StatusCode(500, "An error occurred updating the catalog item type");
        }
    }

    [HttpPut("{id}/picture")]
    public async Task<IActionResult> UpdatePicture(int id, UpdatePictureRequest request)
    {
        try
        {
            var item = await _context.CatalogItems.FindAsync(id);
            if (item == null)
                return NotFound($"CatalogItem with ID {id} not found.");

            item.UpdatePictureUri(request.PictureName);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating catalog item picture with ID {Id}", id);
            return StatusCode(500, "An error occurred updating the catalog item picture");
        }
    }

    [HttpPut("{id}/details")]
    public async Task<IActionResult> UpdateCatalogItemDetails(int id, UpdateCatalogItemDetailsRequest request)
    {
        try
        {
            var item = await _context.CatalogItems.FindAsync(id);
            if (item == null)
                return NotFound($"CatalogItem with ID {id} not found.");

            item.UpdateDetails(request.Name, request.Description, request.Price);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating catalog item details with ID {Id}", id);
            return StatusCode(500, "An error occurred updating the catalog item details");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var item = await _context.CatalogItems.FindAsync(id);
            if (item == null)
                return NotFound($"CatalogItem with ID {id} not found.");

            _context.CatalogItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting catalog item with ID {Id}", id);
            return StatusCode(500, "An error occurred deleting the catalog item");
        }
    }
}

// Request DTOs
public record CreateCatalogItemRequest(
    int CatalogTypeId,
    int CatalogBrandId,
    string Description,
    string Name,
    decimal Price,
    string? PictureUri = null
);

public record UpdateCatalogItemDetailsRequest(
    string Name,
    string Description,
    decimal Price
);

public record UpdateBrandRequest(int CatalogBrandId);
public record UpdateTypeRequest(int CatalogTypeId);
public record UpdatePictureRequest(string PictureName);

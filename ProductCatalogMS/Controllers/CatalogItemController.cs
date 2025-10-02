using Microsoft.AspNetCore.Mvc;
using ProductCatalogMS.Entities;

namespace ProductCatalogMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogItemController : ControllerBase
{
    // In-memory storage for demo - replace with repository/DbContext in production
    private static readonly List<CatalogItem> _catalogItems = new();
    private static int _nextId = 6; // Start from 6 since we'll add 5 items

    static CatalogItemController()
    {
        // Initialize with sample data
        InitializeSampleData();
    }

    private static void InitializeSampleData()
    {
        var sampleItems = new[]
        {
            new CatalogItem(2, 1, ".NET Bot Black Sweatshirt", ".NET Bot Black Sweatshirt", 19.5M, "images/products/1.png"),
            new CatalogItem(1, 1, ".NET Black & White Mug", ".NET Black & White Mug", 8.50M, "images/products/2.png"),
            new CatalogItem(2, 2, "Prism White T-Shirt", "Prism White T-Shirt", 12.0M, "images/products/3.png"),
            new CatalogItem(2, 1, ".NET Foundation Sweatshirt", ".NET Foundation Sweatshirt", 12.0M, "images/products/4.png"),
            new CatalogItem(1, 2, "Roslyn Red Sheet", "Roslyn Red Sheet", 8.5M, "images/products/5.png")
        };

        for (int i = 0; i < sampleItems.Length; i++)
        {
            SetId(sampleItems[i], i + 1);
            _catalogItems.Add(sampleItems[i]);
        }
    }

    [HttpGet]
    public ActionResult<IEnumerable<CatalogItem>> GetAll()
    {
        return Ok(_catalogItems);
    }

    [HttpGet("{id}")]
    public ActionResult<CatalogItem> GetById(int id)
    {
        var item = _catalogItems.FirstOrDefault(x => x.Id == id);
        if (item == null)
            return NotFound($"CatalogItem with ID {id} not found.");

        return Ok(item);
    }

    [HttpPost]
    public ActionResult<CatalogItem> Create(CreateCatalogItemRequest request)
    {
        try
        {
            var catalogItem = new CatalogItem(
                request.CatalogTypeId,
                request.CatalogBrandId,
                request.Description,
                request.Name,
                request.Price,
                request.PictureUri ?? ""
            );

            // Set ID using reflection (in real app, this would be handled by database)
            SetId(catalogItem, _nextId++);
            _catalogItems.Add(catalogItem);

            return CreatedAtAction(nameof(GetById), new { id = catalogItem.Id }, catalogItem);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public IActionResult UpdateDetails(int id, UpdateCatalogItemDetailsRequest request)
    {
        var item = _catalogItems.FirstOrDefault(x => x.Id == id);
        if (item == null)
            return NotFound($"CatalogItem with ID {id} not found.");

        try
        {
            item.UpdateDetails(request.Name, request.Description, request.Price);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/brand")]
    public IActionResult UpdateBrand(int id, UpdateBrandRequest request)
    {
        var item = _catalogItems.FirstOrDefault(x => x.Id == id);
        if (item == null)
            return NotFound($"CatalogItem with ID {id} not found.");

        try
        {
            item.UpdateBrand(request.CatalogBrandId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/type")]
    public IActionResult UpdateType(int id, UpdateTypeRequest request)
    {
        var item = _catalogItems.FirstOrDefault(x => x.Id == id);
        if (item == null)
            return NotFound($"CatalogItem with ID {id} not found.");

        try
        {
            item.UpdateType(request.CatalogTypeId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/picture")]
    public IActionResult UpdatePicture(int id, UpdatePictureRequest request)
    {
        var item = _catalogItems.FirstOrDefault(x => x.Id == id);
        if (item == null)
            return NotFound($"CatalogItem with ID {id} not found.");

        item.UpdatePictureUri(request.PictureName);
        return NoContent();
    }

    [HttpPut("{id}/details")]
    public IActionResult UpdateCatalogItemDetails(int id, UpdateCatalogItemDetailsRequest request)
    {
        var item = _catalogItems.FirstOrDefault(x => x.Id == id);
        if (item == null)
            return NotFound($"CatalogItem with ID {id} not found.");

        try
        {
            item.UpdateDetails(request.Name, request.Description, request.Price);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var item = _catalogItems.FirstOrDefault(x => x.Id == id);
        if (item == null)
            return NotFound($"CatalogItem with ID {id} not found.");

        _catalogItems.Remove(item);
        return NoContent();
    }

    private static void SetId(BaseEntity entity, int id)
    {
        var property = typeof(BaseEntity).GetProperty("Id");
        property?.SetValue(entity, id);
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



using Microsoft.AspNetCore.Mvc;
using ProductCatalogMS.Entities;

namespace ProductCatalogMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogBrandController : ControllerBase
{
    private static readonly List<CatalogBrand> _brands = new();

    static CatalogBrandController()
    {
        // Initialize with sample data
        var brand1 = new CatalogBrand("Azure");
        SetId(brand1, 1);
        _brands.Add(brand1);

        var brand2 = new CatalogBrand(".NET");
        SetId(brand2, 2);
        _brands.Add(brand2);
    }

    [HttpGet]
    public ActionResult<IEnumerable<CatalogBrand>> GetAll()
    {
        return Ok(_brands);
    }

    [HttpGet("{id}")]
    public ActionResult<CatalogBrand> GetById(int id)
    {
        var brand = _brands.FirstOrDefault(x => x.Id == id);
        if (brand == null)
            return NotFound();
        return Ok(brand);
    }

    private static void SetId(BaseEntity entity, int id)
    {
        var property = typeof(BaseEntity).GetProperty("Id");
        property?.SetValue(entity, id);
    }
}



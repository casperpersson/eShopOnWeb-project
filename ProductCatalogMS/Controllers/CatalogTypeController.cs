using Microsoft.AspNetCore.Mvc;
using ProductCatalogMS.Entities;

namespace ProductCatalogMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogTypeController : ControllerBase
{
    private static readonly List<CatalogType> _types = new();

    static CatalogTypeController()
    {
        // Initialize with sample data
        var type1 = new CatalogType("T-Shirt");
        SetId(type1, 1);
        _types.Add(type1);

        var type2 = new CatalogType("Mug");
        SetId(type2, 2);
        _types.Add(type2);
    }

    [HttpGet]
    public ActionResult<IEnumerable<CatalogType>> GetAll()
    {
        return Ok(_types);
    }

    [HttpGet("{id}")]
    public ActionResult<CatalogType> GetById(int id)
    {
        var type = _types.FirstOrDefault(x => x.Id == id);
        if (type == null)
            return NotFound();
        return Ok(type);
    }

    private static void SetId(BaseEntity entity, int id)
    {
        var property = typeof(BaseEntity).GetProperty("Id");
        property?.SetValue(entity, id);
    }
}



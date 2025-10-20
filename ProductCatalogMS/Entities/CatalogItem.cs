namespace ProductCatalogMS.Entities;

public class CatalogItem : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public string PictureUri { get; private set; }
    public int CatalogTypeId { get; private set; }
    public int CatalogBrandId { get; private set; }

    public CatalogItem(int catalogTypeId,
        int catalogBrandId,
        string description,
        string name,
        decimal price,
        string pictureUri)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null or empty", nameof(description));
        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero", nameof(price));
        if (catalogTypeId <= 0)
            throw new ArgumentException("CatalogTypeId must be greater than zero", nameof(catalogTypeId));
        if (catalogBrandId <= 0)
            throw new ArgumentException("CatalogBrandId must be greater than zero", nameof(catalogBrandId));

        CatalogTypeId = catalogTypeId;
        CatalogBrandId = catalogBrandId;
        Description = description;
        Name = name;
        Price = price;
        PictureUri = pictureUri ?? string.Empty;
    }

    public void UpdateDetails(string name, string description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null or empty", nameof(description));
        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero", nameof(price));

        Name = name;
        Description = description;
        Price = price;
    }

    public void UpdateBrand(int catalogBrandId)
    {
        if (catalogBrandId <= 0)
            throw new ArgumentException("CatalogBrandId must be greater than zero", nameof(catalogBrandId));
        CatalogBrandId = catalogBrandId;
    }

    public void UpdateType(int catalogTypeId)
    {
        if (catalogTypeId <= 0)
            throw new ArgumentException("CatalogTypeId must be greater than zero", nameof(catalogTypeId));
        CatalogTypeId = catalogTypeId;
    }

    public void UpdatePictureUri(string pictureName)
    {
        if (string.IsNullOrEmpty(pictureName))
        {
            PictureUri = string.Empty;
            return;
        }
        PictureUri = $"images/products/{pictureName}?{DateTime.UtcNow.Ticks}";
    }
    public string Brand { get; private set; }
    public virtual CatalogBrand CatalogBrand { get; set; }
    public virtual CatalogType CatalogType { get; set; }
}


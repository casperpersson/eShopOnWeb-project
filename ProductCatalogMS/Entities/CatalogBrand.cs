namespace ProductCatalogMS.Entities;

public class CatalogBrand : BaseEntity
{
    public string Brand { get; private set; }

    public CatalogBrand(string brand)
    {
        if (string.IsNullOrWhiteSpace(brand))
            throw new ArgumentException("Brand cannot be null or empty", nameof(brand));

        Brand = brand;
    }
}


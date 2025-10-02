namespace ProductCatalogMS.Entities;

public class CatalogType : BaseEntity
{
    public string Type { get; private set; }

    public CatalogType(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Type cannot be null or empty", nameof(type));

        Type = type;
    }
}


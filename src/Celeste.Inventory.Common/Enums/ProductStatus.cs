namespace Celeste.Inventory.Common.Enums;

/// <summary>
///     Represents lifecycle states for products.
/// </summary>
public enum ProductStatus
{
    /// <summary>
    ///     Indicates the product is available.
    /// </summary>
    Active = 1,

    /// <summary>
    ///     Indicates the product is inactive.
    /// </summary>
    Inactive = 2,

    /// <summary>
    ///     Indicates the product is discontinued.
    /// </summary>
    Discontinued = 3,
}

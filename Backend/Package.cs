
namespace Backend;

/// <summary>
/// Represents a package with its pickup and delivery locations.
/// </summary>
public readonly record struct Package(int Id, string Pickup, string Delivery);

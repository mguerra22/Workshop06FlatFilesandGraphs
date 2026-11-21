// Backend/Edge.cs
namespace Backend;

/// <summary>A connection between two vertices. Weight is 1 for unweighted graphs.</summary>
public readonly record struct Edge<T>(T From, T To, double Weight = 1)
{
    public override string ToString() => Weight == 1 ? $"{From} -> {To}" : $"{From} -> {To} ({Weight})";
}

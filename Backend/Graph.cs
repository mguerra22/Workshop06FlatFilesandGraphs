// Backend/Graph.cs
namespace Backend;

/// <summary>
/// Graph stored as an adjacency list: every vertex maps to the list of edges that leave it.
/// Works for directed and undirected, weighted and unweighted graphs.
/// </summary>
public class Graph<T> where T : notnull
{
    private readonly Dictionary<T, List<Edge<T>>> _adjacency = new();

    public bool IsDirected { get; }
    public int EdgeCount { get; private set; }

    public Graph(bool directed = false)
    {
        IsDirected = directed;
    }

    public IEnumerable<T> Vertices => _adjacency.Keys;
    public int VertexCount => _adjacency.Count;

    public bool ContainsVertex(T vertex) => _adjacency.ContainsKey(vertex);

    /// <summary>Adds the vertex if it is not there yet. Returns true if it was added.</summary>
    public bool AddVertex(T vertex)
    {
        if (_adjacency.ContainsKey(vertex)) return false;
        _adjacency[vertex] = new List<Edge<T>>();
        return true;
    }

    /// <summary>Adds an edge (and its vertices if needed). In an undirected graph the edge is stored in both directions.</summary>
    public void AddEdge(T from, T to, double weight = 1)
    {
        AddVertex(from);
        AddVertex(to);

        if (HasEdge(from, to)) return; // No parallel edges.

        _adjacency[from].Add(new Edge<T>(from, to, weight));
        if (!IsDirected && !from.Equals(to))
        {
            _adjacency[to].Add(new Edge<T>(to, from, weight));
        }
        EdgeCount++;
    }

    public bool HasEdge(T from, T to)
    {
        return _adjacency.TryGetValue(from, out var edges) && edges.Any(e => e.To.Equals(to));
    }

    /// <summary>Edges leaving the vertex. Throws if the vertex does not exist.</summary>
    public IReadOnlyList<Edge<T>> EdgesFrom(T vertex)
    {
        if (!_adjacency.TryGetValue(vertex, out var edges))
        {
            throw new KeyNotFoundException($"Vertex '{vertex}' is not in the graph.");
        }
        return edges;
    }

    /// <summary>The vertices reachable in one step from the given vertex.</summary>
    public IEnumerable<T> Neighbors(T vertex) => EdgesFrom(vertex).Select(e => e.To);

    /// <summary>Number of edges leaving the vertex (out-degree in a directed graph).</summary>
    public int Degree(T vertex) => EdgesFrom(vertex).Count;

    /// <summary>Number of edges arriving at the vertex. O(V + E): we have to scan every list.</summary>
    public int InDegree(T vertex)
    {
        return _adjacency.Values.Sum(edges => edges.Count(e => e.To.Equals(vertex)));
    }

    public bool RemoveEdge(T from, T to)
    {
        if (!HasEdge(from, to)) return false;

        _adjacency[from].RemoveAll(e => e.To.Equals(to));
        if (!IsDirected) _adjacency[to].RemoveAll(e => e.To.Equals(from));
        EdgeCount--;
        return true;
    }

    /// <summary>Removes the vertex and every edge that touches it.</summary>
    public bool RemoveVertex(T vertex)
    {
        if (!_adjacency.Remove(vertex, out var outgoing)) return false;

        EdgeCount -= outgoing.Count;
        foreach (var edges in _adjacency.Values)
        {
            int removed = edges.RemoveAll(e => e.To.Equals(vertex));
            if (IsDirected) EdgeCount -= removed; // Undirected ones were already counted above.
        }
        return true;
    }

    /// <summary>Every edge once. In an undirected graph each pair is reported a single time.</summary>
    public IEnumerable<Edge<T>> Edges()
    {
        var reported = new HashSet<(T, T)>();

        foreach (var edges in _adjacency.Values)
        {
            foreach (var edge in edges)
            {
                if (!IsDirected && reported.Contains((edge.To, edge.From))) continue;
                reported.Add((edge.From, edge.To));
                yield return edge;
            }
        }
    }

    /// <summary>Prints one line per vertex with its adjacency list.</summary>
    public void Print()
    {
        Console.WriteLine($"{(IsDirected ? "Directed" : "Undirected")} graph: {VertexCount} vertices, {EdgeCount} edges");
        foreach (var (vertex, edges) in _adjacency)
        {
            string list = edges.Count == 0
                ? "(none)"
                : string.Join(", ", edges.Select(e => e.Weight == 1 ? $"{e.To}" : $"{e.To}({e.Weight})"));
            Console.WriteLine($"  {vertex} -> {list}");
        }
    }
}

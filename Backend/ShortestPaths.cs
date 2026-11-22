
namespace Backend;

/// <summary>
/// Calculates shortest paths in a weighted graph using Dijkstra's algorithm.
/// </summary>
public class ShortestPaths<T> where T : notnull
{
    private readonly Graph<T> _graph;
    private readonly Dictionary<T, double> _distances;
    private readonly Dictionary<T, T?> _previous;

    /// <summary>
    /// Initializes the shortest paths calculator.
    /// </summary>
    /// <param name="graph">The graph used to calculate shortest paths.</param>
    public ShortestPaths(Graph<T> graph)
    {
        _graph = graph;
        _distances = new Dictionary<T, double>();
        _previous = new Dictionary<T, T?>();
    }

    /// <summary>
    /// Calculates the shortest distances from the starting vertex using Dijkstra's algorithm.
    /// </summary>
    /// <param name="start">The starting vertex.</param>
    public void Calculate(T start)
    {
        if (!_graph.ContainsVertex(start))
        {
            throw new ArgumentException("Starting vertex does not exist in the graph.");
        }

        _distances.Clear();
        _previous.Clear();

        foreach (T vertex in _graph.Vertices)
        {
            _distances[vertex] = double.PositiveInfinity;
            _previous[vertex] = default;
        }

        var priorityQueue = new PriorityQueue<T, double>();

        _distances[start] = 0;
        priorityQueue.Enqueue(start, 0);

        while (priorityQueue.Count > 0)
        {
            T current = priorityQueue.Dequeue();
            double currentDistance = _distances[current];

            foreach (Edge<T> edge in _graph.EdgesFrom(current))
            {
                double newDistance = currentDistance + edge.Weight;

                if (newDistance < _distances[edge.To])
                {
                    _distances[edge.To] = newDistance;
                    _previous[edge.To] = current;

                    priorityQueue.Enqueue(edge.To, newDistance);
                }
            }
        }
    }

    /// <summary>
    /// Gets the shortest distance to the specified destination.
    /// </summary>
    /// <param name="destination">The destination vertex.</param>
    /// <returns>The shortest distance, or positive infinity if unreachable.</returns>
    public double GetDistance(T destination)
    {
        if (!_distances.TryGetValue(destination, out double distance))
        {
            throw new ArgumentException("Destination does not exist in the graph.");
        }

        return distance;
    }

    /// <summary>
    /// Reconstructs the shortest path to the specified destination.
    /// </summary>
    /// <param name="destination">The destination vertex.</param>
    /// <returns>The shortest path as a list of vertices.</returns>
    public List<T> GetPath(T destination)
    {
        if (!_distances.ContainsKey(destination))
        {
            throw new ArgumentException("Destination does not exist in the graph.");
        }

        if (double.IsPositiveInfinity(_distances[destination]))
        {
            return new List<T>();
        }

        var path = new List<T>();
        T? current = destination;

        while (current is not null)
        {
            path.Add(current);

            if (!_previous.TryGetValue(current, out T? previous))
            {
                break;
            }

            current = previous;
        }

        path.Reverse();

        return path;
    }


}

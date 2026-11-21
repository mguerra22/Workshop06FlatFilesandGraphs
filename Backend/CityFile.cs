
namespace Backend;

/// <summary>
/// Loads the city map from a text file and creates a directed weighted graph.
/// </summary>
public class CityFile
{
    public int StreetCount { get; private set; }
    public int OneWayStreetCount { get; private set; }

    /// <summary>Loads the city map from the specified file.</summary>
    public Graph<string> Load(string path)
    {
        var graph = new Graph<string>(directed: true);
        var streets = new HashSet<(string, string)>();

        StreetCount = 0;
        OneWayStreetCount = 0;

        string[] lines = File.ReadAllLines(path);

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
            {
                continue;
            }

            string[] fields = line.Split(',');

            if (fields.Length != 4)
            {
                throw new FormatException(
                    $"Invalid format at line {i + 1}.");
            }

            string from = fields[0].Trim();
            string to = fields[1].Trim();
            string direction = fields[3].Trim();

            if (string.IsNullOrWhiteSpace(from) ||
                string.IsNullOrWhiteSpace(to))
            {
                throw new FormatException(
                    $"Invalid location at line {i + 1}.");
            }

            if (!int.TryParse(fields[2].Trim(), out int distance) ||
                distance <= 0)
            {
                throw new FormatException(
                    $"Invalid distance at line {i + 1}.");
            }

            if (!direction.Equals("TwoWay", StringComparison.OrdinalIgnoreCase) &&
                !direction.Equals("OneWay", StringComparison.OrdinalIgnoreCase))
            {
                throw new FormatException(
                    $"Invalid direction at line {i + 1}.");
            }

            var street = string.CompareOrdinal(from, to) < 0
                ? (from, to)
                : (to, from);

            if (!streets.Add(street))
            {
                throw new FormatException(
                    $"Duplicate street at line {i + 1}.");
            }

            graph.AddVertex(from);
            graph.AddVertex(to);

            graph.AddEdge(from, to, distance);

            if (direction.Equals("TwoWay", StringComparison.OrdinalIgnoreCase))
            {
                graph.AddEdge(to, from, distance);
            }
            else
            {
                OneWayStreetCount++;
            }

            StreetCount++;
        }

        return graph;
    }
}

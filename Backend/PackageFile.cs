
namespace Backend;

/// <summary>
/// Loads and validates package pickup and delivery tasks from a text file.
/// </summary>
public class PackageFile
{
    /// <summary>
    /// Loads packages from the specified file and validates their locations.
    /// </summary>
    /// <param name="path">The path of the package file.</param>
    /// <param name="graph">The city graph used to validate locations.</param>
    /// <returns>The list of validated packages.</returns>

    public List<Package> Load(string path, Graph<string> graph)
    {
        var packages = new Dictionary<int, (string? Pickup, string? Delivery)>();

        string[] lines = File.ReadAllLines(path);

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
            {
                continue;
            }

            string[] fields = line.Split(',');

            if (fields.Length != 3)
            {
                throw new FormatException($"Invalid format at line {i + 1}.");
            }

            string location = fields[0].Trim();
            string action = fields[1].Trim();

            if (string.IsNullOrWhiteSpace(location))
            {
                throw new FormatException($"Invalid location at line {i + 1}.");
            }

            if (!graph.ContainsVertex(location))
            {
                throw new FormatException(
                    $"Unknown location '{location}' at line {i + 1}.");
            }

            if (!int.TryParse(fields[2].Trim(), out int packageId) ||
                packageId <= 0)
            {
                throw new FormatException(
                    $"Invalid package ID at line {i + 1}.");
            }

            if (!action.Equals("Pickup", StringComparison.OrdinalIgnoreCase) &&
                !action.Equals("Delivery", StringComparison.OrdinalIgnoreCase))
            {
                throw new FormatException(
                    $"Invalid action at line {i + 1}.");
            }

            if (!packages.TryGetValue(packageId, out var tasks))
            {
                tasks = (null, null);
            }

            if (action.Equals("Pickup", StringComparison.OrdinalIgnoreCase))
            {
                if (tasks.Pickup is not null)
                {
                    throw new FormatException(
                        $"Duplicate pickup for package {packageId} at line {i + 1}.");
                }

                tasks.Pickup = location;
            }
            else
            {
                if (tasks.Delivery is not null)
                {
                    throw new FormatException(
                        $"Duplicate delivery for package {packageId} at line {i + 1}.");
                }

                tasks.Delivery = location;
            }

            packages[packageId] = tasks;
        }

        var result = new List<Package>();

        foreach (var item in packages.OrderBy(x => x.Key))
        {
            if (item.Value.Pickup is null)
            {
                throw new FormatException(
                    $"Package {item.Key} is missing its pickup.");
            }

            if (item.Value.Delivery is null)
            {
                throw new FormatException(
                    $"Package {item.Key} is missing its delivery.");
            }

            result.Add(new Package(
                item.Key,
                item.Value.Pickup,
                item.Value.Delivery));
        }

        return result;
    }

}

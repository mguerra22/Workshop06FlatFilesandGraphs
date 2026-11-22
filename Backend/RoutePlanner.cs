namespace Backend;

public class RoutePlanner
{
    private readonly Graph<string> _city;
    private readonly IReadOnlyList<Package> _packages;
    private readonly string _start;

    private readonly Dictionary<(string From, string To), double> _distances = new();
    private readonly Dictionary<(string From, string To), List<string>> _paths = new();

    public RoutePlanner(Graph<string> city, IReadOnlyList<Package> packages, string start)
    {
        _city = city;
        _packages = packages;
        _start = start;

        if (!_city.ContainsVertex(_start))
            throw new ArgumentException("Starting location does not exist.");

        var locations = new List<string> { _start };

        foreach (Package package in _packages)
        {
            locations.Add(package.Pickup);
            locations.Add(package.Delivery);
        }

        locations = locations.Distinct(StringComparer.Ordinal).ToList();

        var shortestPaths = new ShortestPaths<string>(_city);

        foreach (string origin in locations)
        {
            shortestPaths.Calculate(origin);

            foreach (string destination in locations)
            {
                double distance = shortestPaths.GetDistance(destination);

                if (double.IsPositiveInfinity(distance))
                    throw new InvalidOperationException(
                        $"Location {destination} cannot be reached from {origin}.");

                _distances[(origin, destination)] = distance;
                _paths[(origin, destination)] = shortestPaths.GetPath(destination);
            }
        }
    }

    public double DistanceBetween(string from, string to)
    {
        return _distances[(from, to)];
    }

    public List<string> PathBetween(string from, string to)
    {
        return new List<string>(_paths[(from, to)]);
    }
    public List<Stop> Plan()
    {
        var route = new List<Stop>();
        var pickedPackages = new HashSet<int>();
        var deliveredPackages = new HashSet<int>();

        string currentLocation = _start;

        while (deliveredPackages.Count < _packages.Count)
        {
            var availableStops = new List<Stop>();

            foreach (Package package in _packages)
            {
                if (!pickedPackages.Contains(package.Id))
                {
                    availableStops.Add(
                        new Stop(package.Pickup, StopKind.Pickup, package.Id));
                }
                else if (!deliveredPackages.Contains(package.Id))
                {
                    availableStops.Add(
                        new Stop(package.Delivery, StopKind.Delivery, package.Id));
                }
            }

            if (availableStops.Count == 0)
                throw new InvalidOperationException("No valid stops available.");

            Stop nextStop = availableStops
                .OrderBy(stop => DistanceBetween(currentLocation, stop.Location))
                .First();

            route.Add(nextStop);

            if (nextStop.Kind == StopKind.Pickup)
                pickedPackages.Add(nextStop.PackageId);
            else
                deliveredPackages.Add(nextStop.PackageId);

            currentLocation = nextStop.Location;
        }

        return route;
    }
    public bool IsValid(List<Stop> route)
    {
        if (route.Count != _packages.Count * 2)
            return false;

        var packageDictionary = _packages.ToDictionary(p => p.Id);
        var pickedPackages = new HashSet<int>();
        var deliveredPackages = new HashSet<int>();

        foreach (Stop stop in route)
        {
            if (!packageDictionary.TryGetValue(stop.PackageId, out Package package))
                return false;

            if (stop.Kind == StopKind.Pickup)
            {
                if (pickedPackages.Contains(stop.PackageId))
                    return false;

                if (stop.Location != package.Pickup)
                    return false;

                pickedPackages.Add(stop.PackageId);
            }
            else if (stop.Kind == StopKind.Delivery)
            {
                if (!pickedPackages.Contains(stop.PackageId))
                    return false;

                if (deliveredPackages.Contains(stop.PackageId))
                    return false;

                if (stop.Location != package.Delivery)
                    return false;

                deliveredPackages.Add(stop.PackageId);
            }
            else
            {
                return false;
            }
        }

        return pickedPackages.Count == _packages.Count &&
               deliveredPackages.Count == _packages.Count;
    }
    public double TotalDistance(List<Stop> route)
    {
        double totalDistance = 0;
        string currentLocation = _start;

        foreach (Stop stop in route)
        {
            totalDistance += DistanceBetween(currentLocation, stop.Location);
            currentLocation = stop.Location;
        }

        return totalDistance;
    }
}
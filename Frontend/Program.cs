
using Backend;

try
{
    string cityFileName = args.Length > 0 ? args[0] : "city.txt";
    string packagesFileName = args.Length > 1 ? args[1] : "packages.txt";
    string startLocation = args.Length > 2 ? args[2] : "D";

    string cityPath = Path.Combine(
        AppContext.BaseDirectory, cityFileName);

    var cityFile = new CityFile();
    var city = cityFile.Load(cityPath);

    string packagesPath = Path.Combine(
        AppContext.BaseDirectory, packagesFileName);

    var packageFile = new PackageFile();
    List<Package> packages = packageFile.Load(packagesPath, city);


    Console.WriteLine("CITY MAP LOADED SUCCESSFULLY");
    Console.WriteLine("-----------------------------");
    Console.WriteLine($"Locations: {city.VertexCount}");
    Console.WriteLine($"Streets: {cityFile.StreetCount}");
    Console.WriteLine($"One-way streets: {cityFile.OneWayStreetCount}");

    Console.WriteLine();
    Console.WriteLine("PACKAGES LOADED SUCCESSFULLY");
    Console.WriteLine("----------------------------");
    Console.WriteLine($"Packages: {packages.Count}");
    Console.WriteLine($"Tasks: {packages.Count * 2}");


    var planner = new RoutePlanner(city, packages, startLocation);

    List<Stop> nearestRoute = planner.Plan();
    List<Stop> insertionRoute = planner.PlanByInsertion();
    List<Stop> relocationRoute = planner.ImproveByRelocation(insertionRoute);
    List<Stop> improvedRoute = planner.ImproveByMovingStops(relocationRoute);
    List<Stop> randomRoute = planner.PlanWithRandomRestarts(20, 42);


    double nearestDistance = planner.TotalDistance(nearestRoute);
    double insertionDistance = planner.TotalDistance(insertionRoute);
    double relocationDistance = planner.TotalDistance(relocationRoute);
    double improvedDistance = planner.TotalDistance(improvedRoute);
    double randomDistance = planner.TotalDistance(randomRoute);

    Console.WriteLine();
    Console.WriteLine("ROUTE COMPARISON");
    Console.WriteLine("----------------");
    Console.WriteLine($"Nearest neighbor: {nearestDistance / 1000.0:F2} km");
    Console.WriteLine($"Insertion: {insertionDistance / 1000.0:F2} km");
    Console.WriteLine($"Relocation: {relocationDistance / 1000.0:F2} km");
    Console.WriteLine($"Moving stops: {improvedDistance / 1000.0:F2} km");
    Console.WriteLine($"Moving stops valid: {planner.IsValid(improvedRoute)}");
    Console.WriteLine($"Random restarts: {randomDistance / 1000.0:F2} km");
    Console.WriteLine($"Random restarts valid: {planner.IsValid(randomRoute)}");

    if (!planner.IsValid(randomRoute))
        throw new InvalidOperationException("The random route is invalid.");

    RoutePrinter.Print(planner, randomRoute, startLocation);



    if (city.ContainsVertex("H"))
    {
        var shortestPaths = new ShortestPaths<string>(city);

        shortestPaths.Calculate("D");

        double distance = shortestPaths.GetDistance("H");
        List<string> path = shortestPaths.GetPath("H");

        Console.WriteLine();
        Console.WriteLine("SHORTEST PATH TEST");
        Console.WriteLine("------------------");
        Console.WriteLine("Origin: D");
        Console.WriteLine("Destination: H");
        Console.WriteLine($"Path: {string.Join(" -> ", path)}");
        Console.WriteLine($"Distance: {distance} meters");
        Console.WriteLine($"Distance: {distance / 1000.0:F3} km");
    }


}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
Console.WriteLine();
Console.WriteLine("Presiona cualquier tecla para salir...");
Console.ReadKey();
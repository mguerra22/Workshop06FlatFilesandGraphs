
using Backend;

try
{
    string cityPath = Path.Combine(AppContext.BaseDirectory, "city.txt");

    var cityFile = new CityFile();
    var city = cityFile.Load(cityPath);

    Console.WriteLine("CITY MAP LOADED SUCCESSFULLY");
    Console.WriteLine("-----------------------------");
    Console.WriteLine($"Locations: {city.VertexCount}");
    Console.WriteLine($"Streets: {cityFile.StreetCount}");
    Console.WriteLine($"One-way streets: {cityFile.OneWayStreetCount}");


    var shortestPaths = new ShortestPaths<string>(city);

    shortestPaths.Calculate("D");

    double distance = shortestPaths.GetDistance("H");
    List<string> path = shortestPaths.GetPath("H");

    Console.WriteLine();
    Console.WriteLine("SHORTEST PATH TEST");
    Console.WriteLine("------------------");
    Console.WriteLine($"Origin: D");
    Console.WriteLine($"Destination: H");
    Console.WriteLine($"Path: {string.Join(" -> ", path)}");
    Console.WriteLine($"Distance: {distance} meters");
    Console.WriteLine($"Distance: {distance / 1000.0:F3} km");

}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
Console.WriteLine();
Console.WriteLine("Presiona cualquier tecla para salir...");
Console.ReadKey();
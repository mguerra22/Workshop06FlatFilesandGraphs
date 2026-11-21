
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
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

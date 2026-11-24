namespace Backend;

public static class RoutePrinter
{
    public static void Print(
        RoutePlanner planner,
        List<Stop> route,
        string start)
    {
        string currentLocation = start;
        double totalDistance = 0;
        int stopNumber = 1;
        int index = 0;

        Console.WriteLine();
        Console.WriteLine("DELIVERY ROUTE");
        Console.WriteLine("--------------");

        while (index < route.Count)
        {
            string location = route[index].Location;
            var tasks = new List<Stop>();

            while (index < route.Count &&
                   route[index].Location == location)
            {
                tasks.Add(route[index]);
                index++;
            }

            double distance = planner.DistanceBetween(
                currentLocation, location);

            totalDistance += distance;

            string actions = string.Join(" and ",
                tasks.OrderBy(t => t.Kind == StopKind.Delivery ? 0 : 1)
                     .Select(t =>
                         $"{(t.Kind == StopKind.Delivery ? "entregar" : "recoger")} paquete {t.PackageId}"));

            if (currentLocation == location)
            {
                Console.WriteLine(
                    $"{stopNumber}. En {location}: {actions}.");
            }
            else
            {
                Console.WriteLine(
                    $"{stopNumber}. Ir de {currentLocation} a {location} ({distance:F0} m): {actions}.");

                List<string> path = planner.PathBetween(
                    currentLocation, location);

                Console.WriteLine(
                    $"     Camino: {string.Join(" -> ", path)}");
            }

            currentLocation = location;
            stopNumber++;
        }

        Console.WriteLine();
        Console.WriteLine("Todos los paquetes fueron recogidos y entregados.");
        Console.WriteLine($"Recorrido total: {totalDistance / 1000.0:F2} km");
    }
}
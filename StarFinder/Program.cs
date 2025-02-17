using System.Globalization;

if (args.Length != 5)
{
    Console.WriteLine("Usage: dotnet run <filePath> <xCoord> <yCoord> <zCoord> <starsToReturn>");
    return;
}

ProcessStarFinder(args);

static void ProcessStarFinder(string[] args)
{
    var filePath = args[0];
    if (!double.TryParse(args[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var xCoord) ||
        !double.TryParse(args[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var yCoord) ||
        !double.TryParse(args[3], NumberStyles.Float, CultureInfo.InvariantCulture, out var zCoord) ||
        !int.TryParse(args[4], out var starsToReturn))
    {
        Console.WriteLine("Invalid input. Ensure coordinates are numbers and starsToReturn is an integer.");
        return;
    }

    var closestStars = StarFinder.FindClosestStars(filePath, xCoord, yCoord, zCoord, starsToReturn);
    foreach (var star in closestStars)
    {
        Console.WriteLine($"{star.StarName}: ({star.X}, {star.Y}, {star.Z})");
    }
}

public readonly record struct StarData(double SquaredDistance, string StarName, double X, double Y, double Z);

public static class StarFinder
{
    public static List<(string StarName, double X, double Y, double Z)> FindClosestStars(
        string filePath, double xCoord, double yCoord, double zCoord, int starsToReturn)
    {
        var maxHeap = new PriorityQueue<StarData, double>();

        using var reader = new StreamReader(filePath);
        while (reader.ReadLine() is { } line)
        {
            var parts = line.Split(',');
            if (parts.Length != 4) continue;

            var starName = parts[0];
            if (!double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var x) ||
                !double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var y) ||
                !double.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out var z))
            {
                continue;
            }

            var squaredDistance = GetSquaredDistance(xCoord, yCoord, zCoord, x, y, z);

            maxHeap.Enqueue(new StarData(squaredDistance, starName, x, y, z), -squaredDistance);

            if (maxHeap.Count > starsToReturn)
            {
                maxHeap.Dequeue();
            }
        }

        var closestStars = new List<StarData>();

        while (maxHeap.Count > 0)
        {
            closestStars.Add(maxHeap.Dequeue());
        }

        return closestStars
            .OrderBy(s => s.SquaredDistance)
            .Select(s => (s.StarName, s.X, s.Y, s.Z))
            .ToList();
    }

    private static double GetSquaredDistance(double x1, double y1, double z1, double x2, double y2, double z2)
    {
        return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) + (z1 - z2) * (z1 - z2);
    }
}

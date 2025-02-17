using System.Globalization;

if (args.Length != 5)
    Console.WriteLine("Usage: dotnet run <filePath> <xCoord> <yCoord> <zCoord> <starsToReturn>");


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

public readonly record struct StarData(double Distance, string StarName, double X, double Y, double Z);

public static class StarFinder
{
    public static List<(string StarName, double X, double Y, double Z)> FindClosestStars(
        string filePath, double xCoord, double yCoord, double zCoord, int starsToReturn)
    {
        var maxHeap = new SortedSet<StarData>(new StarComparer());

        using var reader = new StreamReader(filePath);
        while (reader.ReadLine() is { } line)
        {
            var parts = line.Split(',');
            if (parts.Length != 4) continue;

            var starName = parts[0];
            if (!double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double x) ||
                !double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out double y) ||
                !double.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out double z))
            {
                continue;
            }

            var distance = GetDistance(xCoord, yCoord, zCoord, x, y, z);
            maxHeap.Add(new StarData(distance, starName, x, y, z));

            if (maxHeap.Count > starsToReturn)
            {
                maxHeap.Remove(maxHeap.First()); // Remove the farthest star
            }
        }

        return maxHeap.OrderBy(s => s.Distance)
            .Select(s => (s.StarName, s.X, s.Y, s.Z))
            .ToList();
    }

    // Compute Euclidean distance
    private static double GetDistance(double x1, double y1, double z1, double x2, double y2, double z2)
    {
        return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) + (z1 - z2) * (z1 - z2));
    }
}

public class StarComparer : IComparer<StarData>
{
    public int Compare(StarData a, StarData b)
    {
        var distanceComparison = b.Distance.CompareTo(a.Distance);
        return distanceComparison != 0 ? distanceComparison : string.Compare(a.StarName, b.StarName, StringComparison.Ordinal);
    }
}

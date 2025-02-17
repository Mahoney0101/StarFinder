namespace StarFinderTest;

[TestFixture]
public class StarFinderTests
{
    private string testFilePath = "test_stars.txt";

    [Test]
    public void FindClosestStars_ReturnsCorrectResults()
    {
        File.WriteAllLines(testFilePath, [
            "AlphaCentauri,1.34,2.12,3.98",
            "Betelgeuse,5.23,-1.12,7.45",
            "Sirius,2.98,3.67,-0.56",
            "Vega,-4.12,8.56,2.01",
            "ProximaCentauri,1.01,1.02,1.03"
        ]);

        var result = StarFinder.FindClosestStars(testFilePath, 1.0, 1.0, 1.0, 2);

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].StarName, Is.EqualTo("ProximaCentauri"));
        Assert.That(result[1].StarName, Is.EqualTo("AlphaCentauri"));
    }

    [TearDown]
    public void CleanUp()
    {
        if (File.Exists(testFilePath))
        {
            File.Delete(testFilePath);
        }
    }
}
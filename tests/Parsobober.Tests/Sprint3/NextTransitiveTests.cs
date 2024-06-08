namespace Parsobober.Tests.Sprint3;

public class NextTransitiveTests() : BaseTestClass(Code.ZadanieDomowe1)
{
    [Theory]
    [InlineData("8",
        "9, 10, 11, 12, 13, 14, 15, 16, 17")]
    [InlineData("19",
        "18, 19, 20, 21, 22")]
    public void NextTransitiveTestLeft(string line, string expected)
    {
        var query = $"prog_line pl;Select pl such that Next*({line}, pl)";
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }

    [Theory]
    [InlineData("4",
        "1, 2, 3")]
    [InlineData("10",
        "1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12")]
    [InlineData("22",
        "18, 19, 20, 21")]
    [InlineData("17",
        "1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16")]
    public void NextTransitiveTestRight(string line, string expected)
    {
        var query = $"prog_line pl;Select pl such that Next*(pl, {line})";
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }

    [Theory]
    [InlineData("1", "17", "TRUE")]
    [InlineData("7", "12", "TRUE")]
    [InlineData("7", "15", "TRUE")]
    [InlineData("12", "9", "FALSE")]
    public void NextTransitiveTestBoolean(string lineLeft, string lineRight, string expected)
    {
        var query = $"Select BOOLEAN such that Next*({lineLeft}, {lineRight})";
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }
}
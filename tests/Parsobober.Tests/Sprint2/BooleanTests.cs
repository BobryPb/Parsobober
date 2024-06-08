namespace Parsobober.Tests.Sprint2;

public class BooleanTests() : BaseTestClass(Code.ZadanieDomowe1)
{
    [Theory]
    // Parent
    [InlineData("""
                stmt s, s1;
                Select BOOLEAN such that Parent(s, s1)
                """, true)]
    [InlineData("""
                stmt s; assign a;
                Select BOOLEAN such that Parent(a, s)
                """, false)]
    [InlineData("Select BOOLEAN such that Parent(1, 2)", false)]
    [InlineData("Select BOOLEAN such that Parent(8, 9)", true)]
    [InlineData("Select BOOLEAN such that Parent(8, 11)", false)]
    [InlineData("Select BOOLEAN such that Parent*(8, 11)", true)]
    // Follows
    [InlineData("Select BOOLEAN such that Follows(1, 2)", true)]
    [InlineData("Select BOOLEAN such that Follows(2, 1)", false)]
    [InlineData("Select BOOLEAN such that Follows(1, 4)", false)]
    [InlineData("Select BOOLEAN such that Follows*(1, 4)", true)]
    // Modifies
    [InlineData("""
                Select BOOLEAN such that Modifies(1, "x")
                """, false)]
    [InlineData("""
                Select BOOLEAN such that Modifies(1, "t")
                """, true)]
    // Uses
    [InlineData("""
                Select BOOLEAN such that Uses(2, "x")
                """, false)]
    [InlineData("""
                Select BOOLEAN such that Uses(2, "t")
                """, true)]
    [InlineData("""
                stmt s; assign a;
                Select BOOLEAN such that Parent(s, a) with s.stmt# = 1 and a.stmt# = 2
                """, false)]
    [InlineData("""
                stmt s; assign a;
                Select BOOLEAN such that Parent(s, a) with s.stmt# = 8 and a.stmt# = 9
                """, true)]
    public void BooleanTest(string query, bool expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected ? "true" : "false");
    }
}
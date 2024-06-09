namespace Parsobober.Tests.Sprint1;

public class FollowsTransitiveTestsShort() : BaseTestClass(Code.ShortStatementsOnly)
{
    [Theory]
    // stmt
    [InlineData("stmt s;", "s", "1", "none")]
    [InlineData("stmt s;", "s", "2", "none")]
    [InlineData("stmt s;", "s", "3", "2")]
    [InlineData("stmt s;", "s", "4", "2,3")]
    [InlineData("stmt s;", "s", "5", "none")]
    [InlineData("stmt s;", "s", "6", "2,3,4")]
    // while
    [InlineData("while w;", "w", "1", "none")]
    [InlineData("while w;", "w", "2", "none")]
    [InlineData("while w;", "w", "3", "none")]
    [InlineData("while w;", "w", "4", "none")]
    [InlineData("while w;", "w", "5", "none")]
    [InlineData("while w;", "w", "6", "4")]
    // assign
    [InlineData("assign a;", "a", "1", "none")]
    [InlineData("assign a;", "a", "2", "none")]
    [InlineData("assign a;", "a", "3", "2")]
    [InlineData("assign a;", "a", "4", "2,3")]
    [InlineData("assign a;", "a", "5", "none")]
    [InlineData("assign a;", "a", "6", "2,3")]
    public void FollowsTransitive_Statement_Line(string declaration, string select, string line, string expected)
    {
        var query = $"{declaration}\nSelect {select} such that Follows* ({select}, {line})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("stmt s;", "1", "s", "none")]
    [InlineData("stmt s;", "2", "s", "3,4,6")]
    [InlineData("stmt s;", "3", "s", "4,6")]
    [InlineData("stmt s;", "4", "s", "6")]
    [InlineData("stmt s;", "5", "s", "none")]
    [InlineData("stmt s;", "6", "s", "none")]
    // while
    [InlineData("while w;", "1", "w", "none")]
    [InlineData("while w;", "2", "w", "4")]
    [InlineData("while w;", "3", "w", "4")]
    [InlineData("while w;", "4", "w", "none")]
    [InlineData("while w;", "5", "w", "none")]
    [InlineData("while w;", "6", "w", "none")]
    // assign
    [InlineData("assign a;", "1", "a", "none")]
    [InlineData("assign a;", "2", "a", "3,6")]
    [InlineData("assign a;", "3", "a", "6")]
    [InlineData("assign a;", "4", "a", "6")]
    [InlineData("assign a;", "5", "a", "none")]
    [InlineData("assign a;", "6", "a", "none")]
    public void FollowsTransitive_Line_Statement(string declaration, string line, string select, string expected)
    {
        var query = $"{declaration}\nSelect {select} such that Follows* ({line}, {select})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("stmt s1, s2;", "s1", "s2", "2,3,4")]
    [InlineData("stmt s; assign a;", "s", "a", "2,3,4")]
    [InlineData("stmt s; while w;", "s", "w", "2,3")]
    // while
    [InlineData("while w1, w2;", "w1", "w2", "none")]
    [InlineData("while w; assign a;", "w", "a", "4")]
    [InlineData("while w; stmt s;", "w", "s", "4")]
    // assign
    [InlineData("assign a1, a2;", "a1", "a2", "2,3")]
    [InlineData("assign a; stmt s;", "a", "s", "2,3")]
    [InlineData("assign a; while w;", "a", "w", "2,3")]
    public void FollowsTransitive_Statement_Statement_Select_Left(string declaration, string left, string right,
        string expected)
    {
        var query = $"{declaration}\nSelect {left} such that Follows* ({left}, {right})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("stmt s1, s2;", "s2", "s1", "3,4,6")]
    [InlineData("stmt s; assign a;", "a", "s", "3,4,6")]
    [InlineData("stmt s; while w;", "w", "s", "6")]
    // while
    [InlineData("while w1, w2;", "w2", "w1", "none")]
    [InlineData("while w; assign a;", "a", "w", "4")]
    [InlineData("while w; stmt s;", "s", "w", "4")]
    // assign
    [InlineData("assign a1, a2;", "a2", "a1", "3,6")]
    [InlineData("assign a; stmt s;", "s", "a", "3,6")]
    [InlineData("assign a; while w;", "w", "a", "6")]
    public void FollowsTransitive_Statement_Statement_Select_Right(string declaration, string left, string right,
        string expected)
    {
        var query = $"{declaration}\nSelect {right} such that Follows* ({left}, {right})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }
}

public class FollowsTransitiveTestsLong() : BaseTestClass(Code.LongStatementsOnly)
{
    [Theory]
    // stmt
    [InlineData("stmt s;", "s", "7", "6")]
    [InlineData("stmt s;", "s", "11", "none")]
    // while
    [InlineData("while w;", "w", "11", "none")]
    public void FollowsTransitive_Statement_Line(string declaration, string select, string line, string expected)
    {
        var query = $"{declaration}\nSelect {select} such that Follows* ({select}, {line})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("stmt s;", "2", "s", "14,15,24")]
    [InlineData("stmt s;", "17", "s", "20,23")]
    // while
    [InlineData("while w;", "2", "w", "15")]
    [InlineData("while w;", "15", "w", "none")]
    public void FollowsTransitive_Line_Statement(string declaration, string line, string select, string expected)
    {
        var query = $"{declaration}\nSelect {select} such that Follows* ({line}, {select})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("stmt s1, s2;", "s1", "s2", "1,2,3,4,5,6,7,8,10,14,15,16,17,18,20,21")]
    // while
    [InlineData("stmt s; while w;", "w", "s", "2,5,7,10,15,17,20")]
    [InlineData("while w1, w2;", "w1", "w2", "2,7,17")]
    // assign
    [InlineData("stmt s; assign a;", "a", "s", "1,3,4,6,8,14,16,18,21")]
    public void FollowsTransitive_Statement_Statement_Select_Left(string declaration, string left, string right,
        string expected)
    {
        var query = $"{declaration}\nSelect {left} such that Follows* ({left}, {right})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("stmt s1, s2;", "s2", "s1", "2,4,5,7,9,10,12,13,14,15,17,19,20,22,23,24")]
    [InlineData("stmt s; assign a;", "a", "s", "2,4,5,7,9,10,12,13,14,15,17,19,20,22,23,24")]
    public void FollowsTransitive_Statement_Statement_Select_Right(string declaration, string left, string right,
        string expected)
    {
        var query = $"{declaration}\nSelect {right} such that Follows* ({left}, {right})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }
}
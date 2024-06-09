namespace Parsobober.Tests.Sprint1;

public class ParentTransitiveTestsShort() : BaseTestClass(Code.ShortStatementsOnly)
{
    [Theory]
    // stmt
    [InlineData("stmt s;", "s", "1", "none")]
    [InlineData("stmt s;", "s", "2", "1")]
    [InlineData("stmt s;", "s", "3", "1")]
    [InlineData("stmt s;", "s", "4", "1")]
    [InlineData("stmt s;", "s", "5", "1,4")]
    [InlineData("stmt s;", "s", "6", "1")]
    // while
    [InlineData("while w;", "w", "1", "none")]
    [InlineData("while w;", "w", "2", "1")]
    [InlineData("while w;", "w", "3", "1")]
    [InlineData("while w;", "w", "4", "1")]
    [InlineData("while w;", "w", "5", "1,4")]
    [InlineData("while w;", "w", "6", "1")]
    // assign
    [InlineData("assign a;", "a", "1", "none")]
    [InlineData("assign a;", "a", "2", "none")]
    [InlineData("assign a;", "a", "3", "none")]
    [InlineData("assign a;", "a", "4", "none")]
    [InlineData("assign a;", "a", "6", "none")]
    [InlineData("assign a;", "a", "5", "none")]
    public void ParentTransitive_Statement_Line(string declaration, string select, string line, string expected)
    {
        var query = $"{declaration}\nSelect {select} such that Parent* ({select}, {line})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("stmt s;", "1", "s", "2,3,4,5,6")]
    [InlineData("stmt s;", "2", "s", "none")]
    [InlineData("stmt s;", "3", "s", "none")]
    [InlineData("stmt s;", "4", "s", "5")]
    [InlineData("stmt s;", "5", "s", "none")]
    [InlineData("stmt s;", "6", "s", "none")]
    // while
    [InlineData("while w;", "1", "w", "4")]
    [InlineData("while w;", "2", "w", "none")]
    [InlineData("while w;", "3", "w", "none")]
    [InlineData("while w;", "4", "w", "none")]
    [InlineData("while w;", "5", "w", "none")]
    [InlineData("while w;", "6", "w", "none")]
    // assign
    [InlineData("assign a;", "1", "a", "2,3,5,6")]
    [InlineData("assign a;", "2", "a", "none")]
    [InlineData("assign a;", "3", "a", "none")]
    [InlineData("assign a;", "4", "a", "5")]
    [InlineData("assign a;", "5", "a", "none")]
    [InlineData("assign a;", "6", "a", "none")]
    public void ParentTransitive_Line_Statement(string declaration, string line, string select, string expected)
    {
        var query = $"{declaration}\nSelect {select} such that Parent* ({line}, {select})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("stmt s1, s2;", "s1", "s2", "1,4")]
    [InlineData("stmt s; assign a;", "s", "a", "1,4")]
    [InlineData("stmt s; while w;", "s", "w", "1")]
    // while
    [InlineData("while w1, w2;", "w1", "w2", "1")]
    [InlineData("while w; assign a;", "w", "a", "1,4")]
    [InlineData("while w; stmt s;", "w", "s", "1,4")]
    // assign
    [InlineData("assign a1, a2;", "a1", "a2", "none")]
    [InlineData("assign a; stmt s;", "a", "s", "none")]
    [InlineData("assign a; while w;", "a", "w", "none")]
    public void ParentTransitive_Statement_Statement_Select_Left(string declaration, string left, string right,
        string expected)
    {
        var query = $"{declaration}\nSelect {left} such that Parent* ({left}, {right})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("stmt s1, s2;", "s2", "s1", "2,3,4,5,6")]
    [InlineData("stmt s; assign a;", "a", "s", "none")]
    [InlineData("stmt s; while w;", "w", "s", "2,3,4,5,6")]
    // while
    [InlineData("while w1, w2;", "w2", "w1", "4")]
    [InlineData("while w; assign a;", "a", "w", "none")]
    [InlineData("while w; stmt s;", "s", "w", "4")]
    // assign
    [InlineData("assign a1, a2;", "a2", "a1", "none")]
    [InlineData("assign a; stmt s;", "s", "a", "2,3,5,6")]
    [InlineData("assign a; while w;", "w", "a", "2,3,5,6")]
    public void ParentTransitive_Statement_Statement_Select_Right(string declaration, string left, string right,
        string expected)
    {
        var query = $"{declaration}\nSelect {right} such that Parent* ({left}, {right})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }
}

public class ParentTransitiveTestsLong() : BaseTestClass(Code.LongStatementsOnly)
{
    [Theory]
    // stmt
    [InlineData("stmt s;", "s", "7", "2,5")]
    [InlineData("stmt s;", "s", "11", "2,5,10")]
    // while
    [InlineData("while w;", "w", "11", "2,5,10")]
    public void ParentTransitive_Statement_Line(string declaration, string select, string line, string expected)
    {
        var query = $"{declaration}\nSelect {select} such that Parent* ({select}, {line})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("stmt s;", "2", "s", "3,4,5,6,7,8,9,10,11,12,13")]
    [InlineData("stmt s;", "17", "s", "18,19")]
    // while
    [InlineData("while w;", "2", "w", "5,7,10")]
    [InlineData("while w;", "15", "w", "17,20")]
    public void ParentTransitive_Line_Statement(string declaration, string line, string select, string expected)
    {
        var query = $"{declaration}\nSelect {select} such that Parent* ({line}, {select})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("stmt s1, s2;", "s1", "s2", "2,5,7,10,15,17,20")]
    // while
    [InlineData("stmt s; while w;", "w", "s", "2,5,7,10,15,17,20")]
    [InlineData("while w1, w2;", "w1", "w2", "2,5,15")]
    // assign
    [InlineData("stmt s; assign a;", "a", "s", "none")]
    public void ParentTransitive_Statement_Statement_Select_Left(string declaration, string left, string right,
        string expected)
    {
        var query = $"{declaration}\nSelect {left} such that Parent* ({left}, {right})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("stmt s1, s2;", "s2", "s1", "3,4,5,6,7,8,9,10,11,12,13,16,17,18,19,20,21,22,23")]
    [InlineData("stmt s; assign a;", "a", "s", "none")]
    public void ParentTransitive_Statement_Statement_Select_Right(string declaration, string left, string right,
        string expected)
    {
        var query = $"{declaration}\nSelect {right} such that Parent* ({left}, {right})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }
}
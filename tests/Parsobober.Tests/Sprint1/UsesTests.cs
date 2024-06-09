namespace Parsobober.Tests.Sprint1;

public class UsesTests() : BaseTestClass(Code.ShortStatementsOnly)
{
    [Theory]
    // stmt
    [InlineData("stmt s;", "s", "\"k\"", "none")]
    [InlineData("stmt s;", "s", "\"a\"", "1,2,4")]
    [InlineData("stmt s;", "s", "\"p\"", "1,4,5")]
    [InlineData("stmt s;", "s", "\"m\"", "1,6")]
    // while
    [InlineData("while w;", "w", "\"k\"", "none")]
    [InlineData("while w;", "w", "\"c\"", "1")]
    [InlineData("while w;", "w", "\"g\"", "1")]
    // assign
    [InlineData("assign a;", "a", "\"k\"", "none")]
    [InlineData("assign a;", "a", "\"p\"", "5")]
    public void Uses_Statement_VariableName(string declaration, string select, string varName, string expected)
    {
        var query = $"{declaration}\nSelect {select} such that Uses ({select}, {varName})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("variable v;", "2", "v", "a,c")]
    // while
    [InlineData("variable v;", "1", "v", "a,b,c,g,m,p,w,y")]
    [InlineData("variable v;", "4", "v", "a,p")]
    public void Uses_Line_Variable(string declaration, string line, string select, string expected)
    {
        var query = $"{declaration}\nSelect {select} such that Uses ({line}, {select})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("stmt s;variable v;", "s", "v", "1,2,3,4,5,6")]
    [InlineData("while w;variable v;", "w", "v", "1,4")]
    [InlineData("assign a;variable v;", "a", "v", "2,3,5,6")]
    public void Uses_Statement_Variable_Select_Left(string declaration, string left, string right, string expected)
    {
        var query = $"{declaration}\nSelect {left} such that Uses ({left}, {right})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("stmt s;variable v;", "s", "v", "a,b,c,g,m,p,w,y")]
    [InlineData("while w;variable v;", "w", "v", "a,b,c,g,m,p,w,y")]
    [InlineData("assign a;variable v;", "a", "v", "a,b,c,g,m,p,w")]
    public void Uses_Statement_Variable_Select_Right(string declaration, string left, string right, string expected)
    {
        var query = $"{declaration}\nSelect {right} such that Uses ({left}, {right})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }
}

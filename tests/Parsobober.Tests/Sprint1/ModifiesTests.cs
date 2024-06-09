namespace Parsobober.Tests.Sprint1;

public class ModifiesTests() : BaseTestClass(Code.ShortStatementsOnly)
{
    [Theory]
    // stmt
    [InlineData("stmt s;", "s", "\"k\"", "none")]
    [InlineData("stmt s;", "s", "\"z\"", "1,3")]
    [InlineData("stmt s;", "s", "\"t\"", "1,4,5")]
    [InlineData("stmt s;", "s", "\"r\"", "1,6")]
    // while
    [InlineData("while w;", "w", "\"k\"", "none")]
    [InlineData("while w;", "w", "\"z\"", "1")]
    [InlineData("while w;", "w", "\"t\"", "1,4")]
    // assign
    [InlineData("assign a;", "a", "\"k\"", "none")]
    [InlineData("assign a;", "a", "\"z\"", "3")]
    public void Modifies_Statement_VariableName(string declaration, string select, string varName, string expected)
    {
        var query = $"{declaration}\nSelect {select} such that Modifies ({select}, {varName})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("variable v;", "2", "v", "x")]
    // while
    [InlineData("variable v;", "1", "v", "r,t,x,z")]
    [InlineData("variable v;", "4", "v", "t")]
    public void Modifies_Line_Variable(string declaration, string line, string select, string expected)
    {
        var query = $"{declaration}\nSelect {select} such that Modifies ({line}, {select})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("stmt s;variable v;", "s", "v", "1,2,3,4,5,6")]
    [InlineData("while w;variable v;", "w", "v", "1,4")]
    [InlineData("assign a;variable v;", "a", "v", "2,3,5,6")]
    public void Modifies_Statement_Variable_Select_Left(string declaration, string left, string right, string expected)
    {
        var query = $"{declaration}\nSelect {left} such that Modifies ({left}, {right})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("stmt s;variable v;", "s", "v", "r,t,x,z")]
    [InlineData("while w;variable v;", "w", "v", "r,t,x,z")]
    [InlineData("assign a;variable v;", "a", "v", "r,t,x,z")]
    public void Modifies_Statement_Variable_Select_Right(string declaration, string left, string right, string expected)
    {
        var query = $"{declaration}\nSelect {right} such that Modifies ({left}, {right})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }
}
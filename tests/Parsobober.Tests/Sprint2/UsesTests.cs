namespace Parsobober.Tests.Sprint2;

public class UsesTests() : BaseTestClass(Code.ZadanieDomowe1)
{
    [Theory]
    [InlineData("\"Circle\"", "a,b,c,d,k,t")]
    [InlineData("\"Rectangle\"", "a,b,c,d,k,t")]
    [InlineData("\"Triangle\"", "a,b,d,k,t")]
    [InlineData("\"Hexagon\"", "a,t")]
    public void Uses_ProcedureName_Variable(string procedureName, string expected)
    {
        var query = $"variable v;\nSelect v such that Uses ({procedureName}, v)";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("\"a\"", "Circle,Hexagon,Rectangle,Triangle")]
    [InlineData("\"t\"", "Circle,Hexagon,Rectangle,Triangle")]
    [InlineData("\"b\"", "Circle,Rectangle,Triangle")]
    [InlineData("\"c\"", "Circle,Rectangle")]
    [InlineData("\"d\"", "Circle,Rectangle,Triangle")]
    [InlineData("\"k\"", "Circle,Rectangle,Triangle")]
    public void Uses_Procedure_VariableName(string variableName, string expected)
    {
        var query = $"procedure p;\nSelect p such that Uses (p, {variableName})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("\"Circle\"", "\"a\"", "true")]
    [InlineData("\"Circle\"", "\"k\"", "true")]
    [InlineData("\"Rectangle\"", "\"k\"", "true")]
    [InlineData("\"Hexagon\"", "\"k\"", "false")]
    [InlineData("\"Triangle\"", "\"a\"", "true")]
    [InlineData("\"Circle\"", "\"x\"", "false")]
    public void BooleanUses_ProcedureName_VariableName(string procedureName, string variableName, string expected)
    {
        var query = $"\nSelect BOOLEAN such that Uses ({procedureName}, {variableName})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }
}

public class UsesDeepTests() : BaseTestClass(Code.CallCode)
{
    [Theory]
    // stmt
    [InlineData("stmt s;", "s", "\"a\"", "2, 3, 4, 5, 6, 7, 8, 9, 12, 13, 15, 16, 18")]
    [InlineData("stmt s;", "s", "\"c\"", "4, 5, 6, 7, 9, 12, 14")]
    // while
    [InlineData("while w;", "w", "\"d\"", "6, 12, 15")]
    [InlineData("while w;", "w", "\"a\"", "6, 12, 15")]
    // assign
    [InlineData("assign a;", "a", "\"a\"", "3, 18")]
    [InlineData("assign a;", "a", "\"x\"", "None")]
    // if
    [InlineData("if i;", "i", "\"t\"", "5, 7, 16")]
    [InlineData("if i;", "i", "\"a\"", "5, 7, 16")]
    // call
    [InlineData("call c;", "c", "\"a\"", "2, 4, 8, 9, 13")]
    [InlineData("call c;", "c", "\"t\"", "2, 4, 8, 9, 13")]
    public void Uses_Statement_VariableName(string declaration, string select, string varName, string expected)
    {
        var query = $"{declaration}\nSelect {select} such that Uses ({select}, {varName})";
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }

    [Theory]
    [InlineData("2", "a, b, d, k, t")]
    [InlineData("4", "a, b, c, d, k, t")]
    [InlineData("5", "a, b, c, d, k, t")]
    [InlineData("6", "a, b, c, d, k, t")]
    [InlineData("15", "a, b, d, k, t")]
    public void Uses_StatementLine_Variable(string lineNumber, string expected)
    {
        var query = $"variable v;\nSelect v such that Uses ({lineNumber}, v)";
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }

    [Theory]
    [InlineData("stmt s; variable v;", "s", "s", "2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19")]
    [InlineData("while w; variable v;", "w", "w", "6, 12, 15")]
    [InlineData("assign a; variable v;", "a", "a", "3, 10, 11, 14, 17, 18, 19")]
    [InlineData("if i; variable v;", "i", "i", "5, 7, 16")]
    [InlineData("call c; variable v;", "c", "c", "2, 4, 8, 9, 13")]
    public void Uses_Statement_Variable_Select_Statement(string declaration, string statement, string select,
        string expected)
    {
        var query = $"{declaration}\nSelect {select} such that Uses ({statement}, v)";
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }

    [Theory]
    [InlineData("stmt s; variable v;", "s", "a, b, c, d, k, t")]
    [InlineData("while w; variable v;", "w", "a, b, c, d, k, t")]
    [InlineData("assign a; variable v;", "a", "a, b, c, d, k, t")]
    [InlineData("if i; variable v;", "i", "a, b, c, d, k, t")]
    [InlineData("call c; variable v;", "c", "a, b, c, d, k, t")]
    public void Uses_Statement_Variable_Select_Variable(string declaration, string statement, string expected)
    {
        var query = $"{declaration}\nSelect v such that Uses ({statement}, v)";
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }

    [Theory]
    [InlineData("""
                stmt s; if i; while w; assign a; call c;
                Select a such that Uses(a, "a") and Uses(c, "a") and Parent(i, a)
                """, "18")]
    [InlineData("""
                stmt s; if i; while w; assign a; call c;
                Select c such that Uses(a, "a") and Uses(c, "a") and Parent(i, c)
                """, "8,9")]
    [InlineData("""
                stmt s; while w; assign a; procedure p; variable v;
                Select p such that Uses(p, v) and Uses(3, v)
                """, "Circle,Rectangle,Triangle")]
    public void Uses_Multiple_Relation(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected);
    }
}
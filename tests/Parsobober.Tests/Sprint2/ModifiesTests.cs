namespace Parsobober.Tests.Sprint2;

public class ModifiesTests() : BaseTestClass(Code.ZadanieDomowe1)
{
    [Theory]
    [InlineData("\"Circle\"", "a,b,c,d,k,t")]
    [InlineData("\"Rectangle\"", "a,c,d,t")]
    [InlineData("\"Triangle\"", "a,c,d")]
    [InlineData("\"Hexagon\"", "t")]
    public void Modifies_ProcedureName_Variable(string procedureName, string expected)
    {
        var query = $"variable v;\nSelect v such that Modifies ({procedureName}, v)";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("\"a\"", "Circle,Rectangle,Triangle")]
    [InlineData("\"t\"", "Circle,Hexagon,Rectangle")]
    [InlineData("\"b\"", "Circle")]
    [InlineData("\"c\"", "Circle,Rectangle,Triangle")]
    [InlineData("\"d\"", "Circle,Rectangle,Triangle")]
    public void Modifies_Procedure_VariableName(string variableName, string expected)
    {
        var query = $"procedure p;\nSelect p such that Modifies (p, {variableName})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("\"Circle\"", "\"a\"", "true")]
    [InlineData("\"Circle\"", "\"k\"", "true")]
    [InlineData("\"Rectangle\"", "\"k\"", "false")]
    [InlineData("\"Triangle\"", "\"a\"", "true")]
    [InlineData("\"Circle\"", "\"x\"", "false")]
    public void BooleanModifies_ProcedureName_VariableName(string procedureName, string variableName, string expected)
    {
        var query = $"\nSelect BOOLEAN such that Modifies ({procedureName}, {variableName})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    // stmt
    [InlineData("stmt s;", "s", "\"a\"", "2, 4, 8, 13, 14, 17, 18, 20, 23, 24, 26")]
    [InlineData("stmt s;", "s", "\"c\"", "4, 8, 10, 12, 16, 17, 18, 20, 21, 27")]
    // while
    [InlineData("while w;", "w", "\"d\"", "10, 18, 23")]
    [InlineData("while w;", "w", "\"a\"", "18, 23")]
    // assign
    [InlineData("assign a;", "a", "\"a\"", "2, 13, 14, 26")]
    [InlineData("assign a;", "a", "\"x\"", "none")]
    // if
    [InlineData("if i;", "i", "\"t\"", "8")]
    [InlineData("if i;", "i", "\"a\"", "8, 24")]
    // call
    [InlineData("call c;", "c", "\"a\"", "4, 17, 20")]
    [InlineData("call c;", "c", "\"t\"", "6, 15, 17")]
    public void Modifies_Statement_VariableName(string declaration, string select, string varName, string expected)
    {
        var query = $"{declaration}\nSelect {select} such that Modifies ({select}, {varName})";
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }
}

public class ModifiesDeepTests() : BaseTestClass(Code.CallCode)
{
    [Theory]
    // stmt
    [InlineData("stmt s;", "s", "\"a\"", "2, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 15, 16, 18")]
    [InlineData("stmt s;", "s", "\"c\"", "2, 4, 5, 6, 7, 8, 9, 12, 13, 14, 19")]
    // while
    [InlineData("while w;", "w", "\"d\"", "6, 12, 15")]
    [InlineData("while w;", "w", "\"a\"", "6, 12, 15")]
    // assign
    [InlineData("assign a;", "a", "\"a\"", "10, 11, 18")]
    [InlineData("assign a;", "a", "\"x\"", "none")]
    // if
    [InlineData("if i;", "i", "\"t\"", "none")]
    [InlineData("if i;", "i", "\"a\"", "5, 7, 16")]
    // call
    [InlineData("call c;", "c", "\"a\"", "2, 4, 8, 9, 13")]
    [InlineData("call c;", "c", "\"t\"", "none")]
    public void Modifies_Statement_VariableName(string declaration, string select, string varName, string expected)
    {
        var query = $"{declaration}\nSelect {select} such that Modifies ({select}, {varName})";
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }

    [Theory]
    [InlineData("2", "a, c, d")]
    [InlineData("4", "a, c, d")]
    [InlineData("5", "a, c, d")]
    [InlineData("6", "a, c, d")]
    [InlineData("15", "a, d")]
    public void Modifies_StatementLine_Variable(string lineNumber, string expected)
    {
        var query = $"variable v;\nSelect v such that Modifies ({lineNumber}, v)";
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }

    [Theory]
    [InlineData("stmt s; variable v;", "s", "s", "1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19")]
    [InlineData("while w; variable v;", "w", "w", "6, 12, 15")]
    [InlineData("assign a; variable v;", "a", "a", "1, 3, 10, 11, 14, 17, 18, 19")]
    [InlineData("if i; variable v;", "i", "i", "5, 7, 16")]
    [InlineData("call c; variable v;", "c", "c", "2, 4, 8, 9, 13")]
    public void Modifies_Statement_Variable_Select_Statement(string declaration, string statement, string select,
        string expected)
    {
        var query = $"{declaration}\nSelect {select} such that Modifies ({statement}, v)";
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }

    [Theory]
    [InlineData("stmt s; variable v;", "s", "a, b, c, d, t")]
    [InlineData("while w; variable v;", "w", "a, c, d")]
    [InlineData("assign a; variable v;", "a", "a, b, c, d, t")]
    [InlineData("if i; variable v;", "i", "a, c, d")]
    [InlineData("call c; variable v;", "c", "a, c, d")]
    public void Modifies_Statement_Variable_Select_Variable(string declaration, string statement, string expected)
    {
        var query = $"{declaration}\nSelect v such that Modifies ({statement}, v)";
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }

    [Theory]
    [InlineData("""
                stmt s; if i; while w; assign a; call c;
                Select a such that Modifies(a, "a") and Modifies(c, "a") and Parent(i, a)
                """, "10,11,18")]
    [InlineData("""
                stmt s; while w; assign a; procedure p; variable v;
                Select p such that Modifies(p, v) and Modifies(3, v)
                """, "Circle")]
    public void Modifies_Multiple_Relation(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected);
    }
}
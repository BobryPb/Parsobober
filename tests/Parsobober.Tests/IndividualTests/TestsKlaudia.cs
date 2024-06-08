namespace Parsobober.Tests.IndividualTests;

public class TestsKlaudia() : BaseTestClass(Code.simpleCodeKM)
{

    # region Test - 1 relation

    [Theory]
    [InlineData("stmt s;", "s", "Follows", "1", "None")]
    //[InlineData("stmt s;", "4", "Follows", "s", "None")]
    [InlineData("assign a;", "a", "Follows", "7", "6")]
    [InlineData("stmt s;", "s", "Parent", "5", "3")]
    //[InlineData("stmt s;", "2", "Parent", "s", "None")]
    public void FollowsParent_Select_Line(string declaration,
                                string select,
                                string relation,
                                string line,
                                string expected
                                )
    {
        var query = $"{declaration}\nSelect {select} such that {relation} ({select}, {line})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("assign a1, a2;", "a1", "Follows", "a2", "1,6,8")]
    public void Follows_Select_Assign(string declaration,
                                        string left,
                                        string relation,
                                        string right,
                                        string expected)
    {
        var query = $"{declaration}\nSelect {left} such that {relation} ({left}, {right})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("variable v;", "5", "Modifies", "v", "i,y")]
    public void Modifies_Line(string declaration,
                            string line,
                            string relation,
                            string select,
                            string expected
                            )
    {
        var query = $"{declaration}\nSelect {select} such that {relation} ({line}, {select})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("assign a;", "a", "Modifies", "g", "11")]
    public void Modifies_VariableName(string declaration,
                                        string select,
                                        string relation,
                                        string varName,
                                        string expected)
    {
        var query = $"{declaration}\nSelect {select} such that {relation} ({select}, {varName})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("while w;", "w", "Modifies", "i", "3")]
    public void Modifies_Select_While(string declaration,
                                string left,
                                string relation,
                                string right,
                                string expected)
    {
        var query = $"{declaration}\nSelect {left} such that {relation} ({left}, {right})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("variable v;", "3", "Uses", "v", "i,t,x,y,z")]
    public void Uses_Line(string declaration,
                                    string line,
                                    string relation,
                                    string select,
                                    string expected)
    {
        var query = $"{declaration}\nSelect {select} such that {relation} ({line}, {select})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    #endregion

    #region Tests - 2 relations

    [Theory]
    [InlineData("assign a;", "a", "Modifies", "i", "Uses", "9")]
    [InlineData("assign a;", "a", "Modifies", "y", "Uses", "None")]
    [InlineData("assign a;", "a", "Modifies", "z", "Uses", "4")]
    [InlineData("assign a;", "a", "Modifies", "y", "Uses", "None")]
    public void Modifies_Uses_Select(string declaration,
                                string select,
                                string relation,
                                string varName,
                                string relationTwo,
                                string expected)
    {
        var query = $"{declaration}\nSelect {select} such that {relation} ({select}, {varName}) and {relationTwo} ({select}, {varName})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("while w;assign a;", "a", "Modifies", "z", "Parent", "w", "4")]
    [InlineData("while w;assign a;", "a", "Modifies", "x", "Parent", "w", "None")]
    //[InlineData("while w;assign a;", "a", "Modifies", "x", "Follows", "y", "1")]
    public void Modifies_Parent_Select(string declaration,
                                        string select,
                                        string relation,
                                        string varName,
                                        string relationTwo,
                                        string varNameTwo,
                                        string expected)
    {
        var query = $"{declaration}\nSelect {select} such that {relation} ({select}, {varName}) and {relationTwo} ({varNameTwo}, {select})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("while w;assign a;", "a", "Parent", "w", "Modifies", "g", "None")]
    [InlineData("while w;assign a;", "a", "Parent", "w", "Modifies", "y", "10")]
    public void Parent_Modifies_Select(string declaration,
                                        string select,
                                        string relation,
                                        string varName,
                                        string relationTwo,
                                        string varNameTwo,
                                        string expected)
    {
        var query = $"{declaration}\nSelect {select} such that {relation} ({varName}, {select}) and {relationTwo} ({select}, {varNameTwo})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("while w;assign a;", "a", "Uses", "y", "Parent", "w", "4")]
    //[InlineData("while w;assign a;", "a", "Uses", "y", "Parent", "i", "None")]
    public void Uses_Parent_Select(string declaration,
                                   string select,
                                   string relation,
                                   string varName,
                                   string relationTwo,
                                   string varNameTwo,
                                   string expected)
    {
        var query = $"{declaration}\nSelect {select} such that {relation} ({select}, {varName}) and {relationTwo} ({varNameTwo}, {select})";
        var result = App.Query(query);
        result.Should().Be(expected);
    }
    #endregion

}
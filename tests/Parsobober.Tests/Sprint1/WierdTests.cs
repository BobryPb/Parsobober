namespace Parsobober.Tests.Sprint1;

public class WierdTests() : BaseTestClass(Code.ShortStatementsOnly)
{
    [Theory]
    [InlineData("while w; stmt s, s1;", "s1", "Parent(w,s)", "1,2,3,4,5,6")]
    [InlineData("while w; stmt s, s1;", "s1", "Parent(w,2)", "1,2,3,4,5,6")]
    [InlineData("while w, w1; stmt s;", "w1", "Parent(w,s)", "1,4")]
    [InlineData("while w, w1; stmt s;", "w1", "Parent(w,2)", "1,4")]
    [InlineData("assign a; stmt s, s1;", "s1", "Parent(a,s)", "none")]
    public void SelectSomethingElse(string declaration, string select, string relation, string expected)
    {
        var query = $"{declaration}\nSelect {select} such that {relation}";

        var result = App.Query(query);

        result.Should().Be(expected);
    }
}
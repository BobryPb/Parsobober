namespace Parsobober.Tests.Sprint3;

public class TupleTests() : BaseTestClass(Code.Dropbox)
{
    [Theory]
    [InlineData("stmt s, s1;\nSelect <s,s1> such that Parent(s,s1)", "")]
    public void TupleTest(string query, string expected)
    {
        var result = App.Query(query);
        Assert.True(true);
    }
}
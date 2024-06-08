namespace Parsobober.Tests.Sprint3;

public class WeirdNamesTests() : BaseTestClass(Code.Dropbox)
{
    [Theory]
    [InlineData("""
                while whilew;
                Select whilew such that Parent(whilew, 1)
                """, "None")]
    
    [InlineData("""
                if ifs1, ifs2;
                Select ifs2 such that Follows(ifs1, ifs2)
                """, "166, 173")]
    public void NameTests(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }
}
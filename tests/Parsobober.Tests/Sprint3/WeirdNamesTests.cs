namespace Parsobober.Tests.Sprint3;

#if !DEBUG

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
    [InlineData("""
                if ifs1, ifs2; while whilew; constant constantc; variable variablev, variablev1;
                Select ifs2 such that Follows(ifs1, ifs2) such that Parent(whilew, ifs2)
                """, "None")]
    [InlineData("""
                if ifs1; while whilew;variable variablev; procedure procedurep;
                Select ifs1 such that Follows(ifs1, whilew) and Parent(whilew, ifs1) with ifs1.stmt# = whilew.stmt# and procedurep.procName = variablev.varName
                """, "None")]    
    [InlineData("""
                if ifs1; while whilew; procedure procedurep;
                Select ifs1 such that Follows(ifs1,whilew) with procedurep.procName="whilew"
                """, "None")]
    // Cursed test, will not work
    // [InlineData("""
    //             assign and; stmt such, Select; while that;
    //             Select such such that Follows(and, such) and Parent(and, that)
    //             """, "None")]
    // [InlineData("""
    //             assign and; stmt such, Select; while that;
    //             Select such such that Follows(and, such) and Parent(and, that) with such.stmt# = Select.stmt#
    //             """, "None")]
    
    public void NameTests(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }
}

#endif
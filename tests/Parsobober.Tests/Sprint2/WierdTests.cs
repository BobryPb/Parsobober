namespace Parsobober.Tests.Sprint2;

public class WierdTests() : BaseTestClass(Code.ZadanieDomowe1)
{
    [Theory]
    [InlineData("""
                assign a; stmt s, s1, s2;
                Select a such that Parent(s, a) and Parent(s1, s2)
                """, "9,11,12,13,14,16,19,21,25,26"
    )]
    [InlineData("""
                assign a; stmt s, s1, s2, s3;
                Select a such that Parent(s, a) and Parent(s1, s2) and Parent(s2, s3) 
                """, "9,11,12,13,14,16,19,21,25,26")]
    [InlineData("""
                assign a; stmt s, s1, s2, s3, s4;
                Select a such that Parent(s, a) and Parent(s1, s2) and Parent(s3, s4) 
                """, "9,11,12,13,14,16,19,21,25,26")]
    [InlineData("""
                assign a; stmt s;
                Select a such that Parent(s, a) and Parent(s, a) and Parent(s, a)
                """, "9,11,12,13,14,16,19,21,25,26")]
    [InlineData("""
                assign a; stmt s;
                Select a such that Parent(s, a) and Parent(s, a) and Parent(s, a) with a.stmt# = 9
                """, "9")]
    [InlineData("""
                stmt s; assign a; while w;
                Select a such that Parent(w, s) with w.stmt# = 1 and s.stmt# = 2
                """, "None")]
    [InlineData("""
                stmt s; assign a; while w;
                Select a such that Parent(w, s) with w.stmt# = 10 and s.stmt# = 11
                """, "1,2,3,5,7,9,11,12,13,14,16,19,21,22,25,26,27,28")]
    [InlineData("""
                stmt s; assign a;
                Select s such that Parent(s, a) with a.stmt# = 9 and s.stmt# = 8
                """, "8")]
    [InlineData("""
                stmt s; assign a; while w; if i;
                Select s such that Parent(w, i) and Parent(i, a) with w.stmt# = 23 and i.stmt# = 24 and a.stmt# = 25
                """, "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28")]
    [InlineData("""
                stmt s; if i; assign a;
                Select i such that Parent(s,i) and Parent(i,a) and Uses(a, "t") and Modifies(a, "a") and Parent(8,9)
                """, "24")]
    [InlineData("""
                assign a, a1; variable v; stmt s;
                Select a1 such that Parent(a,s) and Modifies(a,"x") and Follows(1,2) and Uses(a1, v)
                """, "None")]
    public void WierdTest(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected);
    }
}
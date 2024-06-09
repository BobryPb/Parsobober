namespace Parsobober.Tests.IndividualTests;

public class TestsBartosz() : BaseTestClass(Code.Dropbox)
{
    [Theory]
    [InlineData("""
                stmt s1;
                Select s1 such that Parent (s1, 9)
                """,
        "6")]
    [InlineData("""
                assign a;
                Select a such that Uses (a, "x")
                """,
        "106, 116")]
    [InlineData("""
                assign a; if i;
                Select i such that Parent(i, a)
                """,
        "14, 15, 23, 34, 38, 51, 55, 66, 72, 76, 80, 86, 97, 109, 140, 144, 159, 160, 163, 166, 170, 173, 176, 192, 198, 204, 224, 237, 241, 250, 266, 267, 303, 309")]
    [InlineData("""
                call c; variable v;
                Select c such that Uses(c,v) with v.varName="left"
                """,
        "33, 45, 78, 108, 114")]
    [InlineData("""
                stmt s;
                Select s such that Follows(s, 5)
                """,
        "4")]
    [InlineData("""
                while w1, w2;
                Select w1 such that Follows(w1, w2)
                """,
        "95, 101, 218")]
    [InlineData("""
                if i1, i2;
                Select i2 such that Parent(i2, i1)
                """,
        "14, 15, 34, 66, 107, 159, 160, 216, 266")]
    [InlineData("""
                procedure p;
                Select p such that Calls(p, "Random")
                """,
        "Draw, Main, Rotate")]
    [InlineData("""
                call c; stmt s;
                Select c such that Parent*(s, c)
                """,
        "18, 22, 33, 41, 42, 44, 45, 54, 62, 63, 78, 84, 87, 92, 108, 114, 117, 118, 195, 221, 222, 227, 231, 232, 235, 274, 277, 292, 293, 307")]
    [InlineData("""
                constant c;
                Select c such that Parent(6,9)
                """,
        "0, 1, 2, 3, 5, 8, 10, 11, 16, 20, 32, 83, 100, 1000")]

    public void TestQuerySingleBartosz(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }

    [Theory]
    [InlineData("""
                stmt s1, s2;
                Select s1 such that Parent*(s1, s2) and Follows*(s1, s2)
                """,
        "None")]
    [InlineData("""
                assign a; call c; variable v;
                Select c such that Modifies(c, v) and Uses(a, v)
                """,
        "1, 5, 18, 22, 33, 41, 42, 44, 45, 54, 62, 63, 78, 84, 87, 92, 108, 114, 117, 118, 119, 190, 195, 208, 221, 222, 227, 231, 232, 235, 259, 262, 274, 277, 287, 293, 294, 296, 297, 307")]
    [InlineData("""
                procedure p1, p2;
                Select p1 such that Calls(p1, p2) and Modifies(p2, "left") with p2.procName="Draw"
                """,
        "Main, Shrink")]
    [InlineData("""
                stmt s1, s2, s3, s4; variable v;
                Select s1 such that Parent(s1, s2) and Modifies(s2, v) and Parent(s3, s4) and Uses(s4, v)
                """,
        "6, 12, 14, 15, 16, 23, 26, 29, 34, 38, 47, 51, 55, 59, 66, 69, 72, 76, 79, 80, 83, 86, 89, 95, 97, 105, 107, 109, 113, 136, 140, 143, 144, 159, 160, 163, 166, 170, 173, 176, 180, 181, 184, 187, 191, 192, 196, 198, 204, 209, 216, 217, 218, 224, 230, 234, 241, 250, 251, 256, 264, 265, 266, 267, 289, 291, 301, 303, 309")]
    [InlineData("""
                call c; procedure p; stmt s;
                Select p such that Modifies(p, "left") and Parent(s, c) with c.procName=p.procName
                """,
        "Draw, Random, Shrink, Translate")]
    [InlineData("""
                call c; procedure p; stmt s;
                Select p such that Modifies(p, "left") and Parent(s, c) with c.procName=p.procName and p.procName = "Draw"
                """,
        "Draw")]
    [InlineData("""
                call c; procedure p; stmt s;
                Select p such that Modifies(p, "left") and Parent(s, c)
                """,
        "Draw,Init,Main,Random,Rotate,Shrink,Translate")]
    [InlineData("""
                assign a1, a2; variable v;
                Select v such that Modifies(a1, v) and Uses(a2, v) and Follows(a1, a2)
                """,
        "I, base, dx, green, height, j, pink, radius, semi, tmp, x1, y1, y2")]
    [InlineData("""
                stmt s1, s2; call c; variable v;
                Select v such that Parent(s1, c) and Follows(s2,c) and Modifies(s2, v) with c.procName="Random"
                """,
        "distance, length, tmp, width")]
    [InlineData("""
                while w1, w2; variable v; if i;
                Select v such that Parent(w1, w2) and Modifies(w2, v) and Uses(i, v) and Parent*(i, w1)
                """,
        "area, decrement, depth, edge, green, height, incre, length, notmove, pink, pixel, semi, temporary, tmp, weight, width, x1, x2, y1, y2")]
    [InlineData("""
                stmt s; while w; variable v; call c;
                Select s such that Modifies(c, v) and Uses(c, v) and Modifies(s, v) such that Follows(s, c) and Parent(w, c)
                """,
        "12, 21, 221, 306")]
    [InlineData("""
                stmt s; while w; call c1, c2;
                Select s such that Follows*(w, s) and Parent*(w, c1) and Parent*(s, c2)
                """,
        "34, 59, 66, 234")]

    public void TestQueryMultipleBartosz(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }
}
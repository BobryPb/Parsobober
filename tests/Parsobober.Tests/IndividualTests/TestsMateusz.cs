namespace Parsobober.Tests.IndividualTests;

public class TestsMateusz() : BaseTestClass(Code.Dropbox)
{
    [Theory]
    [InlineData("if i;Select i such that Uses(i, \"k\")", "14, 15, 66, 76, 80")]
    [InlineData("while w;Select w such that Uses(w, \"area\")", "6, 12, 16, 29, 95, 105")]
    [InlineData("procedure p;Select p such that Calls(\"Main\", p)",
        "Draw, Enlarge, Init, Move, Random, Shear, Shift, Shrink, Transform, Translate")]
    [InlineData("procedure p;Select p such that Calls*(p, \"Random\")", "Draw, Main, Rotate, Shrink, Translate")]
    [InlineData("stmt s; constant c;Select s with s.stmt# = c", "1, 2, 3, 5, 8, 10, 11, 16, 20, 32, 83, 100")]
    [InlineData("Select BOOLEAN such that Uses(2, \"width\")", "false")]
    [InlineData("Select BOOLEAN such that Modifies(2, \"width\")", "true")]
    [InlineData("if i; while w;Select w such that Parent(i, w)",
        "16, 26, 69, 79, 83, 89, 113, 217, 239, 251, 256, 279, 281")]
    [InlineData("if i; while w;Select w such that Follows(i, w)", "29, 59, 95, 143, 196")]
    [InlineData("if i; while w;Select w such that Follows(w, i)", "29, 196")]
    [InlineData("stmt s;Select s such that Follows*(1, s)", "2, 3, 4, 5, 6, 119")]
    public void TestQuerySingleMateusz(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }

    [Theory]
    [InlineData("assign a;Select a such that Uses(a, \"width\") and Uses(a, \"height\")",
        "11, 32, 40, 186")]
    [InlineData("stmt s; if i; assign a;Select s such that Parent(s,i) and Parent(i,a) and Modifies(a, \"width\")",
        "79")]
    [InlineData("procedure p;Select p such that Calls*(\"Main\", p) and Uses(p, \"width\") and Uses(p, \"height\")",
        "Move")]
    [InlineData(
        "while w; stmt s; variable v;Select w such that Uses(w, v) and Parent(w, s) and Modifies(s, v) with v.varName = area",
        "6, 12, 16, 29")]
    [InlineData(
        "while w, w1, w2; variable v;Select w such that Parent(w, w1) and Parent(w1, w2) and Uses(w, v) and Uses(w1, v) and Uses(w2, v)",
        "6, 12, 180")]
    [InlineData(
        "stmt s;\nSelect s such that Modifies(s, \"width\") and Modifies(s, \"height\") and Modifies(s, \"area\")",
        "6, 12, 14, 15, 16, 29")]
    [InlineData(
        "stmt s, s1;\nSelect s such that Follows(s, s1) and Modifies(s, \"width\") and Modifies(s1, \"height\")",
        "2, 30, 82, 85")]
    [InlineData("stmt s, s1, s2;\nSelect s2 such that Follows*(1, s) and Parent(s, s1) and Follows*(s1, s2)",
        "8, 9, 10, 11, 12, 118")]
    [InlineData(
        "variable v; procedure p;\nSelect v such that Modifies(\"Main\", v) and Uses(\"Main\", v) and Calls(\"Main\", p) and Modifies(p, v) and Uses(p, v)",
        "I,asterick,base,bottom,decrement,depth,dot,dx,dy,edge,factor,green,height,incre,j,left,line,marking,notmove,pct,pink,pixel,range,right,s,semi,temporary,tmp,top,triangle,weight,x1,x2,y1,y2")]
    [InlineData(
        "stmt s; if i; while w;\nSelect s such that Parent*(s, i) and Uses(s, \"width\") and Parent(i, w) and Uses(w, \"height\")",
        "6, 12, 14, 15, 79")]
    public void TestQueryMultipleMateusz(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }
}
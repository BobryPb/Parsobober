namespace Parsobober.Tests.IndividualTests;

public class TestsPiotrek() : BaseTestClass(Code.Dropbox)
{
    [Theory]
    [InlineData("if i; while w;\nSelect w such that Follows(i, w)", "29, 59, 95, 143, 196")]
    [InlineData("if i1, i2;\nSelect i2 such that Parent*(i1, i2)",
        "15, 23, 34, 38, 51, 55, 66, 72, 76, 80, 86, 109, 160, 163, 166, 170, 173, 176, 224, 230, 237, 267")]
    [InlineData("procedure p;\nSelect p such that Calls(p, \"SS\")", "TT")]
    [InlineData("procedure p;\nSelect BOOLEAN such that Calls(\"SS\", p)", "true")]
    [InlineData("assign a;\nSelect a such that Uses(a, \"line\")", "273")]
    [InlineData("Select BOOLEAN such that Modifies(2, \"width\")", "true")]
    [InlineData("procedure p;\nSelect p such that Uses(p, \"edge\")",
        "Enlarge,Main,Rotate,Translate")]
    [InlineData("variable v;\nSelect v such that Modifies(30, v)", "width")]
    [InlineData("while w; variable v;\nSelect w such that Modifies(w, v)",
        "6, 12, 16, 26, 29, 47, 59, 69, 79, 83, 89, 95, 101, 103, 105, 113, 136, 143, 180, 181, 184, 187, 191, 196, 209, 217, 218, 234, 239, 251, 256, 264, 265, 279, 281, 289, 301")]
    [InlineData("call c; assign a;\nSelect c such that Follows*(c, a)",
        "1, 18, 22, 45, 63, 84, 114, 221, 222, 227, 259, 307")]
    public void TestQuerySinglePiotrek(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }

    [Theory]
    [InlineData("""
                variable v; stmt s; while w;
                Select v such that Parent(26, s) and Modifies(s, v) and Uses(s, v)
                """,
        "y1")]
    [InlineData("""
                variable v; assign a; while w;
                Select v such that Parent(16, a) and Modifies(a, v) and Uses(a, v)
                """,
        "difference")]
    [InlineData("""
                stmt s, s1;
                Select s such that Parent*(s, s1) and Uses(s1, "j")
                """,
        "6, 12, 14, 15, 16, 66, 69, 72, 76, 79, 80, 83, 101, 103, 250, 256")]
    [InlineData("""
                if i; while w; assign a; call c;
                Select c such that Follows(i, w) and Follows(w, a) and Follows(a, c)
                """,
        "none")]
    [InlineData("""
                if i; while w; assign a; call c; variable v;
                Select v such that Parent(i, w) and Follows*(a, c) and Follows*(c, i) and Modifies(c, v)
                """,
        "I,asterick,blue,depth,dx,dy,edge,factor,green,j,left,line,marking,notmove,p1,p2,pct,peak,pink,pixel,range,right,s,semi,temporary,total,trim,x1,x2,y1,y2")]
    [InlineData("""
                if i; while w; stmt s, s1;
                Select i such that Follows*(i, w) and Parent(s, i) and Parent(s1, s)
                """,
        "14, 23, 34, 55")]
    [InlineData("""
                while w, w1, w2; variable v; call c;
                Select v such that Parent(w, w1) and Parent(w1, w2) and Follows*(w, c) and Uses(w2, v)
                """,
        "I,area,base,bottom,c,decrement,degrees,depth,difference,dot,dx,dy,edge,green,half,height,incre,j,k,left,length,line,pink,pixel,radius,right,temporary,tmp,top,triange,triangle,weight,width,x,x1,x2,y1,y2")]
    [InlineData("""
                assign a; if i; call c;
                Select a such that Follows*(c, a) and Parent*(i, a)
                """,
        "19, 20, 21, 46, 64, 65, 85, 115, 223, 228, 229, 233")]
    [InlineData("""
                assign a; call c;
                Select a such that Follows(c, a) and Uses (a, "difference")
                """,
        "19")]
    [InlineData("""
                while w; assign a; call c; variable v;
                Select v such that Parent(w, a) and Follows*(a, c) and Modifies(c, v) and Modifies(a, v)
                """,
        "cs5,cs6,semi,x2")]
    public void TestQueryMultiplePiotrek(string query, string expected)
    {
        var result = App.Query(query);
        result.Should().Be(expected.Replace(" ", ""));
    }
}
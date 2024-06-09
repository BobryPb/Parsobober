using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Parser.Tests;

public class PqlParserTests
{
    [Fact]
    public void Select_Parent()
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
        var parser = new PqlParser(builderMock.Object);

        const string queryString = "stmt s1, s2; Select s1 such that Parent (s1, s2)";
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s1, s2;"));
        builderMock.Verify(b => b.AddSelect("s1"));
        builderMock.Verify(b => b.AddParent("s1", "s2"));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Fact]
    public void Select_ParentTransitive()
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
        var parser = new PqlParser(builderMock.Object);

        const string queryString = "stmt s1, s2; Select s1 such that Parent* (s1, s2)";
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s1, s2;"));
        builderMock.Verify(b => b.AddSelect("s1"));
        builderMock.Verify(b => b.AddParentTransitive("s1", "s2"));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Theory]
    [InlineData("s1", "s2")]
    [InlineData("\"s1\"", "s2")]
    [InlineData("s1", "\"s2\"")]
    [InlineData("\"s1\"", "\"s2\"")]
    public void Select_Modifies(string left, string right)
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
        var parser = new PqlParser(builderMock.Object);

        var queryString = $"stmt s1, s2; Select s1 such that Modifies ({left}, {right})";
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s1, s2;"));
        builderMock.Verify(b => b.AddSelect("s1"));
        builderMock.Verify(b => b.AddModifies(left, right));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Fact]
    public void Select_Follows()
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
        var parser = new PqlParser(builderMock.Object);

        const string queryString = "stmt s1, s2; Select s1 such that Follows (s1, s2)";
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s1, s2;"));
        builderMock.Verify(b => b.AddSelect("s1"));
        builderMock.Verify(b => b.AddFollows("s1", "s2"));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Fact]
    public void Select_FollowsTransitive()
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
        var parser = new PqlParser(builderMock.Object);

        const string queryString = "stmt s1, s2; Select s1 such that Follows* (s1, s2)";
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s1, s2;"));
        builderMock.Verify(b => b.AddSelect("s1"));
        builderMock.Verify(b => b.AddFollowsTransitive("s1", "s2"));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Theory]
    [InlineData("stmt s; variable v;Select s with v.varName = \"test\"", "v.varName", "\"test\"")]
    [InlineData("stmt s; procedure p;Select s with p.procName = \"test\"", "p.procName", "\"test\"")]
    [InlineData("stmt s1, s2;Select s1 with s1.stmt# = 1", "s1.stmt#", "1")]
    [InlineData("stmt s; constant c;Select s with c.value = 1", "c.value", "1")]
    [InlineData("stmt s; call c;Select s with c.procName = \"main\"", "c.procName", "\"main\"")]
    public void Select_With(string queryString, string attribute, string value)
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
        var parser = new PqlParser(builderMock.Object);

        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.With(attribute, value));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Theory]
    [InlineData("s1", "s2")]
    [InlineData("\"s1\"", "s2")]
    [InlineData("s1", "\"s2\"")]
    [InlineData("\"s1\"", "\"s2\"")]
    public void Select_Uses(string left, string right)
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
        var parser = new PqlParser(builderMock.Object);

        var queryString = $"stmt s1, s2; Select s1 such that Uses ({left}, {right})";
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s1, s2;"));
        builderMock.Verify(b => b.AddSelect("s1"));
        builderMock.Verify(b => b.AddUses(left, right));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Fact]
    public void Select_Parent_Integer()
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
        var parser = new PqlParser(builderMock.Object);

        const string queryString = "stmt s1; Select s1 such that Parent(s1, 10)";
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s1;"));
        builderMock.Verify(b => b.AddSelect("s1"));
        builderMock.Verify(b => b.AddParent("s1", "10"));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Fact]
    public void ParseSingleRelationWithMultipleWith()
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
        var parser = new PqlParser(builderMock.Object);

        const string queryString = """
                                   stmt s1;
                                   Select s1 such that Parent(s1, 10)
                                   with s1.stmt# = 5 and s1.value = 10
                                   """;
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s1;"));
        builderMock.Verify(b => b.AddSelect("s1"));
        builderMock.Verify(b => b.AddParent("s1", "10"));
        builderMock.Verify(b => b.With("s1.stmt#", "5"));
        builderMock.Verify(b => b.With("s1.value", "10"));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Fact]
    public void ParseDoubleRelation()
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
        var parser = new PqlParser(builderMock.Object);

        const string queryString = "stmt s; while w, w1; Select w such that Parent(w, w1) and Parent(w1, s)";
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s;"));
        builderMock.Verify(b => b.AddDeclaration("while w, w1;"));
        builderMock.Verify(b => b.AddSelect("w"));
        builderMock.Verify(b => b.AddParent("w", "w1"));
        builderMock.Verify(b => b.AddParent("w1", "s"));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Fact]
    public void ParseDoubleRelationWith()
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
        var parser = new PqlParser(builderMock.Object);

        const string queryString =
            "stmt s; while w, w1; Select w such that Parent(w, w1) and Parent(w1, s) with w.stmt# = 5";
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s;"));
        builderMock.Verify(b => b.AddDeclaration("while w, w1;"));
        builderMock.Verify(b => b.AddSelect("w"));
        builderMock.Verify(b => b.AddParent("w", "w1"));
        builderMock.Verify(b => b.AddParent("w1", "s"));
        builderMock.Verify(b => b.With("w.stmt#", "5"));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Fact]
    public void ParseDoubleRelationWithMultipleWith()
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
        var parser = new PqlParser(builderMock.Object);

        const string queryString = """
                                   stmt s; while w, w1;
                                   Select w such that Parent(w, w1) and Parent(w1, s)
                                   with w.stmt# = 5 and w1.stmt# = 7
                                   """;
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s;"));
        builderMock.Verify(b => b.AddDeclaration("while w, w1;"));
        builderMock.Verify(b => b.AddSelect("w"));
        builderMock.Verify(b => b.AddParent("w", "w1"));
        builderMock.Verify(b => b.AddParent("w1", "s"));
        builderMock.Verify(b => b.With("w.stmt#", "5"));
        builderMock.Verify(b => b.With("w1.stmt#", "7"));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Fact]
    public void ParseTripleRelation()
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
        var parser = new PqlParser(builderMock.Object);

        const string queryString =
            "stmt s; while w, w1, w2; Select w such that Parent(w, w1) and Parent(w1, w2) and Parent(w2, s)";
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s;"));
        builderMock.Verify(b => b.AddDeclaration("while w, w1, w2;"));
        builderMock.Verify(b => b.AddSelect("w"));
        builderMock.Verify(b => b.AddParent("w", "w1"));
        builderMock.Verify(b => b.AddParent("w1", "w2"));
        builderMock.Verify(b => b.AddParent("w2", "s"));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Fact]
    public void ParseTripleRelationWithMultipleWith()
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
        var parser = new PqlParser(builderMock.Object);

        const string queryString = """
                                   stmt s; while w, w1, w2;
                                   Select w such that Parent(w, w1) and Parent(w1, w2) and Parent(w2, s)
                                   with w.stmt# = 5 and w1.stmt# = 7 and w2.stmt# = 10
                                   """;

        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("stmt s;"));
        builderMock.Verify(b => b.AddDeclaration("while w, w1, w2;"));
        builderMock.Verify(b => b.AddSelect("w"));
        builderMock.Verify(b => b.AddParent("w", "w1"));
        builderMock.Verify(b => b.AddParent("w1", "w2"));
        builderMock.Verify(b => b.AddParent("w2", "s"));
        builderMock.Verify(b => b.With("w.stmt#", "5"));
        builderMock.Verify(b => b.With("w1.stmt#", "7"));
        builderMock.Verify(b => b.With("w2.stmt#", "10"));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Theory]
    [InlineData("variable v;", "v.varName", "test")]
    [InlineData("procedure p;", "p.procName", "test")]
    [InlineData("call c;", "c.procName", "test")]
    [InlineData("constant c;", "c.value", "1")]
    [InlineData("stmt s;", "s.stmt#", "1")]
    public void ParseWithAttribute(string declaration, string attribute, string value)
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
        var parser = new PqlParser(builderMock.Object);

        var queryString = $"""
                           {declaration}
                           Select s with {attribute} = {value}
                           """;

        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration(declaration));
        builderMock.Verify(b => b.With(attribute, value));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Theory]
    [InlineData("\"main\"", "p")]
    [InlineData("p", "\"main\"")]
    [InlineData("p", "p1")]
    [InlineData("\"main\"", "\"main\"")]
    public void Select_Calls(string left, string right)
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
        var parser = new PqlParser(builderMock.Object);

        var queryString = $"procedure p, p1; Select p such that Calls({left}, {right})";
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("procedure p, p1;"));
        builderMock.Verify(b => b.AddSelect("p"));
        builderMock.Verify(b => b.AddCalls(left, right));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Theory]
    [InlineData("\"main\"", "p")]
    [InlineData("p", "\"main\"")]
    [InlineData("p", "p1")]
    [InlineData("\"main\"", "\"main\"")]
    public void Select_CallsTransitive(string left, string right)
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
        var parser = new PqlParser(builderMock.Object);

        var queryString = $"procedure p, p1; Select p such that Calls*({left}, {right})";
        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration("procedure p, p1;"));
        builderMock.Verify(b => b.AddSelect("p"));
        builderMock.Verify(b => b.AddCallsTransitive(left, right));
        builderMock.Verify(b => b.Build(), Times.Once);
    }

    [Theory]
    [InlineData("stmt s, s1, s2;")]
    [InlineData("stmt s,s1,s2;")]
    [InlineData("procedure p, p1, p2;")]
    [InlineData("assign a, a1, a2;")]
    [InlineData("while w, w1, w2;")]
    [InlineData("if i, i1, i2;")]
    [InlineData("call c, c1, c2;")]
    [InlineData("variable v, v1, v2;")]
    [InlineData("constant c, c1, c2;")]
    [InlineData("stmtLst sl, sl1, sl2;")]
    [InlineData("prog_line n, n1, n2;")]
    public void Declarations(string declarations)
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
        var parser = new PqlParser(builderMock.Object);
        var queryString = $"{declarations} Select s such that Parent(s, s1)";

        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration(declarations));
    }

    [Theory]
    [InlineData("stmt s, s1, s2;", "s, s1, s2")]
    public void Tuple(string declarations, string tuple)
    {
        // Arrange
        var builderMock = new Mock<IQueryBuilder>();
        var queryMock = new Mock<IQueryResult>();
        var parser = new PqlParser(builderMock.Object);
        var queryString = $"{declarations} Select <{tuple}> such that Parent(s, s1)";

        builderMock.Setup(b => b.Build()).Returns(queryMock.Object);

        // Act
        var query = parser.Parse(queryString);

        // Assert
        query.Should().NotBeNull();
        query.Should().Be(queryMock.Object);
        builderMock.Verify(b => b.AddDeclaration(declarations));
        foreach (var x in tuple.Split(",").Select(x => x.Trim()))
        {
            builderMock.Verify(b => b.AddTuple(x));
        }
    }
}
using sly.lexer;

namespace Parsobober.Pql.Parser;

internal static class PqlAuxiliaryTokens
{
    public const string Integer = @"\d+";
    public const string Ident = @"([a-zA-Z][a-zA-Z\d#]*)";
    public const string Synonym = Ident;
    public const string DesignEntity = "(procedure|stmtLst|stmt|assign|call|while|if|variable|constant|prog_line)\\b";
    public const string StatementReference = $@"({Synonym}|{Integer})\b";
    public const string QuoteReference = $"\"{StatementReference}\"";
}

internal enum PqlToken
{
    [Lexeme("<")]
    LeftAngleBracket,

    [Lexeme(">")]
    RightAngleBracket,

    #region Separators

    [Lexeme(@"\(")]
    LeftParenthesis,

    [Lexeme(@"\)")]
    RightParenthesis,

    [Lexeme(",")]
    Coma,

    [Lexeme(";")]
    SemiColon,

    #endregion

    [Lexeme("=")]
    Equal,

    #region Keywords

    [Lexeme("Select")]
    Select,

    [Lexeme("Pattern|pattern")]
    Pattern,

    [Lexeme("such that")]
    SuchThat,

    [Lexeme("with")]
    With,

    [Lexeme("and")]
    And,

    [Lexeme("BOOLEAN")]
    Boolean,

    #region Relations

    // Each relation should be a separate token

    [Lexeme(@"Parent\*")]
    ParentTransitive,

    [Lexeme("Parent")]
    Parent,

    [Lexeme("Modifies")]
    Modifies,

    [Lexeme(@"Follows\*")]
    FollowsTransitive,

    [Lexeme("Follows")]
    Follows,

    [Lexeme("Uses")]
    Uses,

    [Lexeme(@"Calls\*")]
    CallsTransitive,

    [Lexeme("Calls")]
    Calls,

    [Lexeme(@"Next\*")]
    NextTransitive,

    [Lexeme("Next")]
    Next,

    #endregion

    #endregion

    [Lexeme($@"{PqlAuxiliaryTokens.Ident}\.{PqlAuxiliaryTokens.Ident}")]
    Attribute,

    #region Declarations

    [Lexeme($"{PqlAuxiliaryTokens.DesignEntity} {PqlAuxiliaryTokens.Synonym}(, ?{PqlAuxiliaryTokens.Synonym})*;")]
    Declaration,

    // Segregate Design entities?
    [Lexeme(PqlAuxiliaryTokens.DesignEntity)]
    DesignEntity,

    #endregion

    [Lexeme(PqlAuxiliaryTokens.QuoteReference)]
    QuoteReference,

    [Lexeme("""(_?"([a-zA-Z\d+\*\- ])*"_?)""")]
    PatternArgument,

    [Lexeme("_\\b")]
    Underscore,

    [Lexeme($"{PqlAuxiliaryTokens.StatementReference}")]
    Reference,

    [Lexeme(@"[ \t]+", true)]
    WhiteSpace,

    [Lexeme(@"[\n\r]+", true, true)]
    EndOfLine,
}
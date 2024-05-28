using sly.lexer;

namespace Parsobober.Pql.Parser;

internal static class PqlAuxiliaryTokens
{
    public const string Integer = @"\d+";
    public const string Ident = @"([a-zA-Z][a-zA-Z\d#]*)";
    public const string Synonym = Ident;
    public const string DesignEntity = "(procedure|stmtLst|stmt|assign|call|while|if|variable|constant|prog_line)";
    public const string Name = @"([a-zA-Z][a-zA-Z\d]*)";
    public const string StatementReference = $"({Synonym}|_|{Integer})";
    public const string EntReference = $"({Synonym}|_|\"{Ident}\")";
}

internal enum PqlToken
{
    #region Separators

    [Lexeme(@"\(")]
    LeftParenthesis,

    [Lexeme(@"\)")]
    RightParenthesis,

    [Lexeme(",")]
    Coma,

    [Lexeme(";")]
    SemiColon,
    
    [Lexeme("\"")]
    Quote,

    #endregion

    [Lexeme("=")]
    Equal,

    #region Keywords

    [Lexeme("Select")]
    Select,

    [Lexeme("such that")]
    SuchThat,

    [Lexeme("with")]
    With,

    [Lexeme("and")]
    And,

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


    #endregion

    #endregion

    [Lexeme($@"{PqlAuxiliaryTokens.Ident}\.{PqlAuxiliaryTokens.Ident}")]
    Attribute,

    #region Declarations

    [Lexeme($"{PqlAuxiliaryTokens.DesignEntity} {PqlAuxiliaryTokens.Synonym}(, {PqlAuxiliaryTokens.Synonym})*;")]
    Declaration,

    // Segregate Design entities?
    [Lexeme(PqlAuxiliaryTokens.DesignEntity)]
    DesignEntity,

    #endregion

    // How to differentiate between Reference and EntReference?
    [Lexeme($"{PqlAuxiliaryTokens.StatementReference}")]
    Reference,

    [Lexeme($"{PqlAuxiliaryTokens.EntReference}")]
    EntReference,

    [Lexeme(@"[ \t]+", true)]
    WhiteSpace,

    [Lexeme(@"[\n\r]+", true, true)]
    EndOfLine,
}
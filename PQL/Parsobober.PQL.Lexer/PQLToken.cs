using sly.lexer;

namespace Parsobober.PQL.Lexer;

public enum PQLToken
{
    [Lexeme("[0-9]+")] Integer,
    [Lexeme(@"([a-zA-Z])([a-zA-Z\d#])*")] Ident,

    // todo

    [Lexeme(@"[ \t]+", true)] WhiteSpace,
    [Lexeme(@"[\n\r]+", true, true)] EndOfLine,
}
using sly.lexer;

namespace Parsobober.Simple.Lexer;

public enum SimpleToken
{
    [Lexeme("procedure|call|while|if")] Keyword,
    [Lexeme(@"=|-|\*|\+")] Operator,
    [Lexeme(@"{|}|\(|\)|;")] Separator,

    [Lexeme("[0-9]+")] Integer,
    [Lexeme(@"([a-zA-Z])([a-zA-Z\d])*")] Name,

    [Lexeme(@"[ \t]+", true)] WhiteSpace,
    [Lexeme(@"[\n\r]+", true, true)] EndOfLine,
}
namespace Parsobober.Lexer;

public interface ILexer<TToken>
    where TToken : Enum
{
    IEnumerable<IToken<TToken>> Tokenize(string input);
}
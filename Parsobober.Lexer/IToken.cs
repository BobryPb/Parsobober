namespace Parsobober.Lexer;

public interface IToken<out TToken>
    where TToken : Enum
{
    string Value { get; }

    TToken Type { get; }
}
using Parsobober.Lexer;

namespace Parsobober.Simple.Lexer;

internal record Token(string Value, SimpleToken Type) : IToken<SimpleToken>;
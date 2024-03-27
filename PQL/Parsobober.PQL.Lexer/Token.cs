using Parsobober.Lexer;

namespace Parsobober.PQL.Lexer;

internal record Token(string Value, PQLToken Type) : IToken<PQLToken>;
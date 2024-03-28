using Parsobober.Lexer;
using Parsobober.Simple.Lexer;

namespace Parsobober.Simple.Parser;

public class SimpleParser(ILexer<SimpleToken> lexer)
{
    private ILexer<SimpleToken> _lexer = lexer;
}
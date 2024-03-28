using Microsoft.Extensions.Logging;
using Parsobober.Lexer;
using Parsobober.Shared;
using sly.lexer;

namespace Parsobober.Simple.Lexer;

internal class SlyLexerAdapter(ILogger<SlyLexerAdapter> logger) : Parsobober.Lexer.ILexer<SimpleToken>
{
    private readonly sly.lexer.ILexer<SimpleToken> _innerLexer = LexerBuilder.BuildLexer<SimpleToken>().Result;

    public IEnumerable<IToken<SimpleToken>> Tokenize(string input)
    {
        logger.LogInformation("Starting tokenization");
        var tokens = _innerLexer.Tokenize(input).Tokens.ToList();
        logger.LogInformation("Tokenized {} tokens", tokens.Count);

        return tokens
            .Where(t => !string.IsNullOrEmpty(t.Value)) // somehow we need this
            .Select<Token<SimpleToken>, IToken<SimpleToken>>(Extensions.ToSimpleToken);
    }
}
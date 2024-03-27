using Microsoft.Extensions.Logging;
using Parsobober.Lexer;
using sly.lexer;

namespace Parsobober.PQL.Lexer;

internal class SlyLexerAdapter(ILogger<SlyLexerAdapter> logger) : Parsobober.Lexer.ILexer<PQLToken>
{
    private readonly sly.lexer.ILexer<PQLToken> _innerLexer = LexerBuilder.BuildLexer<PQLToken>().Result;

    public IEnumerable<IToken<PQLToken>> Tokenize(string input)
    {
        logger.LogInformation("Starting tokenization");
        var tokens = _innerLexer.Tokenize(input).Tokens.ToList();
        logger.LogInformation("Tokenized {} tokens", tokens.Count);

        return tokens
            .Where(t => !string.IsNullOrEmpty(t.Value)) // somehow we need this
            .Select<Token<PQLToken>, IToken<PQLToken>>(Extensions.ToPQLToken);
    }
}
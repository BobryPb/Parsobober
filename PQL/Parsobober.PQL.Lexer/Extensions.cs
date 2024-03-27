namespace Parsobober.PQL.Lexer;

internal static class Extensions
{
    public static Token ToPQLToken(this sly.lexer.Token<PQLToken> token) =>
        new(token.Value, token.TokenID);
}
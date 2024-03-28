namespace Parsobober.Simple.Lexer;

internal static class Extensions
{
    public static Token ToSimpleToken(this sly.lexer.Token<SimpleToken> token) =>
        new(token.Value, token.TokenID);
}
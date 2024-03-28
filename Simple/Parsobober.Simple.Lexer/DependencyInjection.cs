using Microsoft.Extensions.DependencyInjection;
using Parsobober.Lexer;

namespace Parsobober.Simple.Lexer;

public static class DependencyInjection
{
    public static IServiceCollection AddSimpleLexer(this IServiceCollection services)
    {
        services.AddSingleton<ILexer<SimpleToken>, SlyLexerAdapter>(); // singleton?

        return services;
    }
}
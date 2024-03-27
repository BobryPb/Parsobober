using Microsoft.Extensions.DependencyInjection;
using Parsobober.Lexer;

namespace Parsobober.PQL.Lexer;

public static class DependencyInjection
{
    public static IServiceCollection AddPQLLexer(this IServiceCollection services)
    {
        services.AddSingleton<ILexer<PQLToken>, SlyLexerAdapter>(); // singleton?

        return services;
    }
}
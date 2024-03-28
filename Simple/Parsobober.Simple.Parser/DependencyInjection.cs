using Microsoft.Extensions.DependencyInjection;

namespace Parsobober.Simple.Parser;

public static class DependencyInjection
{
    public static IServiceCollection AddSimpleParser(this IServiceCollection services)
    {
        services.AddSingleton<SimpleParser>(); // singleton?

        return services;
    }
}
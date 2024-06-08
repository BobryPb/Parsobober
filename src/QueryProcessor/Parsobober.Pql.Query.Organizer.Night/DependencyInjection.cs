using Microsoft.Extensions.DependencyInjection;
using Parsobober.Pql.Query.Abstractions;

namespace Parsobober.Pql.Query.Organizer.Night;

public static class DependencyInjection
{
    public static IServiceCollection AddPqlQueries(this IServiceCollection services)
    {
        services.AddTransient<IQueryOrganizerBuilder, QueryOrganizerBuilder>();
        Pql.Query.DependencyInjection.AddPqlQueries(services);

        return services;
    }
}
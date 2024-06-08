using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Parsobober.DesignExtractor;
using Parsobober.Pkb.Ast;
using Parsobober.Pkb.Relations;
using Parsobober.Pql.Parser;
using Parsobober.Pql.Query.Organizer.Another;
using Parsobober.Simple.Lexer;
using Parsobober.Simple.Parser;

namespace Parsobober.CLI;

public class AppBuilder
{
    private readonly HostApplicationBuilder _innerBuilder = Host.CreateApplicationBuilder();

    public AppBuilder()
    {
        _innerBuilder.Services
            .AddSimpleLexer()
            .AddDesignExtractor()
            .AddSimpleParserBuilder()
            .AddAst()
            .AddRelations()
            .AddPqlQueries()
            .AddPqlParser();
    }

    public AppBuilder RemoveLogging()
    {
        _innerBuilder.Logging.ClearProviders();
        return this;
    }

    public App Build(string code)
    {
        var host = _innerBuilder.Build();
        return new App(host.Services.CreateScope(), code);
    }
}
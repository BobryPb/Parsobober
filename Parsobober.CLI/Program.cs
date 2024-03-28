using Microsoft.Extensions.Hosting;
using Parsobober.Simple.Lexer;
using Parsobober.Simple.Parser;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddSimpleLexer()
    .AddSimpleParser();

var host = builder.Build();

Console.WriteLine("Parsobober 🦫");

await host.RunAsync();
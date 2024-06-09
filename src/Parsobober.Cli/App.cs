using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Parsobober.Pql.Parser;
using Parsobober.Simple.Parser.Abstractions;

namespace Parsobober.CLI;

public class App : IDisposable
{
    private readonly IServiceScope _scope;

    public App(IServiceScope scope, string code)
    {
        _scope = scope;
        var simpleParserBuilder = scope.ServiceProvider.GetRequiredService<IParserBuilder>();
        simpleParserBuilder.BuildParser(code).Parse();
    }

    public string Query(string queryStr)
    {
#if !DEBUG
        queryStr = ReplaceDeclarationsWithRandomNames(queryStr);
#endif

        var pqlParser = _scope.ServiceProvider.GetRequiredService<PqlParser>();
        var query = pqlParser.Parse(queryStr);
        var response = query.Execute();
        return response;
    }

    public void Dispose() => _scope.Dispose();

    public static string ReplaceDeclarationsWithRandomNames(string queryStr)
    {
        var regex = new Regex(
            @"\b(?:procedure|stmtLst|stmt|assign|call|while|if|variable|constant|prog_line)\s*((?:\w+\s*,\s*)*\w+)\s*;");
        var matches = regex.Matches(queryStr);
        if (matches.Count == 0)
        {
            return queryStr;
        }

        // Extract declarations and their replacements
        var replacements = new Dictionary<string, string>();
        foreach (Match match in matches)
        {
            if (!match.Success)
            {
                continue;
            }

            var declarations = match.Groups[1].Value.Split(",").Select(s => s.Trim()).ToList();
            var newDeclarations = declarations.Select(_ => GenerateUniqueName()).ToList();

            for (int i = 0; i < declarations.Count; i++)
            {
                replacements[declarations[i]] = newDeclarations[i];
            }
        }

        // Split queryStr into parts outside and inside quotes
        var regexForSplit = new Regex(@"(\"".*?\"")|([^\""]+)");
        var segments = regexForSplit.Matches(queryStr).Cast<Match>().Select(m => m.Value).ToList();

        // Process only parts outside quotes
        for (int i = 0; i < segments.Count; i++)
        {
            // If the segment is within quotes, skip it
            if (segments[i].StartsWith("\"") && segments[i].EndsWith("\""))
            {
                continue;
            }

            // Replace variable names in this segment
            foreach (var replacement in replacements)
            {
                var oldName = replacement.Key;
                var newName = replacement.Value;

                segments[i] = Regex.Replace(segments[i], $@"\b{oldName}\b", newName);
                segments[i] = Regex.Replace(segments[i], $@"\b{oldName}\.", newName + ".");
            }
        }

        // Reassemble the query string
        return string.Join("", segments);
    }

    private static string GenerateUniqueName()
    {
        return "gen" + Guid.NewGuid().ToString("N")[..8];
    }
}
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using McpResumeTools;


var builder = Host.CreateApplicationBuilder(args);

// Important for MCP stdio
builder.Logging.ClearProviders();

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();

[McpServerToolType]
public static class ResumeTools
{
    [McpServerTool]
    [Description("Returns Snehil's technical skills from SQL Server database.")]
    public static string GetSkills()
    {
        using var db = new AppDbContext();

        var skills = db.Skills
            .Select(s => s.Name)
            .ToList();

        return string.Join(", ", skills);
    }

    [McpServerTool]
    [Description("Gets Snehil's project experience.")]
    public static string GetProjects()
    {
        return """
        1. Full-stack React + ASP.NET Core apps
        2. SQL Server + EF Core data-driven applications
        3. Frontend API integration and dashboard-style UI projects
        """;
    }

    [McpServerTool]
    [Description("Recommends next skill based on career goal.")]
    public static string RecommendNextSkill(string goal)
    {
        if (goal.ToLower().Contains("ai"))
        {
            return "OpenAI API, Microsoft.Extensions.AI, Semantic Kernel, RAG, MCP";
        }

        return "Azure, Clean Architecture, JWT Authentication, Redis, CI/CD";
    }
}
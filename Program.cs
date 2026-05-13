using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;


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
    [Description("Gets Snehil's current technical skills.")]
    public static string GetSkills()
    {
        return """
        React, TypeScript, ASP.NET Core Web API, SQL Server, Entity Framework Core
        """;
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
    [Description("Recommends the next skill based on a target career goal.")]
    public static string RecommendNextSkill(
        [Description("The career goal, for example AI .NET Developer or Full Stack Developer")]
        string goal)
    {
        if (goal.ToLower().Contains("ai"))
        {
            return """
            Recommended path:
            1. OpenAI / Azure OpenAI API
            2. Microsoft.Extensions.AI
            3. Semantic Kernel
            4. RAG with embeddings
            5. MCP tools in C#
            """;
        }

        return """
        Recommended path:
        1. Azure deployment
        2. Clean Architecture
        3. Authentication with JWT
        4. Caching with Redis
        5. CI/CD with GitHub Actions
        """;
    }
}
# McpResumeTools

A .NET 10 MCP (Model Context Protocol) server that exposes developer profile tools to AI clients such as MCP Inspector, GitHub Copilot Agent Mode, or Claude Desktop.

This project demonstrates how an AI client can call backend .NET methods through MCP and interact with SQL Server, EF Core, and OpenAI-powered tools.

---

## Features

- .NET 10 MCP Server
- MCP stdio transport
- SQL Server + Entity Framework Core
- AI-callable tools
- MCP Inspector testing
- GitHub Copilot Agent Mode testing
- OpenAI integration
- Secure API key handling using environment variables

---

## Architecture

```text
MCP Inspector / GitHub Copilot / Claude
        ↓
     MCP Client
        ↓
   JSON-RPC over stdio
        ↓
  .NET MCP Server
        ↓
 SQL Server / OpenAI API
```

---

## Tech Stack

- .NET 10
- C#
- ModelContextProtocol SDK
- SQL Server
- Entity Framework Core
- OpenAI .NET SDK
- MCP Inspector

---

## Project Structure

```text
McpResumeTools
│
├── Program.cs
├── AppDbContext.cs
├── Models
│   └── Skill.cs
│
├── .vscode
│   └── mcp.json
│
└── README.md
```

---

## NuGet Packages

```bash
dotnet add package ModelContextProtocol
dotnet add package Microsoft.Extensions.Hosting
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package OpenAI
```

---

## SQL Server Setup

```sql
CREATE DATABASE McpResumeDb;
GO

USE McpResumeDb;
GO

CREATE TABLE Skills
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL
);
GO

INSERT INTO Skills(Name)
VALUES
('React'),
('TypeScript'),
('ASP.NET Core Web API'),
('SQL Server'),
('Entity Framework Core'),
('MCP'),
('AI Integration in .NET');
GO
```

---

## Entity Model

### Models/Skill.cs

```csharp
namespace McpResumeTools.Models;

public class Skill
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
```

---

## DbContext

### AppDbContext.cs

```csharp
using Microsoft.EntityFrameworkCore;
using McpResumeTools.Models;

namespace McpResumeTools;

public class AppDbContext : DbContext
{
    public DbSet<Skill> Skills => Set<Skill>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            "Server=localhost;Database=McpResumeDb;Trusted_Connection=True;TrustServerCertificate=True;");
    }
}
```

---

## OpenAI API Key Setup

Do not hardcode your OpenAI API key in the source code.

Use an environment variable instead.

### Windows PowerShell

```powershell
setx OPENAI_API_KEY "your_api_key_here"
```

After running this command, close and reopen your terminal or Visual Studio.

### Check if the key exists

```powershell
echo $env:OPENAI_API_KEY
```

---

## Program.cs

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using McpResumeTools;
using OpenAI.Chat;
using System.ClientModel;

var builder = Host.CreateApplicationBuilder(args);

// Important for MCP stdio transport.
// Avoid writing normal logs to stdout.
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
    [Description("Gets project experience.")]
    public static string GetProjects()
    {
        return """
        1. Full-stack React + ASP.NET Core apps
        2. SQL Server + EF Core applications
        3. AI-integrated .NET projects
        4. MCP server development with .NET
        """;
    }

    [McpServerTool]
    [Description("Recommends next skill based on career goal.")]
    public static string RecommendNextSkill(string goal)
    {
        if (goal.ToLower().Contains("ai"))
        {
            return """
            Recommended AI .NET path:
            1. OpenAI API
            2. Microsoft.Extensions.AI
            3. Semantic Kernel
            4. RAG with embeddings
            5. MCP tools in C#
            """;
        }

        return """
        Recommended full-stack .NET path:
        1. Azure
        2. Clean Architecture
        3. JWT Authentication
        4. Redis
        5. CI/CD with GitHub Actions
        """;
    }

    [McpServerTool]
    [Description("Generates interview questions for a given technical skill using OpenAI.")]
    public static async Task<string> GenerateInterviewQuestions(string skill)
    {
        try
        {
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return "OpenAI API key is missing. Please set the OPENAI_API_KEY environment variable.";
            }

            var client = new ChatClient(
                model: "gpt-4o-mini",
                credential: new ApiKeyCredential(apiKey));

            var response = await client.CompleteChatAsync(
                $"""
                Generate 5 practical interview questions for {skill}.
                Keep the questions useful for a .NET / software developer interview.
                """);

            return response.Value.Content[0].Text;
        }
        catch (Exception ex)
        {
            return $"Error calling OpenAI: {ex.Message}";
        }
    }
}
```

---

## Running the Project

```bash
dotnet run
```

For publish:

```bash
dotnet publish -c Release
```

---

## MCP Inspector Testing

Run MCP Inspector:

```bash
npx @modelcontextprotocol/inspector dotnet run
```

Then:

1. Open MCP Inspector in browser
2. Select `STDIO`
3. Command:
   ```text
   dotnet
   ```
4. Arguments:
   ```text
   run
   ```
5. Click `Connect`
6. Click `List Tools`

Available tools:

| Tool | Description |
|---|---|
| get_skills | Returns skills from SQL Server |
| get_projects | Returns project experience |
| recommend_next_skill | Recommends learning path |
| generate_interview_questions | Generates AI interview questions using OpenAI |

---

## Example Tool Test

### get_skills

Expected output:

```text
React, TypeScript, ASP.NET Core Web API, SQL Server, Entity Framework Core, MCP, AI Integration in .NET
```

### generate_interview_questions

Input:

```json
{
  "skill": "ASP.NET Core"
}
```

Expected output:

```text
1. What is middleware in ASP.NET Core?
2. How does dependency injection work in ASP.NET Core?
3. What is the difference between controllers and minimal APIs?
4. How do you secure an ASP.NET Core Web API?
5. How do you handle configuration in ASP.NET Core?
```

---

## GitHub Copilot / Visual Studio MCP Config

Example `.vscode/mcp.json` or MCP server config:

```json
{
  "servers": {
    "resume-tools": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "C:\\Users\\.....\\source\\repos\\McpResumeTools\\bin\\Release\\net10.0\\publish\\McpResumeTools.dll"
      ]
    }
  }
}
```

Update the path based on your local project folder.

---

## Important: Do Not Commit Secrets

Never commit:

- OpenAI API keys
- GitHub tokens
- Azure keys
- database passwords
- `.env` files with real secrets

Use environment variables instead.

Recommended `.gitignore` additions:

```gitignore
# Secrets
.env
*.env
appsettings.Development.json
secrets.json

# Build output
bin/
obj/

# Visual Studio
.vs/
```

---

## Current Status

Completed:

- MCP server setup
- Tool discovery
- SQL Server integration
- EF Core integration
- MCP Inspector testing
- OpenAI SDK integration
- Environment variable setup for API key

Known issue:

- OpenAI API may return `insufficient_quota` if the account has no billing credits.

---

## Future Improvements

- Add resume analysis tool
- Add job description comparison tool
- Add match score calculation
- Add Semantic Kernel
- Add RAG with embeddings
- Add vector database
- Add Azure deployment
- Add React frontend dashboard

---

## Learning Outcomes

This project helped demonstrate:

- How MCP works
- How AI clients call backend tools
- How JSON-RPC over stdio works
- How .NET can expose AI-callable tools
- How EF Core can provide real data to MCP tools
- How OpenAI can be integrated into MCP tools
- How to secure API keys using environment variables

---

## Author

Snehil Kosmetty

.NET Full Stack Developer exploring MCP, AI integration, SQL Server, EF Core, and modern AI backend engineering.

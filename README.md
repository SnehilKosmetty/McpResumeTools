# McpResumeTools

A .NET 10 MCP (Model Context Protocol) server that exposes developer profile tools to AI clients such as MCP Inspector, GitHub Copilot Agent Mode, and Claude Desktop.

This project demonstrates how AI clients can invoke backend .NET tools through MCP and interact with SQL Server, EF Core, and OpenAI-powered services.

---

## Features

- .NET 10 MCP Server
- MCP stdio transport
- SQL Server + Entity Framework Core integration
- AI-callable tools
- MCP Inspector testing
- GitHub Copilot Agent Mode support
- OpenAI integration
- Dependency Injection with ASP.NET Core
- Secure API key management

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
├── appsettings.json
├── appsettings.Development.json
│
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

Install required packages:

```bash
dotnet add package ModelContextProtocol
dotnet add package Microsoft.Extensions.Hosting
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package OpenAI
```

---

## SQL Server Setup

Run the following SQL script:

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

## MCP Server Registration

```csharp
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<ResumeTools>();
```

---

## EF Core Registration

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});
```

---

## Example MCP Tool

```csharp
[McpServerTool]
[Description("Returns technical skills from SQL Server database.")]
public string GetSkills()
{
    var skills = _db.Skills
        .Select(s => s.Name)
        .ToList();

    return string.Join(", ", skills);
}
```

---

## OpenAI MCP Tool

```csharp
[McpServerTool]
[Description("Generates interview questions using OpenAI.")]
public async Task<string> GenerateInterviewQuestions(string skill)
{
    var apiKey = _configuration["OpenAI:ApiKey"];

    var client = new ChatClient(
        model: "gpt-4o-mini",
        credential: new ApiKeyCredential(apiKey));

    var response = await client.CompleteChatAsync(
        $"Generate 5 interview questions for {skill}");

    return response.Value.Content[0].Text;
}
```

---

## Environment Configuration

This project uses:

- `appsettings.json` → safe public configuration
- `appsettings.Development.json` → local secrets (ignored by Git)

---

## appsettings.json

Safe to commit to GitHub.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "OpenAI": {
    "ApiKey": ""
  }
}
```

---

## appsettings.Development.json

Do NOT commit this file.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=McpResumeDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "OpenAI": {
    "ApiKey": "YOUR_REAL_OPENAI_API_KEY"
  }
}
```

---

## .gitignore

Ensure this line exists:

```gitignore
appsettings.Development.json
```

This prevents secrets from being uploaded to GitHub.

---

## Running the Project

Run locally:

```bash
dotnet run
```

Publish release build:

```bash
dotnet publish -c Release
```

---

## MCP Inspector Testing

Start MCP Inspector:

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
5. Open `Environment Variables`
6. Add:
   ```text
   DOTNET_ENVIRONMENT=Development
   ```
7. Click `Connect`
8. Click `List Tools`

---

## Available MCP Tools

| Tool | Description |
|---|---|
| get_skills | Returns skills from SQL Server |
| get_projects | Returns project experience |
| recommend_next_skill | Recommends learning path |
| generate_interview_questions | Generates AI interview questions |

---

## Example Tool Tests

### get_skills

Expected output:

```text
React, TypeScript, ASP.NET Core Web API, SQL Server, Entity Framework Core, MCP, AI Integration in .NET
```

---

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
2. Explain dependency injection in ASP.NET Core.
3. What is the difference between controllers and minimal APIs?
4. How do you secure ASP.NET Core Web APIs?
5. How do you manage configuration in ASP.NET Core?
```

---

## Visual Studio / GitHub Copilot MCP Configuration

Example `.vscode/mcp.json`:

```json
{
  "servers": {
    "resume-tools": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "C:\\Users\\YOUR_USERNAME\\source\\repos\\McpResumeTools\\bin\\Release\\net10.0\\publish\\McpResumeTools.dll"
      ]
    }
  }
}
```

Update the DLL path based on your local machine.

---

## Security Best Practices

Never commit:

- OpenAI API keys
- GitHub tokens
- Azure secrets
- SQL passwords
- `.env` files
- `appsettings.Development.json`

Use:
- environment variables
- local development configuration
- secret managers

for sensitive information.

---

## Recommended .gitignore Additions

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
- Dependency Injection setup
- Secure configuration management

Known issue:

- OpenAI API may return `insufficient_quota` if the account has no billing credits.

---

## Future Improvements

- Resume analysis tool
- Job description comparison
- Resume-job match scoring
- Semantic Kernel integration
- RAG with embeddings
- Vector database integration
- Azure deployment
- React frontend dashboard

---

## Learning Outcomes

This project demonstrates:

- How MCP works
- How AI clients invoke backend tools
- JSON-RPC over stdio communication
- AI-callable .NET backend architecture
- EF Core + SQL Server integration
- OpenAI integration in MCP tools
- Dependency Injection in MCP servers
- Secure secret management

---

## Author

Snehil Kosmetty

.NET Full Stack Developer exploring MCP, AI integration, SQL Server, EF Core, and modern AI backend engineering.

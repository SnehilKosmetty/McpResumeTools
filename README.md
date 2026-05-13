# McpResumeTools

A modern .NET 10 MCP (Model Context Protocol) server built with ASP.NET Core, EF Core, SQL Server, and MCP Inspector integration.

This project demonstrates how AI tools like GitHub Copilot, Claude, or MCP Inspector can invoke backend .NET methods as AI-callable tools using MCP.

---

# 🚀 Features

- MCP Server built in .NET 10
- MCP stdio transport
- AI-callable tools
- SQL Server integration
- Entity Framework Core
- MCP Inspector testing
- GitHub Copilot Agent integration
- JSON-RPC based tool communication

---

# 🧠 What is MCP?

MCP (Model Context Protocol) allows AI systems to securely call backend tools/functions exposed by applications.

Architecture:

```text
AI Client / MCP Inspector
        ↓
     JSON-RPC
        ↓
   MCP Server (.NET)
        ↓
 Business Logic / Database
```

This project exposes developer profile tools such as:

- get_skills
- get_projects
- recommend_next_skill

---

# 🛠️ Tech Stack

- .NET 10
- C#
- ModelContextProtocol SDK
- SQL Server
- Entity Framework Core
- MCP Inspector
- GitHub Copilot Agent Mode

---

# 📂 Project Structure

```text
McpResumeTools
│
├── Program.cs
├── AppDbContext.cs
├── Models
│   └── Skill.cs
│
├── bin/
├── obj/
└── .vscode/
    └── mcp.json
```

---

# ⚙️ Installation

## 1. Clone Repository

```bash
git clone <your-repo-url>
cd McpResumeTools
```

---

## 2. Install NuGet Packages

```bash
dotnet add package ModelContextProtocol
dotnet add package Microsoft.Extensions.Hosting
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

---

# 🗄️ SQL Server Setup

## Create Database

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

# 🧱 Entity Model

## Models/Skill.cs

```csharp
namespace McpResumeTools.Models;

public class Skill
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
```

---

# 🗃️ DbContext

## AppDbContext.cs

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

# 🔌 MCP Server Configuration

## Program.cs

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using McpResumeTools;

var builder = Host.CreateApplicationBuilder(args);

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
        """;
    }

    [McpServerTool]
    [Description("Recommends next skill based on career goal.")]
    public static string RecommendNextSkill(string goal)
    {
        if (goal.ToLower().Contains("ai"))
        {
            return """
            OpenAI API,
            Microsoft.Extensions.AI,
            Semantic Kernel,
            RAG,
            MCP
            """;
        }

        return """
        Azure,
        JWT Authentication,
        Redis,
        CI/CD,
        Clean Architecture
        """;
    }
}
```

---

# 🧪 Running the MCP Server

## Publish Application

```bash
dotnet publish -c Release
```

---

# 🧰 MCP Inspector Setup

Run MCP Inspector:

```bash
npx @modelcontextprotocol/inspector dotnet run
```

---

# 🔍 Available MCP Tools

| Tool | Description |
|---|---|
| get_skills | Returns skills from SQL Server |
| get_projects | Returns project experience |
| recommend_next_skill | Suggests next learning path |

---

# ✅ Testing MCP Tools

Inside MCP Inspector:

1. Connect using STDIO transport
2. Command:
   ```text
   dotnet
   ```
3. Arguments:
   ```text
   run
   ```
4. Click:
   ```text
   List Tools
   ```
5. Run:
   ```text
   get_skills
   ```

Expected response:

```text
React, TypeScript, ASP.NET Core Web API, SQL Server, Entity Framework Core, MCP, AI Integration in .NET
```

---

# 🤖 GitHub Copilot Integration

Visual Studio 2022 Agent Mode can detect the MCP server using:

```json
{
  "servers": {
    "resume-tools": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "C:\\Users\\....\\source\\repos\\McpResumeTools\\bin\\Release\\net10.0\\publish\\McpResumeTools.dll"
      ]
    }
  }
}
```

---

# 📈 Future Improvements

- OpenAI integration
- Resume analysis tools
- Job description matching
- Semantic Kernel integration
- RAG implementation
- Vector database support
- Multi-agent workflows

---

# 📚 Learning Outcomes

This project demonstrates:

- MCP server creation in .NET
- AI tool architecture
- JSON-RPC communication
- AI-callable backend functions
- SQL Server integration
- EF Core integration
- Modern AI engineering concepts

---

# 🧑‍💻 Author

Snehil Kosmetty

.NET Full Stack Developer exploring AI-integrated backend engineering and MCP architecture.

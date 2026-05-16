using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Server;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using ModelContextProtocol.Server;
using OpenAI.Chat;
using System.ClientModel;
using System.ComponentModel;

namespace McpResumeTools.Tools
{

    [McpServerToolType]
    public class ResumeTools
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;

        public ResumeTools(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        [McpServerTool]
        [Description("Returns Snehil's technical skills from SQL Server database.")]
        public string GetSkills()
        {
            var skills = _db.Skills
                .Select(s => s.Name)
                .ToList();

            return string.Join(", ", skills);
        }

        [McpServerTool]
        [Description("Gets Snehil's project experience.")]
        public string GetProjects()
        {
            return """
            1. Full-stack React + ASP.NET Core apps
            2. SQL Server + EF Core data-driven applications
            3. Frontend API integration and dashboard-style UI projects
            """;
        }

        [McpServerTool]
        [Description("Recommends next skill based on career goal.")]
        public string RecommendNextSkill(string goal)
        {
            if (goal.ToLower().Contains("ai"))
            {
                return "OpenAI API, Microsoft.Extensions.AI, Semantic Kernel, RAG, MCP";
            }

            return "Azure, Clean Architecture, JWT Authentication, Redis, CI/CD";
        }

        [McpServerTool]
        [Description("Generates interview questions for a given skill.")]
        public async Task<string> GenerateInterviewQuestions(string skill)
        {
            try
            {
                var apiKey = _configuration["OpenAI:ApiKey"];

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    return "OpenAI API key is missing. Please set it in appsettings.Development.json or environment variable.";
                }

                var client = new ChatClient(
                    model: "gpt-4o-mini",
                    credential: new ApiKeyCredential(apiKey));

                var response = await client.CompleteChatAsync(
                    $"Generate 5 interview questions for {skill}. Keep them practical for software engineers.");

                return response.Value.Content[0].Text;
            }
            catch (Exception ex)
            {
                return $"Error calling OpenAI: {ex.Message}";
            }
        }

        [McpServerTool]
        [Description("Analyzes a resume using OpenAI and returns skills, strengths, weak areas, and improvement suggestions.")]
        public async Task<string> AnalyzeResume(string resumeText)
        {
            try
            {
                var apiKey = _configuration["OpenAI:ApiKey"];

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    return "OpenAI API key is missing. Please set it in appsettings.Development.json or environment variable.";
                }

                var client = new ChatClient(
                    model: "gpt-4o-mini",
                    credential: new ApiKeyCredential(apiKey));

                var prompt = $"""
                Analyze the following resume for a .NET / Full Stack Developer role.

                Return the response in this exact format:

                1. Resume Summary
                2. Detected Technical Skills
                3. Strong Areas
                4. Weak Areas
                5. Missing Skills
                6. Resume Improvement Suggestions
                7. Recommended Next Learning Path

                Resume Text:
                {resumeText}
                """;

                var response = await client.CompleteChatAsync(prompt);

                return response.Value.Content[0].Text;
            }
            catch (Exception ex)
            {
                return $"Error analyzing resume: {ex.Message}";
            }
        }
    }
}

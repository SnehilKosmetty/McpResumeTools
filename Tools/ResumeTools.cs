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
                You are an expert technical recruiter and resume reviewer.

                Analyze the following resume for a .NET Full Stack Developer role.

                Evaluate:
                - ATS friendliness
                - Technical depth
                - Cloud skills
                - Frontend/backend balance
                - Project quality
                - Missing industry-standard skills

                Return the response in the following structured format:

                1. Resume Summary
                2. ATS Compatibility Score
                3. Detected Technical Skills
                4. Frontend vs Backend Evaluation
                5. Cloud & DevOps Readiness
                6. Strong Areas
                7. Weak Areas
                8. Missing Skills
                9. Resume Improvement Suggestions
                10. Recommended Next Learning Path

                Resume:
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

        [McpServerTool]
        [Description("Compares a resume with a job description using OpenAI and returns match score, matched skills, missing skills, and suggestions.")]
        public async Task<string> CompareResumeWithJob(string resumeText, string jobDescription)
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
                You are an expert technical recruiter and resume-job matching specialist.

                Compare the following resume with the job description for a .NET Full Stack Developer role.

                Evaluate:
                - Overall job match
                - Technical skill alignment
                - Frontend/backend match
                - Cloud and DevOps match
                - Database experience match
                - AI/modern tooling relevance
                - Missing required skills
                - Interview readiness

                Return the response in the following structured format:

                1. Match Score Percentage
                2. Overall Match Summary
                3. Matched Technical Skills
                4. Missing Technical Skills
                5. Frontend Match Analysis
                6. Backend Match Analysis
                7. Database Match Analysis
                8. Cloud & DevOps Gap
                9. AI / Modern Tooling Relevance
                10. Resume Improvement Suggestions for This Job
                11. Interview Preparation Topics

                Resume:
                {resumeText}

                Job Description:
                {jobDescription}
                """;

                var response = await client.CompleteChatAsync(prompt);

                return response.Value.Content[0].Text;
            }
            catch (Exception ex)
            {
                return $"Error comparing resume with job: {ex.Message}";
            }
        }
    }
}

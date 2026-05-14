using McpResumeTools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using McpResumeTools.Tools;


var builder = Host.CreateApplicationBuilder(args);

// Important for MCP stdio
builder.Logging.ClearProviders();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<ResumeTools>();

await builder.Build().RunAsync();



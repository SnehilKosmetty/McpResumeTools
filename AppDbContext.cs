using McpResumeTools.Models;
using Microsoft.EntityFrameworkCore;

namespace McpResumeTools;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Skill> Skills => Set<Skill>();
}
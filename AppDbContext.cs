using Microsoft.EntityFrameworkCore;
using McpResumeTools.Models;

namespace McpResumeTools;

public class AppDbContext : DbContext
{
    public DbSet<Skill> Skills => Set<Skill>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            "Server=SNEHILKOSMETTY\\SQLEXPRESS;Database=McpResumeDb;Trusted_Connection=True;TrustServerCertificate=True;");
    }
}
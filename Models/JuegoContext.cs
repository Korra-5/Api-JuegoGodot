using Microsoft.EntityFrameworkCore;

namespace JuegoApi.Models;

public class JuegoContext : DbContext
{
    public JuegoContext(DbContextOptions<JuegoContext> options)
        : base(options)
    {
    }

    public DbSet<JuegoItem> TodoItems { get; set; } = null!;
}
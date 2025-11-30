using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
public class RepoContext : DbContext {
    public RepoContext (DbContextOptions options) : base(options) { }

    protected override void OnModelCreating (ModelBuilder modelBuilder) {

    }
}

public class RepositoryContextFactory : IDesignTimeDbContextFactory<RepoContext> {
    public RepoContext CreateDbContext (string[] args) {
        var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

        var builder = new DbContextOptionsBuilder<RepoContext>()
            .UseSqlServer(config
            .GetConnectionString("sqlConnection"),
            b => b.MigrationsAssembly("ApplicationAPI")
            );

        return new RepoContext(builder.Options);
    }
}

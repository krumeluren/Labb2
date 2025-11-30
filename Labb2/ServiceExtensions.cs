using Microsoft.EntityFrameworkCore;

public static class ServiceExtensions {

    public static void ConfigureSqlContext (this IServiceCollection services, IConfiguration config) {
        var connectionString = config.GetConnectionString("sqlConnection");

        services.AddDbContext<RepoContext>(options => options.UseSqlServer(connectionString,
            sqlServerOptionsAction: sqlOptions => {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);
            }));
    }
}
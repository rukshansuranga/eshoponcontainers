using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Ordering.API.Extensions;

public static class HostExtensions
{
    public static IApplicationBuilder MigrateDatabase<TContext>(this IApplicationBuilder app,Action<TContext, IServiceProvider> seeder, int? retry = 0) where TContext : DbContext
    {
        int retryForAvailability = retry.Value;


        using (var scope = app.ApplicationServices.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<TContext>>();
            var context = services.GetService<TContext>();

            try
            {
                logger.LogInformation("Migrating mssql server database");

                InvokeSeeder(seeder, context, services);
                    
                logger.LogInformation("Migrated mssqlserver database.");
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "An error occurred while migrating the postresql database");

                if(retryForAvailability < 50)
                {
                    retryForAvailability++;
                    Thread.Sleep(2000);
                    MigrateDatabase<TContext>(app, seeder, retryForAvailability);
                }
            }
        }

        return app;
    }       

    private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services)
        where TContext : DbContext
    {
        context.Database.Migrate();
        seeder(context, services);
    }
}
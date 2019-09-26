using System;
using Domain;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Persistence;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {

                var service = scope.ServiceProvider;
                try
                {
                    var dbContext = service.GetRequiredService<DataContext>();
                    var userManager = service.GetRequiredService<UserManager<AppUser>>();
                    dbContext.Database.Migrate();
                    Seed.SeedData(dbContext, userManager).Wait();
                }
                catch (Exception ex)
                {
                    var logger = service.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, " An error occured during migration");
                }
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}

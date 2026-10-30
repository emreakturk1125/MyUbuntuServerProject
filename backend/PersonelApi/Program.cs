using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PersonelApi.Data;
using PersonelApi.Services;

namespace PersonelApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();

                try
                {
                    var context = services.GetRequiredService<AppDbContext>();
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Veritabanı migration işlemi başarıyla tamamlandı.");
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Veritabanı migration işlemi sırasında hata oluştu. Lütfen PostgreSQL bağlantı ayarlarınızı (appsettings.json) kontrol edin.");
                }

                try
                {
                    var storageService = services.GetRequiredService<IStorageService>();
                    await storageService.EnsureBucketExistsAsync();
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "MinIO 'personel-cvler' bucket kontrolü/oluşturulması sırasında hata oluştu. Lütfen MinIO bağlantı ayarlarınızı (appsettings.json) kontrol edin.");
                }
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.TryAddScoped<IStorageService, MinioStorageService>();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

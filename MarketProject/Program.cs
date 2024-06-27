using Autofac;
using Autofac.Extensions.DependencyInjection;
using Market.Abstractions;
using Market.Repo;
using Market.Repositories;
using Microsoft.Extensions.FileProviders;

namespace Market
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddMemoryCache(o => o.TrackStatistics = true);
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(cb =>
            {
                cb.RegisterType<ProductRepository>().As<IProductRepository>();
            });

            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", false);
            var cfg = config.Build();

            builder.Host.ConfigureContainer<ContainerBuilder>(cb =>
            {
                cb.Register(c => new ProductContext(cfg.GetConnectionString("db"))).InstancePerDependency();
            });

            builder.Services.AddSingleton<IProductRepository, ProductRepository>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            var staticFilesPath = Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles");
            Directory.CreateDirectory(staticFilesPath);

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(staticFilesPath),
                RequestPath = "/static"
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

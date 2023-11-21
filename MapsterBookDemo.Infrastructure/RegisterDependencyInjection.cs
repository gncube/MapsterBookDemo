using MapsterBookDemo.Application.Interfaces;
using MapsterBookDemo.Configurations;
using MapsterBookDemo.Domain.Models;
using MapsterBookDemo.Infrastructure.Data;
using MapsterBookDemo.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MapsterBookDemo.Infrastructure;

public static class RegisterDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var home = Environment.GetEnvironmentVariable("HOME") ?? "";
        var databasePath = Path.Combine(home, "MapsterBookDemo.sqlite");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={databasePath}"));

        services.AddScoped<IRepository<Book, int>, BookRepository>();

        services.AddMapster();
        return services;
    }
}

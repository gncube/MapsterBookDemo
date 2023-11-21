using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace MapsterBookDemo.Application;

public static class RegisterDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(x => new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return services;
    }
}

﻿using Mapster;
using MapsterBookDemo.Application.DTOs;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MapsterBookDemo.Configurations;
public static class MapsterConfiguration
{
    /// <summary>
    /// Adds Mapster configurations to application.
    /// </summary>
    /// <param name="services">The service collection</param>
    public static void AddMapster(this IServiceCollection services)
    {
        // Get the global type adapter configuration.
        TypeAdapterConfig typeAdapterConfig = TypeAdapterConfig.GlobalSettings;

        // Scan the Application to find models for mapping based on BaseDto.
        Assembly appAssembly = typeof(BaseDto<,>).Assembly;
        typeAdapterConfig.Scan(appAssembly);
    }
}

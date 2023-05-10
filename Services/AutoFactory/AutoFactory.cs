using System.Reflection;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.AutoFactoryHelper;

namespace Services.AutoFactory;

public static class AutoFactory
{
    public static void Configure(ContainerBuilder builder, IConfiguration configuration, IServiceCollection? serviceCollection)
    {
        builder.AddInstallersFromAssemblies(configuration, Assembly.GetExecutingAssembly(), serviceCollection);
    }
}
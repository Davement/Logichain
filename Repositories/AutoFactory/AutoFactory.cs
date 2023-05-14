using System.Reflection;
using Autofac;
using Common.AutoFactoryHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Repositories.AutoFactory;

public static class AutoFactory
{
    public static void Configure(ContainerBuilder builder, IConfiguration configuration, IServiceCollection? serviceCollection)
    {
        builder.AddInstallersFromAssemblies(configuration, Assembly.GetExecutingAssembly(), serviceCollection);
    }
}
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Migrations.Database;
using Services.AutoFactoryHelper;

// ReSharper disable UnusedType.Global
namespace Migrations;

public class SetupDependencyInjection : IAutoFactoryDiInstaller
{
    public void AddServices(ContainerBuilder builder, IConfiguration configuration, IServiceCollection? serviceCollection)
    {
        builder.RegisterType<DatabaseService>().AsImplementedInterfaces().AutoActivate();
    }
}
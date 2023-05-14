using Autofac;
using Common.AutoFactoryHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Database;
using Services.MasterData;
using Services.User;

namespace Services;

public class SetupDependencyInjection : IAutoFactoryDiInstaller
{
    public void AddServices(ContainerBuilder builder, IConfiguration configuration, IServiceCollection? serviceCollection)
    {
        builder.RegisterType<DatabaseService>().AsImplementedInterfaces().AutoActivate();
        builder.RegisterType<UserService>().AsImplementedInterfaces();
        builder.RegisterType<LocationService>().AsImplementedInterfaces();
    }
}
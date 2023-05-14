using Autofac;
using Common.AutoFactoryHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repositories.MasterData;

namespace Repositories;

public class SetupDependencyInjection : IAutoFactoryDiInstaller
{
    public void AddServices(ContainerBuilder builder, IConfiguration configuration, IServiceCollection? serviceCollection)
    {
        builder.RegisterType<LocationRepository>().AsImplementedInterfaces();
    }
}
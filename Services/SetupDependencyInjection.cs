using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.AutoFactoryHelper;

namespace Services;

public class SetupDependencyInjection : IAutoFactoryDiInstaller
{
    public void AddServices(ContainerBuilder builder, IConfiguration configuration, IServiceCollection? serviceCollection)
    {
        builder.RegisterType<UserService>().AsImplementedInterfaces();
    }
}
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable UnusedParameter.Global
namespace Services.AutoFactoryHelper;

/// <summary>
/// Create a file called: SetupDependencyInjection.cs in every folder you want to register the classes. And implement this interface.
/// Use the InstallerExtensions.AddInstallersFromAssemblies in the .DLL to automatically find all classes with this interface and all classes will be executed on startup.
/// </summary>
public interface IAutoFactoryDiInstaller
{
    void AddServices(ContainerBuilder builder, IConfiguration configuration, IServiceCollection? serviceCollection);
}
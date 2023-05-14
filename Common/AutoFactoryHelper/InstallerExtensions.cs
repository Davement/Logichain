using System.Reflection;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.AutoFactoryHelper;

public static class InstallerExtensions
{
    private static Assembly[] AppDomainAssemblies { get; }

    static InstallerExtensions()
    {
        AppDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
    }

    /// <summary>
    /// Finds all classes with IAutoFactoryDiInstaller and register them
    /// </summary>
    /// <param name="container"></param>
    /// <param name="configuration"></param>
    /// <param name="executingAssembly">Pass here: Assembly.GetExecutingAssembly()</param>
    /// <param name="serviceCollection"></param>
    public static void AddInstallersFromAssemblies(this ContainerBuilder container, IConfiguration configuration, Assembly executingAssembly, IServiceCollection? serviceCollection)
    {
        var installerTypes =
            executingAssembly.DefinedTypes.Where(x => typeof(IAutoFactoryDiInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

        var installers = installerTypes.Select(Activator.CreateInstance).Cast<IAutoFactoryDiInstaller>();

        foreach (var installer in installers)
        {
            installer.AddServices(container, configuration, serviceCollection);
        }
    }

    /// <summary>
    /// An idea to register all the classes in a specific namespace .AsImplementedInterfaces();
    /// </summary>
    /// <param name="containerBuilder"></param>
    /// <param name="nameSpace"></param>
    /// <exception cref="ArgumentException"></exception>
    public static void RegisterAllAsImplementedInterfaces(this ContainerBuilder containerBuilder, string nameSpace)
    {
        var assemblyName = nameSpace.Split(".")[0];
        var assembly = AppDomainAssemblies.FirstOrDefault(x => x.GetName().Name == assemblyName);

        if (assembly == null)
        {
            throw new ArgumentException($"No assembly found in assembly: {assemblyName}");
        }

        var classesInNameSpace = assembly.GetTypes()
            .Where(x =>
                x.Namespace != null &&
                x.Namespace.Equals(nameSpace) &&
                x.IsClass &&
                x.GetInterfaces().Any(y => y == typeof(IAutoFactoryDiInstaller)) == false &&
                x.IsPublic &&
                x.IsAbstract == false
            );

        var classInNamespaces = classesInNameSpace.ToList();
        if (classInNamespaces.Any() == false)
        {
            throw new ArgumentException($"No classes found in NameSpace: {nameSpace}");
        }

        foreach (var classInNamespace in classInNamespaces)
        {
            containerBuilder.RegisterType(classInNamespace).AsImplementedInterfaces();
        }
    }
}
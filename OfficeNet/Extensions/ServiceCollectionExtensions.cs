using Microsoft.AspNetCore.Authorization;
using System.Reflection;

namespace OfficeNet.Extensions
{
    public static class ServiceCollectionExtensions
    {
        //public static void AddScopedServicesByConvention(this IServiceCollection services, Assembly assembly)
        //{
        //    var types = assembly.GetTypes();

        //    var serviceTypes = types
        //        .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service"))
        //        .Select(impl => new
        //        {
        //            Implementation = impl,
        //            Interface = impl.GetInterface($"I{impl.Name}")
        //        })
        //        .Where(t => t.Interface != null);

        //    foreach (var svc in serviceTypes)
        //    {
        //        services.AddScoped(svc.Interface, svc.Implementation);
        //    }
        //}

        public static void AddServicesByConvention(this IServiceCollection services, Assembly assembly)
        {
            var types = assembly.GetTypes();

            var serviceTypes = types
                .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service"))
                .Select(impl => new
                {
                    Implementation = impl,
                    Interface = impl.GetInterface($"I{impl.Name}"),
                    Lifetime = impl.GetCustomAttribute<ServiceLifetimeAttribute>()?.Lifetime ?? ServiceLifetimeType.Scoped
                })
                .Where(t => t.Interface != null);

            foreach (var svc in serviceTypes)
            {
                switch (svc.Lifetime)
                {
                    case ServiceLifetimeType.Singleton:
                        services.AddSingleton(svc.Interface, svc.Implementation);
                        break;
                    case ServiceLifetimeType.Transient:
                        services.AddTransient(svc.Interface, svc.Implementation);
                        break;
                    default:
                        services.AddScoped(svc.Interface, svc.Implementation);
                        break;
                }
            }

            //Added authorization handlers

            var authHandlers = types
            .Where(t => typeof(IAuthorizationHandler).IsAssignableFrom(t)
                        && t.IsClass && !t.IsAbstract);

            foreach (var handler in authHandlers)
            {
                services.AddScoped(typeof(IAuthorizationHandler), handler);
            }
        }
    }

}


[AttributeUsage(AttributeTargets.Class)]
public class ServiceLifetimeAttribute : Attribute
{
    public ServiceLifetimeType Lifetime { get; }

    public ServiceLifetimeAttribute(ServiceLifetimeType lifetime)
    {
        Lifetime = lifetime;
    }
}

public enum ServiceLifetimeType
{
    Singleton,
    Scoped,
    Transient
}
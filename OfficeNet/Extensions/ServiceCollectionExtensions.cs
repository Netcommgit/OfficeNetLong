using System.Reflection;

namespace OfficeNet.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddScopedServicesByConvention(this IServiceCollection services, Assembly assembly)
        {
            var types = assembly.GetTypes();

            var serviceTypes = types
                .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service"))
                .Select(impl => new
                {
                    Implementation = impl,
                    Interface = impl.GetInterface($"I{impl.Name}")
                })
                .Where(t => t.Interface != null);

            foreach (var svc in serviceTypes)
            {
                services.AddScoped(svc.Interface, svc.Implementation);
            }
        }
    }

}

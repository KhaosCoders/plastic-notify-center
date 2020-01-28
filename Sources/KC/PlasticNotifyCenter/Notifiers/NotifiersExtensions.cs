using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace PlasticNotifyCenter.Notifiers
{
    /// <summary>
    /// Extension class used in Startup.cs
    /// </summary>
    public static class NotifiersExtensions
    {
        /// <summary>
        /// Finds and registers all notifiers of this project as services
        /// </summary>
        /// <param name="services">dotnet core service collection</param>
        public static void AddNotifiers(this IServiceCollection services) =>
            typeof(INotifier)
                .Assembly
                .GetTypes()
                .Where(t => !t.IsInterface
                            && !t.IsAbstract
                            && typeof(INotifier).IsAssignableFrom(t)
                            && t.GetInterfaces().Any())
                .ToList()
                .ForEach(notifier =>
                    services.AddTransient(notifier.GetInterfaces().First(), notifier));
    }
}
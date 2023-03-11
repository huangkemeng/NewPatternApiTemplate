using Autofac;
using Microsoft.AspNetCore.Builder;
using System.Reflection;

namespace RenameMe.Api.Engines.Bases
{
    public static class EngineHub
    {
        public static void StartEngines<T>(T? container)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var iengineType = typeof(IEngine<T>);
            var engineTypes = currentAssembly
                ?.ExportedTypes
                .Where(e => e.GetInterfaces().Contains(iengineType) && e.IsClass && !e.IsAbstract)
                .OrderBy(e =>
                {
                    var engineStartPriority = e.GetCustomAttribute<EngineStartPriority>();
                    if (engineStartPriority == null)
                    {
                        return EngineStartPriority.DefaultPriority;
                    }
                    return engineStartPriority.Priority;
                })
                .ToArray();

            if (engineTypes != null && engineTypes.Length > 0)
            {
                foreach (var engineType in engineTypes)
                {
                    var engine = Activator.CreateInstance(engineType);
                    var configureMethod = engineType.GetMethod(nameof(IEngine<T>.Run));
                    if (configureMethod != null)
                    {
                        if (container == null)
                        {
                            configureMethod?.Invoke(engine, null);
                        }
                        else
                        {
                            configureMethod?.Invoke(engine, new[] { (object)container });
                        }
                    }
                }
            }
        }
    }
}

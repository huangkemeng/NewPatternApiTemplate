using Autofac;
using Microsoft.Extensions.Configuration;
using NewPattern.Api.Engines.Bases;
using NewPattern.Api.Infrastructure.Bases;

namespace NewPattern.Api.Engines.SettingEngine
{
    [EngineStartPriority(EngineStartPriority.DefaultPriority - 10)]
    public class SettingConfigurationEngine : IAutofacEngine
    {
        public void Run(ContainerBuilder containerBuilder)
        {
            var icontractType = typeof(ISetting);
            var infrastructureAssembly = icontractType.Assembly;
            var settingTypes = infrastructureAssembly
                ?.ExportedTypes
                .Where(e => e.GetInterfaces().Contains(icontractType) && e.IsClass && !e.IsAbstract)
                .ToArray();
            var basePath = AppContext.BaseDirectory;
            if (settingTypes != null && settingTypes.Any())
            {
                foreach (var settingType in settingTypes)
                {
                    if (settingType.GetInterfaces().Contains(typeof(IJsonFileSetting)))
                    {
                        var jsonSetting = Activator.CreateInstance(settingType)!;
                        var jsonFilePathProperty = settingType.GetProperty(nameof(IJsonFileSetting.JsonFilePath))!;
                        var jsonFilePath = (string?)jsonFilePathProperty.GetValue(jsonSetting);
                        if (jsonFilePath != null)
                        {
                            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
                            configurationBuilder.AddJsonFile(Path.Combine(basePath, jsonFilePath), true);
                            var configuration = configurationBuilder.Build();
                            configuration.Bind(jsonSetting);
                            containerBuilder.RegisterInstance(jsonSetting).As(settingType).SingleInstance();
                        }
                    }
                    else if (settingType.GetInterfaces().Contains(typeof(IStringSetting)))
                    {
                        var stringSetting = Activator.CreateInstance(settingType)!;
                        containerBuilder.RegisterInstance(stringSetting).As(settingType).SingleInstance();
                    }
                }
            }
        }
    }
}

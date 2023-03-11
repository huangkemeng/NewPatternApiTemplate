using Autofac;
using FluentValidation;
using RenameMe.Api.Engines.Bases;
using RenameMe.Api.Primary.Contracts.Bases;
using RenameMe.Api.Realization.Bases;

namespace RenameMe.Api.Engines.ValidatorEngine
{
    [EngineStartPriority(EngineStartPriority.DefaultPriority - 8)]
    public class ValidatorConfigurationEngine : IAutofacEngine
    {
        public void Run(ContainerBuilder builder)
        {
            var realizationAssembly = typeof(IRealization).Assembly;
            var icontractType = typeof(IContract<>);
            var realizationTypes = realizationAssembly
                ?.ExportedTypes
                .Where(e => e.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == icontractType) && e.IsClass && !e.IsAbstract)
                .ToArray();
            if (realizationTypes != null && realizationTypes.Any())
            {
                foreach (var realizationType in realizationTypes)
                {
                    var validatorMethod = realizationType.GetMethod("Validator");
                    if (validatorMethod != null)
                    {
                        builder.Register(context =>
                        {
                            var realizationObj = context.Resolve(realizationType);
                            var ret = Activator.CreateInstance(validatorMethod.GetParameters()[0].ParameterType)!;
                            validatorMethod?.Invoke(realizationObj, new[] { ret });
                            return ret;
                        })
                        .As(validatorMethod.GetParameters()[0].ParameterType.GetInterfaces().First(e => e.IsGenericType && e.GetGenericTypeDefinition() == typeof(IValidator<>)))
                        .InstancePerLifetimeScope();
                    }
                }
            }
        }
    }
}

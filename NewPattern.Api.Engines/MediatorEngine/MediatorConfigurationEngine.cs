using Autofac;
using FluentValidation;
using Mediator.Net;
using Mediator.Net.Autofac;
using Mediator.Net.Binding;
using RenameMe.Api.Engines.Bases;
using RenameMe.Api.Primary.Contracts.Bases;
using RenameMe.Api.Realization.Bases;

namespace RenameMe.Api.Engines.MediatorEngine
{
    [EngineStartPriority(EngineStartPriority.DefaultPriority - 5)]
    public class MediatorConfigurationEngine : IAutofacEngine
    {
        public void Run(ContainerBuilder builder)
        {
            var mediatorBuilder = new MediatorBuilder();
            var realizationAssembly = typeof(IRealization).Assembly;
            var icontractType = typeof(IContract<>);
            var realizationTypes = realizationAssembly?
                    .ExportedTypes
                    .Where(e => e.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == icontractType) && e.IsClass && !e.IsAbstract)
                    .ToArray();

            if (realizationTypes != null && realizationTypes.Any())
            {
                var messageBindings = new List<MessageBinding>();
                foreach (var realizationType in realizationTypes)
                {
                    var handler = realizationType.GetMethod("Handle");
                    if (handler != null)
                    {
                        var msgType = handler.GetParameters()[0].ParameterType.GenericTypeArguments[0];
                        messageBindings.Add(new MessageBinding(msgType, realizationType));
                    }
                }
                mediatorBuilder.RegisterHandlers(() => messageBindings);
                mediatorBuilder.ConfigureGlobalReceivePipe(c =>
                {
                    c.AddPipeSpecification(new DoValidatePipe(c.DependencyScope));
                    c.AddPipeSpecification(new EfCorePipe(c.DependencyScope));
                });
            }
            builder.RegisterMediator(mediatorBuilder);
        }
    }
}

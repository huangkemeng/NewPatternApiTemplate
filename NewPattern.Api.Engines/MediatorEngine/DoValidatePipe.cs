using FluentValidation;
using Mediator.Net;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Pipeline;
using NewPattern.Api.Primary.Contracts.Bases;
using NewPattern.Api.Realization.Bases;
using System.Runtime.ExceptionServices;

namespace NewPattern.Api.Engines.MediatorEngine
{
    public class DoValidatePipe : IPipeSpecification<IReceiveContext<IMessage>>
    {
        private readonly IDependencyScope dependencyScope;

        public DoValidatePipe(IDependencyScope dependencyScope)
        {
            this.dependencyScope = dependencyScope;
        }
        public bool ShouldExecute(IReceiveContext<IMessage> context, CancellationToken cancellationToken)
        {
            return true;
        }

        public Task BeforeExecute(IReceiveContext<IMessage> context, CancellationToken cancellationToken)
        {
            return Task.WhenAll();
        }

        public async Task Execute(IReceiveContext<IMessage> context, CancellationToken cancellationToken)
        {
            if (ShouldExecute(context, cancellationToken))
            {
                if (dependencyScope != null && context.Message != null)
                {
                    var msgType = context.Message.GetType();
                    var ivalidatorType = typeof(IValidator<>).MakeGenericType(msgType);
                    var validator = dependencyScope.Resolve(ivalidatorType) as IValidator;
                    if (validator != null && validator.CanValidateInstancesOfType(msgType))
                    {
                        var result = await validator.ValidateAsync(new ContractValidationContext(context.Message), cancellationToken);
                        if (!result.IsValid)
                        {
                            var validationMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
                            throw new BusinessException(validationMessages, BusinessExceptionTypeState.Validator);
                        }
                    }
                }
            }
        }

        public Task AfterExecute(IReceiveContext<IMessage> context, CancellationToken cancellationToken)
        {
            return Task.WhenAll();
        }

        public Task OnException(Exception ex, IReceiveContext<IMessage> context)
        {
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw ex;
        }
    }
}

using FluentValidation;
using Mediator.Net.Contracts;

namespace NewPattern.Api.Primary.Contracts.Bases
{
    public class ContractValidator<TMessage> : AbstractValidator<TMessage> where TMessage : IMessage
    {

    }
}

﻿using Mediator.Net.Contracts;

namespace NewPattern.Api.Primary.Contracts.Bases
{
    public interface IRequestContract<TRequest, TResponse> : IContract<TRequest>, IRequestHandler<TRequest, TResponse> where TRequest : class, IRequest where TResponse : class, IResponse
    {
    }
}

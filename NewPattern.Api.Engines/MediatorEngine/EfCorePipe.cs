using Mediator.Net;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Pipeline;
using Microsoft.EntityFrameworkCore;
using System.Runtime.ExceptionServices;

namespace RenameMe.Api.Engines.MediatorEngine
{
    public class EfCorePipe : IPipeSpecification<IReceiveContext<IMessage>>
    {
        private readonly DbContext dbContext;
        public EfCorePipe(IDependencyScope dependencyScope)
        {
            dbContext = dependencyScope.Resolve<DbContext>();
        }
        public bool ShouldExecute(IReceiveContext<IMessage> context, CancellationToken cancellationToken)
        {
            return true;
        }

        public Task BeforeExecute(IReceiveContext<IMessage> context, CancellationToken cancellationToken)
        {
            return Task.WhenAll();
        }

        public Task Execute(IReceiveContext<IMessage> context, CancellationToken cancellationToken)
        {
            return Task.WhenAll();
        }

        public async Task AfterExecute(IReceiveContext<IMessage> context, CancellationToken cancellationToken)
        {
            if (dbContext != null)
            {
                await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public Task OnException(Exception ex, IReceiveContext<IMessage> context)
        {
            ExceptionDispatchInfo.Capture(ex).Throw();
            throw ex;
        }
    }
}

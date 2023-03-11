using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using NewPattern.Api.Engines.Bases;
using NewPattern.Api.Infrastructure.EfCore;
using NewPattern.Api.Infrastructure.EfCore.Entities.Bases;

namespace NewPattern.Api.Engines.MongoDbEngine
{
    [EngineStartPriority(EngineStartPriority.DefaultPriority - 9)]
    public class ConfigureEfCoreEngine : IAutofacEngine
    {
        public void Run(ContainerBuilder context)
        {
            context.RegisterType<SqlDbContext>()
                   .AsSelf()
                   .As<DbContext>()
                   .InstancePerLifetimeScope();
            var idbEntityType = typeof(IEfDbEntity<>);
            var idbEntityAssembly = idbEntityType.Assembly;
            var dbEntityTypes = idbEntityAssembly
                ?.ExportedTypes
                .Where(e => e.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == idbEntityType) && e.IsClass && !e.IsAbstract)
                .ToArray();
            if (dbEntityTypes != null && dbEntityTypes.Any())
            {
                foreach (var dbEntityType in dbEntityTypes)
                {
                    context.Register(c =>
                    {
                        var dbContext = c.Resolve<DbContext>();
                        var dbsetType = typeof(DbContext).GetMethods().First(e => e.Name == nameof(DbContext.Set) && e.GetParameters().Length == 0);
                        var dbsetGenericType = dbsetType.MakeGenericMethod(dbEntityType);
                        return dbsetGenericType.Invoke(dbContext, null);
                    })
                     .As(typeof(DbSet<>).MakeGenericType(dbEntityType));
                }
            }
        }
    }

    public class UseEfCoreContextEngine : IUsingEngine
    {
        public void Run(WebApplication context)
        {
            var _lifetimeScope = ((IApplicationBuilder)context).ApplicationServices.GetAutofacRoot();
            using (var migrateScope = _lifetimeScope.BeginLifetimeScope())
            {
                var dbContext = migrateScope.Resolve<DbContext>()!;
                dbContext.Database.Migrate();
            }
        }
    }
}

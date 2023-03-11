
namespace NewPattern.Api.Infrastructure.EfCore.Entities.Bases
{
    public static class EntityDefaultValueSet
    {

        public static void DefaultMainValue<T>(this T entity) where T : IMainEntity
        {
            entity.DefaultPrimaryValue();
            entity.CreatedOn = DateTimeOffset.Now;
        }

        public static void DefaultPrimaryValue<T>(this T entity) where T : IEntityPrimary
        {
            entity.Id = Guid.NewGuid();
        }
    }
}

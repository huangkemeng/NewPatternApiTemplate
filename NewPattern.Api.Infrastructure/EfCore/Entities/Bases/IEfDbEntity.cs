using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NewPattern.Api.Infrastructure.EfCore.Entities.Bases
{
    /// <summary>
    ///  Ef实体请实现该接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEfDbEntity<T> where T : class, IEntityPrimary
    {
        public static abstract void EntityConfigure(EntityTypeBuilder<T> builder);
    }
}

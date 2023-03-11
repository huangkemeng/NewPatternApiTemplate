namespace RenameMe.Api.Infrastructure.EfCore.Entities.Bases
{
    /// <summary>
    /// 扩展表请基于本接口
    /// </summary>
    public interface IExtendedEntity : IEntityPrimary
    {
        Guid MainId { get; set; }
    }
}

namespace RenameMe.Api.Infrastructure.EfCore.Entities.Bases
{
    /// <summary>
    /// 多对多关系表请基于本接口
    /// </summary>
    public interface IRelationEntity : IEntityPrimary
    {
        Guid LeftId { get; set; }
        Guid RightId { get; set; }
    }
}

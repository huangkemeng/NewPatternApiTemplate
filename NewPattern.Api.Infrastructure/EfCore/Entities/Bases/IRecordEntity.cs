namespace RenameMe.Api.Infrastructure.EfCore.Entities.Bases
{
    /// <summary>
    /// 备份表请基于本接口
    /// </summary>
    public interface IRecordEntity : IMultipleVersion
    {
        public Guid OriginalId { get; set; }
    }
}

using NewPattern.Api.Infrastructure.Bases;

namespace NewPattern.Api.Infrastructure.EfCore
{
    public class DbSetting : IJsonFileSetting
    {
        public string ConnectionString { get; set; }
        public string JsonFilePath => "./EfCore/dbsettings.json";
    }
}

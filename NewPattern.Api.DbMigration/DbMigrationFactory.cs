using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using RenameMe.Api.Infrastructure.EfCore;

namespace RenameMe.Api.DbMigration
{
    public class DbMigrationFactory : IDesignTimeDbContextFactory<SqlDbContext>
    {
        public SqlDbContext CreateDbContext(string[] args)
        {
            var basePath = AppContext.BaseDirectory;
            var dbSetting = new DbSetting();
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile(Path.Combine(basePath, dbSetting.JsonFilePath), true);
            var configuration = configurationBuilder.Build();
            configuration.Bind(dbSetting);
            return new SqlDbContext(dbSetting);
        }
    }
}

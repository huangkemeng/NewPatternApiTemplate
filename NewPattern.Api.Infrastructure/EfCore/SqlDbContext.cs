using Microsoft.EntityFrameworkCore;
using NewPattern.Api.Infrastructure.Bases;

namespace NewPattern.Api.Infrastructure.EfCore
{
    public class SqlDbContext : DbContext
    {
        private readonly DbSetting dbSetting;

        public SqlDbContext(DbSetting dbSetting) : base()
        {
            this.dbSetting = dbSetting;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(dbSetting.ConnectionString, options =>
            {
                options.CommandTimeout(6000);
            });
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.LoadFromEntityConfigure();
        }
    }
}

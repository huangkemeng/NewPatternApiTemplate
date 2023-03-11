using Microsoft.EntityFrameworkCore;
using RenameMe.Api.Infrastructure.Bases;

namespace RenameMe.Api.Infrastructure.EfCore
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

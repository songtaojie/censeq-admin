using Censeq.Framework.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;

namespace Censeq.Framework.EntityFrameworkCore
{
    /// <summary>
    /// DbContextOptionsBuilder扩展
    /// </summary>
    public static class DbContextOptionsBuilderExtension
    {
        /// <summary>
        /// 动态选择数据库
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="optionsBuilder"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static DbContextOptionsBuilder<TContext> UseDynamicSql<TContext>(this DbContextOptionsBuilder<TContext> optionsBuilder, IConfiguration configuration)
            where TContext : DbContext
        => (DbContextOptionsBuilder<TContext>)((DbContextOptionsBuilder)optionsBuilder).UseDynamicSql(configuration,typeof(TContext));

        /// <summary>
        /// 动态选择数据库
        /// </summary>
        /// <param name="optionsBuilder"></param>
        /// <param name="configuration"></param>
        /// <exception cref="ArgumentException"></exception>
        private static DbContextOptionsBuilder UseDynamicSql(this DbContextOptionsBuilder optionsBuilder, IConfiguration configuration,Type dbContextType)
        {
            var dbType = configuration.GetConnectionString("DbType");
            var connectionString = ConnectionStringParser.ParseConnectionString(configuration.GetConnectionString(ConnectionStrings.DefaultConnectionStringName));
            switch (dbType?.ToLower())
            {
                case "mysql":
                    optionsBuilder.UseMySql(ServerVersion.AutoDetect(connectionString),opt => opt.ConfigureMigrations(dbContextType));
                    break;
                case "npgsql":
                    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                    optionsBuilder.UseNpgsql(connectionString,opt => opt.ConfigureMigrations(dbContextType));
                    break;
                default:
                    optionsBuilder.UseSqlite(connectionString, opt => opt.ConfigureMigrations(dbContextType));
                    break;
            }
            return optionsBuilder;
        }

        /// <summary>
        /// 动态选择数据库
        /// </summary>
        /// <param name="context"></param>
        /// <param name="configuration"></param>
        /// <exception cref="ArgumentException"></exception>
        public static AbpDbContextConfigurationContext UseDynamicSql<TDbContext>(this AbpDbContextConfigurationContext<TDbContext> context, IConfiguration configuration)
            where TDbContext : AbpDbContext<TDbContext>
        {
            var dbType = configuration.GetConnectionString("DbType");
            var dbContextType = typeof(TDbContext);
            switch (dbType?.ToLower())
            {
                case "mysql":
                    context.UseMySQL(opt => opt.ConfigureMigrations(dbContextType));
                    break;
                case "npgsql":
                    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                    context.UseNpgsql(opt => opt.ConfigureMigrations(dbContextType));
                    break;
                default:
                    context.UseSqlite(opt => opt.ConfigureMigrations(dbContextType));
                    break;
            }
            return context;
        }

        private static NpgsqlDbContextOptionsBuilder ConfigureMigrations(this NpgsqlDbContextOptionsBuilder builder, Type dbContextType)
        {
            return builder.MigrationsAssembly(dbContextType.Assembly.FullName)
                .MigrationsHistoryTable("ef_migrations_history",AbpCommonDbProperties.DbSchema);
        }

        private static MySqlDbContextOptionsBuilder ConfigureMigrations(this MySqlDbContextOptionsBuilder builder, Type dbContextType)
        {
            return builder.MigrationsAssembly(dbContextType.Assembly.FullName)
                .MigrationsHistoryTable("ef_migrations_history", AbpCommonDbProperties.DbSchema);
        }

        private static SqliteDbContextOptionsBuilder ConfigureMigrations(this SqliteDbContextOptionsBuilder builder, Type dbContextType)
        {
            return builder.MigrationsAssembly(dbContextType.Assembly.FullName)
                .MigrationsHistoryTable("ef_migrations_history", AbpCommonDbProperties.DbSchema);
        }
    }
}

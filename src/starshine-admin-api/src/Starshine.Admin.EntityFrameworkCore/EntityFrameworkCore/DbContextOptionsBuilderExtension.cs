using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;

namespace Starshine.Admin.EntityFrameworkCore
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
        => (DbContextOptionsBuilder<TContext>)UseDynamicSql((DbContextOptionsBuilder)optionsBuilder, configuration);

        /// <summary>
        /// 动态选择数据库
        /// </summary>
        /// <param name="optionsBuilder"></param>
        /// <param name="configuration"></param>
        /// <exception cref="ArgumentException"></exception>
        public static DbContextOptionsBuilder UseDynamicSql(this DbContextOptionsBuilder optionsBuilder, IConfiguration configuration)
        {
            var dbType = configuration.GetConnectionString("DbType");
            var connectionString = ConnectionStringParser.ParseConnectionString(configuration.GetConnectionString(ConnectionStrings.DefaultConnectionStringName));
            switch (dbType?.ToLower())
            {
                case "mysql":
                    optionsBuilder.UseMySql(ServerVersion.AutoDetect(connectionString),opt => opt.ConfigureMigrations());
                    break;
                case "npgsql":
                    optionsBuilder.UseNpgsql(connectionString,opt => opt.ConfigureMigrations());
                    break;
                default:
                    optionsBuilder.UseSqlite(connectionString, opt => opt.ConfigureMigrations());
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
        public static AbpDbContextConfigurationContext UseDynamicSql(this AbpDbContextConfigurationContext context, IConfiguration configuration)
        {
            var dbType = configuration.GetConnectionString("DbType");
            switch (dbType?.ToLower())
            {
                case "mysql":
                    context.UseMySQL(opt => opt.ConfigureMigrations());
                    break;
                case "npgsql":
                    context.UseNpgsql(opt => opt.ConfigureMigrations());
                    break;
                default:
                    context.UseSqlite(opt => opt.ConfigureMigrations());
                    break;
            }
            return context;
        }

        private static NpgsqlDbContextOptionsBuilder ConfigureMigrations(this NpgsqlDbContextOptionsBuilder builder)
        {
            return builder.MigrationsAssembly(typeof(StarshineAdminDbContext).Assembly.FullName)
                .MigrationsHistoryTable("ef_migrations_history",AbpCommonDbProperties.DbSchema);
        }

        private static MySqlDbContextOptionsBuilder ConfigureMigrations(this MySqlDbContextOptionsBuilder builder)
        {
            return builder.MigrationsAssembly(typeof(StarshineAdminDbContext).Assembly.FullName)
                .MigrationsHistoryTable("ef_migrations_history", AbpCommonDbProperties.DbSchema);
        }

        private static SqliteDbContextOptionsBuilder ConfigureMigrations(this SqliteDbContextOptionsBuilder builder)
        {
            return builder.MigrationsAssembly(typeof(StarshineAdminDbContext).Assembly.FullName)
                .MigrationsHistoryTable("ef_migrations_history", AbpCommonDbProperties.DbSchema);
        }
    }
}

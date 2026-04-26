using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Censeq.Admin.Migrations
{
    /// <inheritdoc />
    public partial class SeedTenantMenuScope : Migration
    {
        // Tenant scope menus defined in MenuDataSeedContributor (scope = 2)
        private static readonly string[] TenantMenuNames =
        [
            "system", "systemDashboard", "systemRole", "systemUser",
            "systemDept", "systemDic", "systemSettings", "systemTenantMenu"
        ];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Set scope = 2 (Tenant) for tenant-side menus (host records only, TenantId IS NULL)
            var names = string.Join(", ", TenantMenuNames.Select(n => $"'{n}'"));
            migrationBuilder.Sql(
                $"UPDATE censeq_menu SET scope = 2 WHERE tenant_id IS NULL AND name IN ({names});");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var names = string.Join(", ", TenantMenuNames.Select(n => $"'{n}'"));
            migrationBuilder.Sql(
                $"UPDATE censeq_menu SET scope = 1 WHERE tenant_id IS NULL AND name IN ({names});");
        }
    }
}

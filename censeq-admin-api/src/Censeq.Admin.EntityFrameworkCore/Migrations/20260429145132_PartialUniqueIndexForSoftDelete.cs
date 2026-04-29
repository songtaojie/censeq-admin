using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Censeq.Admin.Migrations
{
    /// <inheritdoc />
    public partial class PartialUniqueIndexForSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_censeq_menu_tenant_id_parent_id_name",
                table: "censeq_menu");

            migrationBuilder.DropIndex(
                name: "ix_censeq_menu_tenant_id_path",
                table: "censeq_menu");

            migrationBuilder.DropIndex(
                name: "ix_censeq_menu_tenant_id_route_name",
                table: "censeq_menu");

            migrationBuilder.CreateIndex(
                name: "ix_censeq_menu_tenant_id_parent_id_name",
                table: "censeq_menu",
                columns: new[] { "tenant_id", "parent_id", "name" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_censeq_menu_tenant_id_path",
                table: "censeq_menu",
                columns: new[] { "tenant_id", "path" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_censeq_menu_tenant_id_route_name",
                table: "censeq_menu",
                columns: new[] { "tenant_id", "route_name" },
                unique: true,
                filter: "is_deleted = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_censeq_menu_tenant_id_parent_id_name",
                table: "censeq_menu");

            migrationBuilder.DropIndex(
                name: "ix_censeq_menu_tenant_id_path",
                table: "censeq_menu");

            migrationBuilder.DropIndex(
                name: "ix_censeq_menu_tenant_id_route_name",
                table: "censeq_menu");

            migrationBuilder.CreateIndex(
                name: "ix_censeq_menu_tenant_id_parent_id_name",
                table: "censeq_menu",
                columns: new[] { "tenant_id", "parent_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_censeq_menu_tenant_id_path",
                table: "censeq_menu",
                columns: new[] { "tenant_id", "path" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_censeq_menu_tenant_id_route_name",
                table: "censeq_menu",
                columns: new[] { "tenant_id", "route_name" },
                unique: true);
        }
    }
}

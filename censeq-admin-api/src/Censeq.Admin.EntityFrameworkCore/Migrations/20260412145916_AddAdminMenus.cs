using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Censeq.Admin.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminMenus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "censeq_menu",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    title = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    route_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    path = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    component = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    redirect = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    icon = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    type = table.Column<byte>(type: "smallint", nullable: false),
                    sort = table.Column<int>(type: "integer", nullable: false),
                    visible = table.Column<bool>(type: "boolean", nullable: false),
                    keep_alive = table.Column<bool>(type: "boolean", nullable: false),
                    affix = table.Column<bool>(type: "boolean", nullable: false),
                    is_external = table.Column<bool>(type: "boolean", nullable: false),
                    external_url = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    is_iframe = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<bool>(type: "boolean", nullable: false),
                    authorization_mode = table.Column<byte>(type: "smallint", nullable: false),
                    remark = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    button_code = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    creation_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleter_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deletion_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_censeq_menu", x => x.id);
                    table.ForeignKey(
                        name: "fk_censeq_menu_censeq_menu_parent_id",
                        column: x => x.parent_id,
                        principalTable: "censeq_menu",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "censeq_menu_permission",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    menu_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permission_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_censeq_menu_permission", x => x.id);
                    table.ForeignKey(
                        name: "fk_censeq_menu_permission_censeq_menu_menu_id",
                        column: x => x.menu_id,
                        principalTable: "censeq_menu",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_censeq_menu_parent_id",
                table: "censeq_menu",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_censeq_menu_tenant_id_parent_id_name",
                table: "censeq_menu",
                columns: new[] { "tenant_id", "parent_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_censeq_menu_tenant_id_parent_id_sort",
                table: "censeq_menu",
                columns: new[] { "tenant_id", "parent_id", "sort" });

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

            migrationBuilder.CreateIndex(
                name: "ix_censeq_menu_permission_menu_id_permission_name",
                table: "censeq_menu_permission",
                columns: new[] { "menu_id", "permission_name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "censeq_menu_permission");

            migrationBuilder.DropTable(
                name: "censeq_menu");
        }
    }
}

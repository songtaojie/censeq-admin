using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Censeq.Admin.Migrations
{
    /// <inheritdoc />
    public partial class AddLocalizationManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "censeq_localization_culture",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    culture_name = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ui_culture_name = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    display_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    creation_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_censeq_localization_culture", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "censeq_localization_resource",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    default_culture_name = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    creation_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_censeq_localization_resource", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "censeq_localization_text",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    resource_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    culture_name = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    key = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    value = table.Column<string>(type: "text", nullable: true),
                    extra_properties = table.Column<string>(type: "text", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    creation_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_censeq_localization_text", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_censeq_localization_culture_tenant_id_culture_name",
                table: "censeq_localization_culture",
                columns: new[] { "tenant_id", "culture_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_censeq_localization_resource_name",
                table: "censeq_localization_resource",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_censeq_localization_text_tenant_id_resource_name_culture_na",
                table: "censeq_localization_text",
                columns: new[] { "tenant_id", "resource_name", "culture_name", "key" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "censeq_localization_culture");

            migrationBuilder.DropTable(
                name: "censeq_localization_resource");

            migrationBuilder.DropTable(
                name: "censeq_localization_text");
        }
    }
}

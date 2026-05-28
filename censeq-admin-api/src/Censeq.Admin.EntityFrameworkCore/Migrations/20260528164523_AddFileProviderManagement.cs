using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Censeq.Admin.Migrations
{
    /// <inheritdoc />
    public partial class AddFileProviderManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "bucket_name",
                table: "censeq_file_record",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "censeq_file_provider",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    provider = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    bucket_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    access_key = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    secret_key = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    region = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    endpoint = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    is_enable_https = table.Column<bool>(type: "boolean", nullable: false),
                    is_enable_cache = table.Column<bool>(type: "boolean", nullable: false),
                    is_enable = table.Column<bool>(type: "boolean", nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false),
                    custom_domain = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    order_no = table.Column<int>(type: "integer", nullable: false),
                    remark = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
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
                    table.PrimaryKey("pk_censeq_file_provider", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_censeq_file_provider_provider_bucket_name",
                table: "censeq_file_provider",
                columns: new[] { "provider", "bucket_name" });

            migrationBuilder.CreateIndex(
                name: "ix_censeq_file_provider_tenant_id_is_enable_is_default_order_no",
                table: "censeq_file_provider",
                columns: new[] { "tenant_id", "is_enable", "is_default", "order_no" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "censeq_file_provider");

            migrationBuilder.DropColumn(
                name: "bucket_name",
                table: "censeq_file_record");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Censeq.Admin.Migrations
{
    /// <inheritdoc />
    public partial class AddFileRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "censeq_file_record",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    owner_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    original_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    file_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    extension = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    content_type = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    relative_path = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    url = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    size = table.Column<long>(type: "bigint", nullable: false),
                    hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    category = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    is_public = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("pk_censeq_file_record", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_censeq_file_record_tenant_id_category_creation_time",
                table: "censeq_file_record",
                columns: new[] { "tenant_id", "category", "creation_time" });

            migrationBuilder.CreateIndex(
                name: "ix_censeq_file_record_url",
                table: "censeq_file_record",
                column: "url");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "censeq_file_record");
        }
    }
}

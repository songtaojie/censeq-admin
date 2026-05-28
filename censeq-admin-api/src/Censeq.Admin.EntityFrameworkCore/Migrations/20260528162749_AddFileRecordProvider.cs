using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Censeq.Admin.Migrations
{
    /// <inheritdoc />
    public partial class AddFileRecordProvider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "provider",
                table: "censeq_file_record",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "provider",
                table: "censeq_file_record");
        }
    }
}

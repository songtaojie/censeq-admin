using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Censeq.Admin.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuScope : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "scope",
                table: "censeq_menu",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "scope",
                table: "censeq_menu");
        }
    }
}

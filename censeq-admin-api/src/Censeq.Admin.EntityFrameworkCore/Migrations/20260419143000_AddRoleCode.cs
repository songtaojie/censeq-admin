using Censeq.Admin.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Censeq.Admin.Migrations
{
    [DbContext(typeof(CenseqAdminDbContext))]
    [Migration("20260419143000_AddRoleCode")]
    public partial class AddRoleCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "censeq_identity_role",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_censeq_identity_role_code",
                table: "censeq_identity_role",
                column: "code",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_censeq_identity_role_code",
                table: "censeq_identity_role");

            migrationBuilder.DropColumn(
                name: "code",
                table: "censeq_identity_role");
        }
    }
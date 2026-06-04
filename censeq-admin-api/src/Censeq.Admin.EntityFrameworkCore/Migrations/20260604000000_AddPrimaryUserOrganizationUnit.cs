using Censeq.Admin.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Censeq.Admin.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(CenseqAdminDbContext))]
    [Migration("20260604000000_AddPrimaryUserOrganizationUnit")]
    public partial class AddPrimaryUserOrganizationUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_primary",
                table: "censeq_identity_user_organization_unit",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(@"
                update censeq_identity_user_organization_unit uou
                set is_primary = true
                where uou.organization_unit_id = (
                    select first_uou.organization_unit_id
                    from censeq_identity_user_organization_unit first_uou
                    where first_uou.user_id = uou.user_id
                    order by first_uou.creation_time, first_uou.organization_unit_id
                    limit 1
                );
            ");

            migrationBuilder.CreateIndex(
                name: "ix_censeq_identity_user_organization_unit_user_id_primary",
                table: "censeq_identity_user_organization_unit",
                column: "user_id",
                unique: true,
                filter: "is_primary");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_censeq_identity_user_organization_unit_user_id_primary",
                table: "censeq_identity_user_organization_unit");

            migrationBuilder.DropColumn(
                name: "is_primary",
                table: "censeq_identity_user_organization_unit");
        }
    }
}

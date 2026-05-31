using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Censeq.Admin.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityClaimTypeOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "censeq_identity_claim_type_option",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    label = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    value = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    sort = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_censeq_identity_claim_type_option", x => x.id);
                    table.ForeignKey(
                        name: "fk_censeq_identity_claim_type_option_censeq_identity_claim_typ",
                        column: x => x.claim_type_id,
                        principalTable: "censeq_identity_claim_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_censeq_identity_claim_type_option_claim_type_id_value",
                table: "censeq_identity_claim_type_option",
                columns: new[] { "claim_type_id", "value" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "censeq_identity_claim_type_option");
        }
    }
}

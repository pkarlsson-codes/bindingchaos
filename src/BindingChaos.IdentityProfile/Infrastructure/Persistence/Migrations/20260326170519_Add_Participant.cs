using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BindingChaos.IdentityProfile.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_Participant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "participant",
                schema: "identity_profile",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Pseudonym = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_participant", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_participant_Pseudonym",
                schema: "identity_profile",
                table: "participant",
                column: "Pseudonym",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "participant",
                schema: "identity_profile");
        }
    }
}

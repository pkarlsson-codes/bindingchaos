using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BindingChaos.IdentityProfile.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "identity_profile");

            migrationBuilder.CreateTable(
                name: "identity_maps",
                schema: "identity_profile",
                columns: table => new
                {
                    Provider = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Sub = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    UserId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_identity_maps", x => new { x.Provider, x.Sub });
                });

            migrationBuilder.CreateTable(
                name: "user_trust",
                schema: "identity_profile",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    PersonhoodVerified = table.Column<bool>(type: "boolean", nullable: false),
                    TrustLevel = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_trust", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_identity_maps_UserId",
                schema: "identity_profile",
                table: "identity_maps",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "identity_maps",
                schema: "identity_profile");

            migrationBuilder.DropTable(
                name: "user_trust",
                schema: "identity_profile");
        }
    }
}

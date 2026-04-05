using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BindingChaos.IdentityProfile.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_SocietyInviteLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trust_invite_link_participant_CreatorUserId",
                schema: "identity_profile",
                table: "trust_invite_link");

            migrationBuilder.RenameColumn(
                name: "CreatorUserId",
                schema: "identity_profile",
                table: "trust_invite_link",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_trust_invite_link_CreatorUserId",
                schema: "identity_profile",
                table: "trust_invite_link",
                newName: "IX_trust_invite_link_CreatedById");

            migrationBuilder.CreateTable(
                name: "society_invite_link",
                schema: "identity_profile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "character varying(22)", maxLength: 22, nullable: false),
                    CreatedById = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    SocietyId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_society_invite_link", x => x.Id);
                    table.ForeignKey(
                        name: "FK_society_invite_link_participant_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "identity_profile",
                        principalTable: "participant",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_society_invite_link_CreatedById",
                schema: "identity_profile",
                table: "society_invite_link",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_society_invite_link_SocietyId",
                schema: "identity_profile",
                table: "society_invite_link",
                column: "SocietyId");

            migrationBuilder.CreateIndex(
                name: "IX_society_invite_link_Token",
                schema: "identity_profile",
                table: "society_invite_link",
                column: "Token",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_trust_invite_link_participant_CreatedById",
                schema: "identity_profile",
                table: "trust_invite_link",
                column: "CreatedById",
                principalSchema: "identity_profile",
                principalTable: "participant",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trust_invite_link_participant_CreatedById",
                schema: "identity_profile",
                table: "trust_invite_link");

            migrationBuilder.DropTable(
                name: "society_invite_link",
                schema: "identity_profile");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                schema: "identity_profile",
                table: "trust_invite_link",
                newName: "CreatorUserId");

            migrationBuilder.RenameIndex(
                name: "IX_trust_invite_link_CreatedById",
                schema: "identity_profile",
                table: "trust_invite_link",
                newName: "IX_trust_invite_link_CreatorUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_trust_invite_link_participant_CreatorUserId",
                schema: "identity_profile",
                table: "trust_invite_link",
                column: "CreatorUserId",
                principalSchema: "identity_profile",
                principalTable: "participant",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

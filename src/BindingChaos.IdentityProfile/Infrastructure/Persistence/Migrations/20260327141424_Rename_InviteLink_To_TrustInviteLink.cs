using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BindingChaos.IdentityProfile.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Rename_InviteLink_To_TrustInviteLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_invite_link_participant_CreatorUserId",
                schema: "identity_profile",
                table: "invite_link");

            migrationBuilder.DropPrimaryKey(
                name: "PK_invite_link",
                schema: "identity_profile",
                table: "invite_link");

            migrationBuilder.RenameTable(
                name: "invite_link",
                schema: "identity_profile",
                newName: "trust_invite_link",
                newSchema: "identity_profile");

            migrationBuilder.RenameIndex(
                name: "IX_invite_link_Token",
                schema: "identity_profile",
                table: "trust_invite_link",
                newName: "IX_trust_invite_link_Token");

            migrationBuilder.RenameIndex(
                name: "IX_invite_link_CreatorUserId",
                schema: "identity_profile",
                table: "trust_invite_link",
                newName: "IX_trust_invite_link_CreatorUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_trust_invite_link",
                schema: "identity_profile",
                table: "trust_invite_link",
                column: "Id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trust_invite_link_participant_CreatorUserId",
                schema: "identity_profile",
                table: "trust_invite_link");

            migrationBuilder.DropPrimaryKey(
                name: "PK_trust_invite_link",
                schema: "identity_profile",
                table: "trust_invite_link");

            migrationBuilder.RenameTable(
                name: "trust_invite_link",
                schema: "identity_profile",
                newName: "invite_link",
                newSchema: "identity_profile");

            migrationBuilder.RenameIndex(
                name: "IX_trust_invite_link_Token",
                schema: "identity_profile",
                table: "invite_link",
                newName: "IX_invite_link_Token");

            migrationBuilder.RenameIndex(
                name: "IX_trust_invite_link_CreatorUserId",
                schema: "identity_profile",
                table: "invite_link",
                newName: "IX_invite_link_CreatorUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_invite_link",
                schema: "identity_profile",
                table: "invite_link",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_invite_link_participant_CreatorUserId",
                schema: "identity_profile",
                table: "invite_link",
                column: "CreatorUserId",
                principalSchema: "identity_profile",
                principalTable: "participant",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

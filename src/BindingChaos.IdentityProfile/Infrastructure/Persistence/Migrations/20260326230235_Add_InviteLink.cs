using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BindingChaos.IdentityProfile.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_InviteLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "invite_link",
                schema: "identity_profile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "character varying(22)", maxLength: 22, nullable: false),
                    CreatorUserId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invite_link", x => x.Id);
                    table.ForeignKey(
                        name: "FK_invite_link_participant_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalSchema: "identity_profile",
                        principalTable: "participant",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_invite_link_CreatorUserId",
                schema: "identity_profile",
                table: "invite_link",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_invite_link_Token",
                schema: "identity_profile",
                table: "invite_link",
                column: "Token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "invite_link",
                schema: "identity_profile");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BindingChaos.Reputation.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "competence");

            migrationBuilder.CreateTable(
                name: "skill_domains",
                schema: "competence",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skill_domains", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "skill_domain_localizations",
                schema: "competence",
                columns: table => new
                {
                    locale = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    domain_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skill_domain_localizations", x => new { x.domain_id, x.locale });
                    table.ForeignKey(
                        name: "FK_skill_domain_localizations_skill_domains_domain_id",
                        column: x => x.domain_id,
                        principalSchema: "competence",
                        principalTable: "skill_domains",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "skills",
                schema: "competence",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    domain_id = table.Column<Guid>(type: "uuid", nullable: false),
                    slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skills", x => x.id);
                    table.ForeignKey(
                        name: "FK_skills_skill_domains_domain_id",
                        column: x => x.domain_id,
                        principalSchema: "competence",
                        principalTable: "skill_domains",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "skill_localizations",
                schema: "competence",
                columns: table => new
                {
                    locale = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    skill_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skill_localizations", x => new { x.skill_id, x.locale });
                    table.ForeignKey(
                        name: "FK_skill_localizations_skills_skill_id",
                        column: x => x.skill_id,
                        principalSchema: "competence",
                        principalTable: "skills",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_skill_domains_slug",
                schema: "competence",
                table: "skill_domains",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_skills_domain_id_slug",
                schema: "competence",
                table: "skills",
                columns: new[] { "domain_id", "slug" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "skill_domain_localizations",
                schema: "competence");

            migrationBuilder.DropTable(
                name: "skill_localizations",
                schema: "competence");

            migrationBuilder.DropTable(
                name: "skills",
                schema: "competence");

            migrationBuilder.DropTable(
                name: "skill_domains",
                schema: "competence");
        }
    }
}

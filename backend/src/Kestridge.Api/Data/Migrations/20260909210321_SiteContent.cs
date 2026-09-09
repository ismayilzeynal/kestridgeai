using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kestridge.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class SiteContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "site_companies",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    logo_file = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "ascii_bin")
                        .Annotation("MySql:CharSet", "ascii"),
                    hidden = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    sort_order = table.Column<uint>(type: "int unsigned", nullable: false, defaultValue: 0u),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_by = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, defaultValue: "")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_companies", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:StoreOptions", "ROW_FORMAT=DYNAMIC");

            migrationBuilder.CreateTable(
                name: "site_faq",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    question = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    answer = table.Column<string>(type: "varchar(600)", maxLength: 600, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort_order = table.Column<uint>(type: "int unsigned", nullable: false, defaultValue: 0u),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_by = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, defaultValue: "")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_faq", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:StoreOptions", "ROW_FORMAT=DYNAMIC");

            migrationBuilder.CreateTable(
                name: "site_service_highlights",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    service_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    text = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort_order = table.Column<uint>(type: "int unsigned", nullable: false, defaultValue: 0u)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_service_highlights", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:StoreOptions", "ROW_FORMAT=DYNAMIC");

            migrationBuilder.CreateTable(
                name: "site_service_steps",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    service_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    phase = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    summary = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    detail = table.Column<string>(type: "varchar(240)", maxLength: 240, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort_order = table.Column<uint>(type: "int unsigned", nullable: false, defaultValue: 0u)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_service_steps", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:StoreOptions", "ROW_FORMAT=DYNAMIC");

            migrationBuilder.CreateTable(
                name: "site_services",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    slug = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false, collation: "ascii_bin")
                        .Annotation("MySql:CharSet", "ascii"),
                    name = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tagline = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    card_label = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(400)", maxLength: 400, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    icon_name = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false, collation: "ascii_bin")
                        .Annotation("MySql:CharSet", "ascii"),
                    sort_order = table.Column<uint>(type: "int unsigned", nullable: false, defaultValue: 0u),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_by = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, defaultValue: "")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_services", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:StoreOptions", "ROW_FORMAT=DYNAMIC");

            migrationBuilder.CreateTable(
                name: "site_team",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    initials = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false, collation: "ascii_bin")
                        .Annotation("MySql:CharSet", "ascii"),
                    role = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    focus = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    photo = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false, defaultValue: "", collation: "ascii_bin")
                        .Annotation("MySql:CharSet", "ascii"),
                    sort_order = table.Column<uint>(type: "int unsigned", nullable: false, defaultValue: 0u),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_by = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, defaultValue: "")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_team", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:StoreOptions", "ROW_FORMAT=DYNAMIC");

            migrationBuilder.CreateIndex(
                name: "ix_site_companies_order",
                table: "site_companies",
                column: "sort_order");

            migrationBuilder.CreateIndex(
                name: "uk_site_companies_file",
                table: "site_companies",
                column: "logo_file",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_site_faq_order",
                table: "site_faq",
                column: "sort_order");

            migrationBuilder.CreateIndex(
                name: "ix_site_service_highlights_service",
                table: "site_service_highlights",
                columns: new[] { "service_id", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "ix_site_service_steps_service",
                table: "site_service_steps",
                columns: new[] { "service_id", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "ix_site_services_order",
                table: "site_services",
                column: "sort_order");

            migrationBuilder.CreateIndex(
                name: "uk_site_services_slug",
                table: "site_services",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_site_team_order",
                table: "site_team",
                column: "sort_order");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "site_companies");

            migrationBuilder.DropTable(
                name: "site_faq");

            migrationBuilder.DropTable(
                name: "site_service_highlights");

            migrationBuilder.DropTable(
                name: "site_service_steps");

            migrationBuilder.DropTable(
                name: "site_services");

            migrationBuilder.DropTable(
                name: "site_team");
        }
    }
}

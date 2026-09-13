using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kestridge.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdminUserManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "admin_disables",
                columns: table => new
                {
                    account_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    disabled_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    disabled_by = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, defaultValue: "")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_disables", x => x.account_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:StoreOptions", "ROW_FORMAT=DYNAMIC");

            migrationBuilder.CreateTable(
                name: "admin_enrollments",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    account_id = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    username = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true, collation: "ascii_bin")
                        .Annotation("MySql:CharSet", "ascii"),
                    display_name = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, defaultValue: "")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password_hash = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false, defaultValue: "", collation: "ascii_bin")
                        .Annotation("MySql:CharSet", "ascii"),
                    totp_secret = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, defaultValue: "", collation: "ascii_bin")
                        .Annotation("MySql:CharSet", "ascii"),
                    token_hash = table.Column<string>(type: "char(64)", nullable: true, collation: "ascii_bin")
                        .Annotation("MySql:CharSet", "ascii"),
                    token_expires_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, defaultValue: "")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_by_account_id = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    failed_attempts = table.Column<ushort>(type: "smallint unsigned", nullable: false, defaultValue: (ushort)0),
                    first_failed_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    locked_until = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_enrollments", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:StoreOptions", "ROW_FORMAT=DYNAMIC");

            migrationBuilder.CreateIndex(
                name: "uk_admin_enrollments_account",
                table: "admin_enrollments",
                column: "account_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uk_admin_enrollments_token",
                table: "admin_enrollments",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uk_admin_enrollments_username",
                table: "admin_enrollments",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin_disables");

            migrationBuilder.DropTable(
                name: "admin_enrollments");
        }
    }
}

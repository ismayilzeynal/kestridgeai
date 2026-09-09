using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kestridge.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdminPanel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "handled_at",
                table: "contact_submissions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "handled_by",
                table: "contact_submissions",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "admin_accounts",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    username = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "ascii_bin")
                        .Annotation("MySql:CharSet", "ascii"),
                    display_name = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, defaultValue: "")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password_hash = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false, collation: "ascii_bin")
                        .Annotation("MySql:CharSet", "ascii"),
                    totp_secret = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, collation: "ascii_bin")
                        .Annotation("MySql:CharSet", "ascii"),
                    totp_last_step = table.Column<ulong>(type: "bigint unsigned", nullable: false, defaultValue: 0ul),
                    disabled = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    failed_attempts = table.Column<ushort>(type: "smallint unsigned", nullable: false, defaultValue: (ushort)0),
                    first_failed_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    locked_until = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    last_login_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_accounts", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:StoreOptions", "ROW_FORMAT=DYNAMIC");

            migrationBuilder.CreateTable(
                name: "admin_sessions",
                columns: table => new
                {
                    token_hash = table.Column<string>(type: "char(64)", nullable: false, collation: "ascii_bin")
                        .Annotation("MySql:CharSet", "ascii"),
                    account_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    last_seen_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    idle_expires_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    absolute_expires_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin_sessions", x => x.token_hash);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:StoreOptions", "ROW_FORMAT=DYNAMIC");

            migrationBuilder.CreateIndex(
                name: "uk_admin_accounts_username",
                table: "admin_accounts",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_admin_sessions_account",
                table: "admin_sessions",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_admin_sessions_expiry",
                table: "admin_sessions",
                column: "absolute_expires_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin_accounts");

            migrationBuilder.DropTable(
                name: "admin_sessions");

            migrationBuilder.DropColumn(
                name: "handled_at",
                table: "contact_submissions");

            migrationBuilder.DropColumn(
                name: "handled_by",
                table: "contact_submissions");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kestridge.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "contact_submissions",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(254)", maxLength: 254, nullable: false, collation: "utf8mb4_0900_as_cs")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    company = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, defaultValue: "")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false, defaultValue: "")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    service = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false, collation: "ascii_bin")
                        .Annotation("MySql:CharSet", "ascii"),
                    message = table.Column<string>(type: "varchar(5000)", maxLength: 5000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dedupe_key = table.Column<string>(type: "char(64)", nullable: false, collation: "ascii_bin")
                        .Annotation("MySql:CharSet", "ascii"),
                    legal_hold = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    purge_after = table.Column<DateOnly>(type: "date", nullable: false),
                    notify_state = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: false, defaultValue: "pending", collation: "ascii_bin")
                        .Annotation("MySql:CharSet", "ascii"),
                    notify_attempts = table.Column<byte>(type: "tinyint unsigned", nullable: false, defaultValue: (byte)0),
                    notify_next_attempt_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    notified_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    notify_error = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false, defaultValue: "")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contact_submissions", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:StoreOptions", "ROW_FORMAT=DYNAMIC");

            migrationBuilder.CreateTable(
                name: "dsr_log",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    received_on = table.Column<DateOnly>(type: "date", nullable: false),
                    request_type = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: false, collation: "ascii_bin")
                        .Annotation("MySql:CharSet", "ascii"),
                    subject_email_hash = table.Column<string>(type: "char(64)", nullable: false, collation: "ascii_bin")
                        .Annotation("MySql:CharSet", "ascii"),
                    rows_affected = table.Column<uint>(type: "int unsigned", nullable: false, defaultValue: 0u),
                    affected_ids = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, defaultValue: "", collation: "ascii_bin")
                        .Annotation("MySql:CharSet", "ascii"),
                    handled_by = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    closed_on = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dsr_log", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:StoreOptions", "ROW_FORMAT=DYNAMIC");

            migrationBuilder.CreateTable(
                name: "job_runs",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    job_name = table.Column<string>(type: "varchar(48)", maxLength: 48, nullable: false, collation: "ascii_bin")
                        .Annotation("MySql:CharSet", "ascii"),
                    started_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    finished_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    outcome = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: false, defaultValue: "running", collation: "ascii_bin")
                        .Annotation("MySql:CharSet", "ascii"),
                    cutoff_date = table.Column<DateOnly>(type: "date", nullable: true),
                    rows_affected = table.Column<uint>(type: "int unsigned", nullable: false, defaultValue: 0u),
                    duration_ms = table.Column<uint>(type: "int unsigned", nullable: false, defaultValue: 0u),
                    detail = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false, defaultValue: "")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_runs", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("MySql:StoreOptions", "ROW_FORMAT=DYNAMIC");

            migrationBuilder.CreateIndex(
                name: "ix_submissions_email",
                table: "contact_submissions",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "ix_submissions_notify",
                table: "contact_submissions",
                columns: new[] { "notify_state", "notify_next_attempt_at" });

            migrationBuilder.CreateIndex(
                name: "ix_submissions_purge",
                table: "contact_submissions",
                columns: new[] { "legal_hold", "purge_after" });

            migrationBuilder.CreateIndex(
                name: "uk_submissions_dedupe",
                table: "contact_submissions",
                column: "dedupe_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_dsr_hash",
                table: "dsr_log",
                column: "subject_email_hash");

            migrationBuilder.CreateIndex(
                name: "ix_dsr_received",
                table: "dsr_log",
                column: "received_on");

            migrationBuilder.CreateIndex(
                name: "ix_job_runs_name_time",
                table: "job_runs",
                columns: new[] { "job_name", "started_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contact_submissions");

            migrationBuilder.DropTable(
                name: "dsr_log");

            migrationBuilder.DropTable(
                name: "job_runs");
        }
    }
}

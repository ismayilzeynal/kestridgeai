using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Kestridge.Api.Data;

public sealed class KestridgeDbContext(DbContextOptions<KestridgeDbContext> options) : DbContext(options)
{
    public DbSet<ContactSubmission> ContactSubmissions => Set<ContactSubmission>();
    public DbSet<JobRun> JobRuns => Set<JobRun>();
    public DbSet<DsrLogEntry> DsrLog => Set<DsrLogEntry>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<ContactSubmission>(e =>
        {
            e.ToTable("contact_submissions");
            // Pinned rather than inherited: a restore onto a server whose
            // innodb_default_row_format is COMPACT drops the index key limit
            // from 3072 to 767 bytes and the schema stops being creatable.
            e.HasTableOption("ROW_FORMAT", "DYNAMIC");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id").HasColumnType("bigint unsigned").ValueGeneratedOnAdd();
            e.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime(6)").IsRequired();
            e.Property(x => x.Name).HasColumnName("name").HasMaxLength(200).IsRequired();

            // Accent-sensitive and case-sensitive on purpose. Under the schema
            // default (utf8mb4_0900_ai_ci) a DSR delete for jose@x.com would
            // also delete jose with an accent, a different person's mailbox.
            // Case handling is done by lowercasing in the application instead.
            e.Property(x => x.Email).HasColumnName("email").HasMaxLength(254)
                .UseCollation("utf8mb4_0900_as_cs").IsRequired();

            e.Property(x => x.Company).HasColumnName("company").HasMaxLength(200).IsRequired().HasDefaultValue("");
            e.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(64).IsRequired().HasDefaultValue("");
            e.Property(x => x.Service).HasColumnName("service").HasMaxLength(32)
                .HasCharSet("ascii").UseCollation("ascii_bin").IsRequired();
            e.Property(x => x.Message).HasColumnName("message").HasMaxLength(5000).IsRequired();
            e.Property(x => x.DedupeKey).HasColumnName("dedupe_key").HasColumnType("char(64)")
                .HasCharSet("ascii").UseCollation("ascii_bin").IsRequired();

            e.Property(x => x.LegalHold).HasColumnName("legal_hold").HasColumnType("tinyint(1)")
                .IsRequired().HasDefaultValue(false);
            e.Property(x => x.PurgeAfter).HasColumnName("purge_after").HasColumnType("date").IsRequired();

            e.Property(x => x.NotifyState).HasColumnName("notify_state").HasMaxLength(16)
                .HasCharSet("ascii").UseCollation("ascii_bin")
                .HasConversion(
                    v => v == NotifyState.Pending ? "pending" : v == NotifyState.Sent ? "sent" : "failed",
                    v => v == "pending" ? NotifyState.Pending : v == "sent" ? NotifyState.Sent : NotifyState.Failed)
                .IsRequired().HasDefaultValue(NotifyState.Pending);
            e.Property(x => x.NotifyAttempts).HasColumnName("notify_attempts")
                .HasColumnType("tinyint unsigned").IsRequired().HasDefaultValue((byte)0);
            e.Property(x => x.NotifyNextAttemptAt).HasColumnName("notify_next_attempt_at").HasColumnType("datetime(6)");
            e.Property(x => x.NotifiedAt).HasColumnName("notified_at").HasColumnType("datetime(6)");
            e.Property(x => x.NotifyError).HasColumnName("notify_error").HasMaxLength(300)
                .IsRequired().HasDefaultValue("");

            e.HasIndex(x => x.DedupeKey).IsUnique().HasDatabaseName("uk_submissions_dedupe");
            e.HasIndex(x => x.Email).HasDatabaseName("ix_submissions_email");
            e.HasIndex(x => new { x.LegalHold, x.PurgeAfter }).HasDatabaseName("ix_submissions_purge");
            e.HasIndex(x => new { x.NotifyState, x.NotifyNextAttemptAt }).HasDatabaseName("ix_submissions_notify");
        });

        b.Entity<JobRun>(e =>
        {
            e.ToTable("job_runs");
            e.HasTableOption("ROW_FORMAT", "DYNAMIC");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id").HasColumnType("bigint unsigned").ValueGeneratedOnAdd();
            e.Property(x => x.JobName).HasColumnName("job_name").HasMaxLength(48)
                .HasCharSet("ascii").UseCollation("ascii_bin").IsRequired();
            e.Property(x => x.StartedAt).HasColumnName("started_at").HasColumnType("datetime(6)").IsRequired();
            e.Property(x => x.FinishedAt).HasColumnName("finished_at").HasColumnType("datetime(6)");
            e.Property(x => x.Outcome).HasColumnName("outcome").HasMaxLength(16)
                .HasCharSet("ascii").UseCollation("ascii_bin").IsRequired().HasDefaultValue("running");
            e.Property(x => x.CutoffDate).HasColumnName("cutoff_date").HasColumnType("date");
            e.Property(x => x.RowsAffected).HasColumnName("rows_affected").HasColumnType("int unsigned")
                .IsRequired().HasDefaultValue(0u);
            e.Property(x => x.DurationMs).HasColumnName("duration_ms").HasColumnType("int unsigned")
                .IsRequired().HasDefaultValue(0u);
            e.Property(x => x.Detail).HasColumnName("detail").HasMaxLength(300).IsRequired().HasDefaultValue("");

            e.HasIndex(x => new { x.JobName, x.StartedAt }).HasDatabaseName("ix_job_runs_name_time");
        });

        b.Entity<DsrLogEntry>(e =>
        {
            e.ToTable("dsr_log");
            e.HasTableOption("ROW_FORMAT", "DYNAMIC");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id").HasColumnType("bigint unsigned").ValueGeneratedOnAdd();
            e.Property(x => x.ReceivedOn).HasColumnName("received_on").HasColumnType("date").IsRequired();
            e.Property(x => x.RequestType).HasColumnName("request_type").HasMaxLength(16)
                .HasCharSet("ascii").UseCollation("ascii_bin").IsRequired();
            e.Property(x => x.SubjectEmailHash).HasColumnName("subject_email_hash").HasColumnType("char(64)")
                .HasCharSet("ascii").UseCollation("ascii_bin").IsRequired();
            e.Property(x => x.RowsAffected).HasColumnName("rows_affected").HasColumnType("int unsigned")
                .IsRequired().HasDefaultValue(0u);
            e.Property(x => x.AffectedIds).HasColumnName("affected_ids").HasMaxLength(500)
                .HasCharSet("ascii").UseCollation("ascii_bin").IsRequired().HasDefaultValue("");
            e.Property(x => x.HandledBy).HasColumnName("handled_by").HasMaxLength(64).IsRequired();
            e.Property(x => x.ClosedOn).HasColumnName("closed_on").HasColumnType("date");

            e.HasIndex(x => x.SubjectEmailHash).HasDatabaseName("ix_dsr_hash");
            e.HasIndex(x => x.ReceivedOn).HasDatabaseName("ix_dsr_received");
        });

        // MySqlConnector hands back Unspecified. Without this every read value
        // is one ToUniversalTime() away from a local-offset error, which on a
        // UTC+4 machine is a four-hour shift in the retention cutoff.
        var utc = new ValueConverter<DateTime, DateTime>(
            v => v,
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        var utcNullable = new ValueConverter<DateTime?, DateTime?>(
            v => v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null);

        foreach (var entity in b.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(utc);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(utcNullable);
                }
            }
        }
    }
}

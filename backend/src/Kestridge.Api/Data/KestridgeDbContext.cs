using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Kestridge.Api.Data;

public sealed class KestridgeDbContext(DbContextOptions<KestridgeDbContext> options) : DbContext(options)
{
    public DbSet<ContactSubmission> ContactSubmissions => Set<ContactSubmission>();
    public DbSet<JobRun> JobRuns => Set<JobRun>();
    public DbSet<DsrLogEntry> DsrLog => Set<DsrLogEntry>();
    public DbSet<AdminAccount> AdminAccounts => Set<AdminAccount>();
    public DbSet<AdminSession> AdminSessions => Set<AdminSession>();
    public DbSet<SiteFaq> SiteFaq => Set<SiteFaq>();
    public DbSet<SiteTeamMember> SiteTeam => Set<SiteTeamMember>();
    public DbSet<SiteCompany> SiteCompanies => Set<SiteCompany>();
    public DbSet<SiteService> SiteServices => Set<SiteService>();
    public DbSet<SiteServiceStep> SiteServiceSteps => Set<SiteServiceStep>();
    public DbSet<SiteServiceHighlight> SiteServiceHighlights => Set<SiteServiceHighlight>();

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

            e.Property(x => x.HandledAt).HasColumnName("handled_at").HasColumnType("datetime(6)");
            e.Property(x => x.HandledBy).HasColumnName("handled_by").HasMaxLength(64)
                .IsRequired().HasDefaultValue("");

            e.HasIndex(x => x.DedupeKey).IsUnique().HasDatabaseName("uk_submissions_dedupe");
            e.HasIndex(x => x.Email).HasDatabaseName("ix_submissions_email");
            e.HasIndex(x => new { x.LegalHold, x.PurgeAfter }).HasDatabaseName("ix_submissions_purge");
            e.HasIndex(x => new { x.NotifyState, x.NotifyNextAttemptAt }).HasDatabaseName("ix_submissions_notify");
        });

        // Both admin blocks sit above the DateTime converter loop at the bottom
        // of this method. Below it, their datetime(6) columns come back as
        // DateTimeKind.Unspecified, which on a UTC+4 machine is a four hour
        // error in session expiry.
        b.Entity<AdminAccount>(e =>
        {
            e.ToTable("admin_accounts");
            e.HasTableOption("ROW_FORMAT", "DYNAMIC");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id").HasColumnType("bigint unsigned").ValueGeneratedOnAdd();
            e.Property(x => x.Username).HasColumnName("username").HasMaxLength(64)
                .HasCharSet("ascii").UseCollation("ascii_bin").IsRequired();
            e.Property(x => x.DisplayName).HasColumnName("display_name").HasMaxLength(64)
                .IsRequired().HasDefaultValue("");
            e.Property(x => x.PasswordHash).HasColumnName("password_hash").HasMaxLength(256)
                .HasCharSet("ascii").UseCollation("ascii_bin").IsRequired();
            e.Property(x => x.TotpSecret).HasColumnName("totp_secret").HasMaxLength(64)
                .HasCharSet("ascii").UseCollation("ascii_bin").IsRequired();
            e.Property(x => x.TotpLastStep).HasColumnName("totp_last_step")
                .HasColumnType("bigint unsigned").IsRequired().HasDefaultValue(0UL);
            e.Property(x => x.Disabled).HasColumnName("disabled").HasColumnType("tinyint(1)")
                .IsRequired().HasDefaultValue(false);
            e.Property(x => x.FailedAttempts).HasColumnName("failed_attempts")
                .HasColumnType("smallint unsigned").IsRequired().HasDefaultValue((ushort)0);
            e.Property(x => x.FirstFailedAt).HasColumnName("first_failed_at").HasColumnType("datetime(6)");
            e.Property(x => x.LockedUntil).HasColumnName("locked_until").HasColumnType("datetime(6)");
            e.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime(6)").IsRequired();
            e.Property(x => x.LastLoginAt).HasColumnName("last_login_at").HasColumnType("datetime(6)");

            e.HasIndex(x => x.Username).IsUnique().HasDatabaseName("uk_admin_accounts_username");
        });

        b.Entity<AdminSession>(e =>
        {
            e.ToTable("admin_sessions");
            e.HasTableOption("ROW_FORMAT", "DYNAMIC");

            // The hash is the key. ValueGeneratedNever stops EF treating a
            // string primary key as something it should populate.
            e.HasKey(x => x.TokenHash);
            e.Property(x => x.TokenHash).HasColumnName("token_hash").HasColumnType("char(64)")
                .HasCharSet("ascii").UseCollation("ascii_bin").ValueGeneratedNever().IsRequired();

            e.Property(x => x.AccountId).HasColumnName("account_id").HasColumnType("bigint unsigned").IsRequired();
            e.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime(6)").IsRequired();
            e.Property(x => x.LastSeenAt).HasColumnName("last_seen_at").HasColumnType("datetime(6)").IsRequired();
            e.Property(x => x.IdleExpiresAt).HasColumnName("idle_expires_at").HasColumnType("datetime(6)").IsRequired();
            e.Property(x => x.AbsoluteExpiresAt).HasColumnName("absolute_expires_at").HasColumnType("datetime(6)").IsRequired();

            e.HasIndex(x => x.AccountId).HasDatabaseName("ix_admin_sessions_account");
            e.HasIndex(x => x.AbsoluteExpiresAt).HasDatabaseName("ix_admin_sessions_expiry");
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

        // The six content blocks sit above the DateTime converter loop for the
        // same reason the admin ones do: below it, updated_at reads back as
        // DateTimeKind.Unspecified and one ToUniversalTime() later the panel
        // shows a save made a second ago as four hours old.
        //
        // No index on any long text column anywhere below.
        // AllIndexKeyLengths_AreUnder3072Bytes scans the whole database, and a
        // varchar(600) utf8mb4 index is 2408 octets on its own.
        b.Entity<SiteFaq>(e =>
        {
            e.ToTable("site_faq");
            e.HasTableOption("ROW_FORMAT", "DYNAMIC");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id").HasColumnType("bigint unsigned").ValueGeneratedOnAdd();
            e.Property(x => x.Question).HasColumnName("question").HasMaxLength(200).IsRequired();
            e.Property(x => x.Answer).HasColumnName("answer").HasMaxLength(600).IsRequired();
            e.Property(x => x.SortOrder).HasColumnName("sort_order").HasColumnType("int unsigned")
                .IsRequired().HasDefaultValue(0);
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime(6)").IsRequired();
            e.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(64)
                .IsRequired().HasDefaultValue("");

            e.HasIndex(x => x.SortOrder).HasDatabaseName("ix_site_faq_order");
        });

        b.Entity<SiteTeamMember>(e =>
        {
            e.ToTable("site_team");
            e.HasTableOption("ROW_FORMAT", "DYNAMIC");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id").HasColumnType("bigint unsigned").ValueGeneratedOnAdd();
            e.Property(x => x.Name).HasColumnName("name").HasMaxLength(80).IsRequired();
            e.Property(x => x.Initials).HasColumnName("initials").HasMaxLength(4)
                .HasCharSet("ascii").UseCollation("ascii_bin").IsRequired();
            e.Property(x => x.Role).HasColumnName("role").HasMaxLength(60).IsRequired();
            e.Property(x => x.Focus).HasColumnName("focus").HasMaxLength(120).IsRequired();
            e.Property(x => x.Photo).HasColumnName("photo").HasMaxLength(80)
                .HasCharSet("ascii").UseCollation("ascii_bin").IsRequired().HasDefaultValue("");
            e.Property(x => x.SortOrder).HasColumnName("sort_order").HasColumnType("int unsigned")
                .IsRequired().HasDefaultValue(0);
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime(6)").IsRequired();
            e.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(64)
                .IsRequired().HasDefaultValue("");

            e.HasIndex(x => x.SortOrder).HasDatabaseName("ix_site_team_order");
        });

        b.Entity<SiteCompany>(e =>
        {
            e.ToTable("site_companies");
            e.HasTableOption("ROW_FORMAT", "DYNAMIC");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id").HasColumnType("bigint unsigned").ValueGeneratedOnAdd();
            e.Property(x => x.Name).HasColumnName("name").HasMaxLength(80).IsRequired();
            e.Property(x => x.LogoFile).HasColumnName("logo_file").HasMaxLength(64)
                .HasCharSet("ascii").UseCollation("ascii_bin").IsRequired();
            e.Property(x => x.Hidden).HasColumnName("hidden").HasColumnType("tinyint(1)")
                .IsRequired().HasDefaultValue(false);
            e.Property(x => x.SortOrder).HasColumnName("sort_order").HasColumnType("int unsigned")
                .IsRequired().HasDefaultValue(0);
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime(6)").IsRequired();
            e.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(64)
                .IsRequired().HasDefaultValue("");

            // One row per logo file, so the same image cannot be added twice
            // and appear twice in the marquee.
            e.HasIndex(x => x.LogoFile).IsUnique().HasDatabaseName("uk_site_companies_file");
            e.HasIndex(x => x.SortOrder).HasDatabaseName("ix_site_companies_order");
        });

        b.Entity<SiteService>(e =>
        {
            e.ToTable("site_services");
            e.HasTableOption("ROW_FORMAT", "DYNAMIC");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id").HasColumnType("bigint unsigned").ValueGeneratedOnAdd();
            e.Property(x => x.Slug).HasColumnName("slug").HasMaxLength(32)
                .HasCharSet("ascii").UseCollation("ascii_bin").IsRequired();
            e.Property(x => x.Name).HasColumnName("name").HasMaxLength(60).IsRequired();
            e.Property(x => x.Tagline).HasColumnName("tagline").HasMaxLength(120).IsRequired();
            e.Property(x => x.CardLabel).HasColumnName("card_label").HasMaxLength(60).IsRequired();
            e.Property(x => x.Description).HasColumnName("description").HasMaxLength(400).IsRequired();
            e.Property(x => x.IconName).HasColumnName("icon_name").HasMaxLength(32)
                .HasCharSet("ascii").UseCollation("ascii_bin").IsRequired();
            e.Property(x => x.SortOrder).HasColumnName("sort_order").HasColumnType("int unsigned")
                .IsRequired().HasDefaultValue(0);
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime(6)").IsRequired();
            e.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(64)
                .IsRequired().HasDefaultValue("");

            e.HasIndex(x => x.Slug).IsUnique().HasDatabaseName("uk_site_services_slug");
            e.HasIndex(x => x.SortOrder).HasDatabaseName("ix_site_services_order");
        });

        b.Entity<SiteServiceStep>(e =>
        {
            e.ToTable("site_service_steps");
            e.HasTableOption("ROW_FORMAT", "DYNAMIC");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id").HasColumnType("bigint unsigned").ValueGeneratedOnAdd();
            e.Property(x => x.ServiceId).HasColumnName("service_id").HasColumnType("bigint unsigned").IsRequired();
            e.Property(x => x.Phase).HasColumnName("phase").HasMaxLength(40).IsRequired();
            e.Property(x => x.Summary).HasColumnName("summary").HasMaxLength(80).IsRequired();
            e.Property(x => x.Detail).HasColumnName("detail").HasMaxLength(240).IsRequired();
            e.Property(x => x.SortOrder).HasColumnName("sort_order").HasColumnType("int unsigned")
                .IsRequired().HasDefaultValue(0);

            e.HasIndex(x => new { x.ServiceId, x.SortOrder }).HasDatabaseName("ix_site_service_steps_service");
        });

        b.Entity<SiteServiceHighlight>(e =>
        {
            e.ToTable("site_service_highlights");
            e.HasTableOption("ROW_FORMAT", "DYNAMIC");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id").HasColumnType("bigint unsigned").ValueGeneratedOnAdd();
            e.Property(x => x.ServiceId).HasColumnName("service_id").HasColumnType("bigint unsigned").IsRequired();
            e.Property(x => x.Text).HasColumnName("text").HasMaxLength(80).IsRequired();
            e.Property(x => x.SortOrder).HasColumnName("sort_order").HasColumnType("int unsigned")
                .IsRequired().HasDefaultValue(0);

            e.HasIndex(x => new { x.ServiceId, x.SortOrder }).HasDatabaseName("ix_site_service_highlights_service");
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

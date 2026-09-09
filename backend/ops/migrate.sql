CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;
DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260906232200_InitialSchema') THEN

    ALTER DATABASE CHARACTER SET utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260906232200_InitialSchema') THEN

    CREATE TABLE `contact_submissions` (
        `id` bigint unsigned NOT NULL AUTO_INCREMENT,
        `created_at` datetime(6) NOT NULL,
        `name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `email` varchar(254) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_as_cs NOT NULL,
        `company` varchar(200) CHARACTER SET utf8mb4 NOT NULL DEFAULT '',
        `phone` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '',
        `service` varchar(32) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
        `message` varchar(5000) CHARACTER SET utf8mb4 NOT NULL,
        `dedupe_key` char(64) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
        `legal_hold` tinyint(1) NOT NULL DEFAULT FALSE,
        `purge_after` date NOT NULL,
        `notify_state` varchar(16) CHARACTER SET ascii COLLATE ascii_bin NOT NULL DEFAULT 'pending',
        `notify_attempts` tinyint unsigned NOT NULL DEFAULT 0,
        `notify_next_attempt_at` datetime(6) NULL,
        `notified_at` datetime(6) NULL,
        `notify_error` varchar(300) CHARACTER SET utf8mb4 NOT NULL DEFAULT '',
        CONSTRAINT `PK_contact_submissions` PRIMARY KEY (`id`)
    ) CHARACTER SET=utf8mb4 ROW_FORMAT=DYNAMIC;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260906232200_InitialSchema') THEN

    CREATE TABLE `dsr_log` (
        `id` bigint unsigned NOT NULL AUTO_INCREMENT,
        `received_on` date NOT NULL,
        `request_type` varchar(16) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
        `subject_email_hash` char(64) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
        `rows_affected` int unsigned NOT NULL DEFAULT 0,
        `affected_ids` varchar(500) CHARACTER SET ascii COLLATE ascii_bin NOT NULL DEFAULT '',
        `handled_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
        `closed_on` date NULL,
        CONSTRAINT `PK_dsr_log` PRIMARY KEY (`id`)
    ) CHARACTER SET=utf8mb4 ROW_FORMAT=DYNAMIC;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260906232200_InitialSchema') THEN

    CREATE TABLE `job_runs` (
        `id` bigint unsigned NOT NULL AUTO_INCREMENT,
        `job_name` varchar(48) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
        `started_at` datetime(6) NOT NULL,
        `finished_at` datetime(6) NULL,
        `outcome` varchar(16) CHARACTER SET ascii COLLATE ascii_bin NOT NULL DEFAULT 'running',
        `cutoff_date` date NULL,
        `rows_affected` int unsigned NOT NULL DEFAULT 0,
        `duration_ms` int unsigned NOT NULL DEFAULT 0,
        `detail` varchar(300) CHARACTER SET utf8mb4 NOT NULL DEFAULT '',
        CONSTRAINT `PK_job_runs` PRIMARY KEY (`id`)
    ) CHARACTER SET=utf8mb4 ROW_FORMAT=DYNAMIC;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260906232200_InitialSchema') THEN

    CREATE INDEX `ix_submissions_email` ON `contact_submissions` (`email`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260906232200_InitialSchema') THEN

    CREATE INDEX `ix_submissions_notify` ON `contact_submissions` (`notify_state`, `notify_next_attempt_at`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260906232200_InitialSchema') THEN

    CREATE INDEX `ix_submissions_purge` ON `contact_submissions` (`legal_hold`, `purge_after`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260906232200_InitialSchema') THEN

    CREATE UNIQUE INDEX `uk_submissions_dedupe` ON `contact_submissions` (`dedupe_key`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260906232200_InitialSchema') THEN

    CREATE INDEX `ix_dsr_hash` ON `dsr_log` (`subject_email_hash`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260906232200_InitialSchema') THEN

    CREATE INDEX `ix_dsr_received` ON `dsr_log` (`received_on`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260906232200_InitialSchema') THEN

    CREATE INDEX `ix_job_runs_name_time` ON `job_runs` (`job_name`, `started_at`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260906232200_InitialSchema') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260906232200_InitialSchema', '9.0.19');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260909175824_AdminPanel') THEN

    ALTER TABLE `contact_submissions` ADD `handled_at` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260909175824_AdminPanel') THEN

    ALTER TABLE `contact_submissions` ADD `handled_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260909175824_AdminPanel') THEN

    CREATE TABLE `admin_accounts` (
        `id` bigint unsigned NOT NULL AUTO_INCREMENT,
        `username` varchar(64) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
        `display_name` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '',
        `password_hash` varchar(256) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
        `totp_secret` varchar(64) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
        `totp_last_step` bigint unsigned NOT NULL DEFAULT 0,
        `disabled` tinyint(1) NOT NULL DEFAULT FALSE,
        `failed_attempts` smallint unsigned NOT NULL DEFAULT 0,
        `first_failed_at` datetime(6) NULL,
        `locked_until` datetime(6) NULL,
        `created_at` datetime(6) NOT NULL,
        `last_login_at` datetime(6) NULL,
        CONSTRAINT `PK_admin_accounts` PRIMARY KEY (`id`)
    ) CHARACTER SET=utf8mb4 ROW_FORMAT=DYNAMIC;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260909175824_AdminPanel') THEN

    CREATE TABLE `admin_sessions` (
        `token_hash` char(64) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
        `account_id` bigint unsigned NOT NULL,
        `created_at` datetime(6) NOT NULL,
        `last_seen_at` datetime(6) NOT NULL,
        `idle_expires_at` datetime(6) NOT NULL,
        `absolute_expires_at` datetime(6) NOT NULL,
        CONSTRAINT `PK_admin_sessions` PRIMARY KEY (`token_hash`)
    ) CHARACTER SET=utf8mb4 ROW_FORMAT=DYNAMIC;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260909175824_AdminPanel') THEN

    CREATE UNIQUE INDEX `uk_admin_accounts_username` ON `admin_accounts` (`username`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260909175824_AdminPanel') THEN

    CREATE INDEX `ix_admin_sessions_account` ON `admin_sessions` (`account_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260909175824_AdminPanel') THEN

    CREATE INDEX `ix_admin_sessions_expiry` ON `admin_sessions` (`absolute_expires_at`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260909175824_AdminPanel') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260909175824_AdminPanel', '9.0.19');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260909210321_SiteContent') THEN

    CREATE TABLE `site_companies` (
        `id` bigint unsigned NOT NULL AUTO_INCREMENT,
        `name` varchar(80) CHARACTER SET utf8mb4 NOT NULL,
        `logo_file` varchar(64) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
        `hidden` tinyint(1) NOT NULL DEFAULT FALSE,
        `sort_order` int unsigned NOT NULL DEFAULT 0,
        `updated_at` datetime(6) NOT NULL,
        `updated_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '',
        CONSTRAINT `PK_site_companies` PRIMARY KEY (`id`)
    ) CHARACTER SET=utf8mb4 ROW_FORMAT=DYNAMIC;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260909210321_SiteContent') THEN

    CREATE TABLE `site_faq` (
        `id` bigint unsigned NOT NULL AUTO_INCREMENT,
        `question` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
        `answer` varchar(600) CHARACTER SET utf8mb4 NOT NULL,
        `sort_order` int unsigned NOT NULL DEFAULT 0,
        `updated_at` datetime(6) NOT NULL,
        `updated_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '',
        CONSTRAINT `PK_site_faq` PRIMARY KEY (`id`)
    ) CHARACTER SET=utf8mb4 ROW_FORMAT=DYNAMIC;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260909210321_SiteContent') THEN

    CREATE TABLE `site_service_highlights` (
        `id` bigint unsigned NOT NULL AUTO_INCREMENT,
        `service_id` bigint unsigned NOT NULL,
        `text` varchar(80) CHARACTER SET utf8mb4 NOT NULL,
        `sort_order` int unsigned NOT NULL DEFAULT 0,
        CONSTRAINT `PK_site_service_highlights` PRIMARY KEY (`id`)
    ) CHARACTER SET=utf8mb4 ROW_FORMAT=DYNAMIC;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260909210321_SiteContent') THEN

    CREATE TABLE `site_service_steps` (
        `id` bigint unsigned NOT NULL AUTO_INCREMENT,
        `service_id` bigint unsigned NOT NULL,
        `phase` varchar(40) CHARACTER SET utf8mb4 NOT NULL,
        `summary` varchar(80) CHARACTER SET utf8mb4 NOT NULL,
        `detail` varchar(240) CHARACTER SET utf8mb4 NOT NULL,
        `sort_order` int unsigned NOT NULL DEFAULT 0,
        CONSTRAINT `PK_site_service_steps` PRIMARY KEY (`id`)
    ) CHARACTER SET=utf8mb4 ROW_FORMAT=DYNAMIC;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260909210321_SiteContent') THEN

    CREATE TABLE `site_services` (
        `id` bigint unsigned NOT NULL AUTO_INCREMENT,
        `slug` varchar(32) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
        `name` varchar(60) CHARACTER SET utf8mb4 NOT NULL,
        `tagline` varchar(120) CHARACTER SET utf8mb4 NOT NULL,
        `card_label` varchar(60) CHARACTER SET utf8mb4 NOT NULL,
        `description` varchar(400) CHARACTER SET utf8mb4 NOT NULL,
        `icon_name` varchar(32) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
        `sort_order` int unsigned NOT NULL DEFAULT 0,
        `updated_at` datetime(6) NOT NULL,
        `updated_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '',
        CONSTRAINT `PK_site_services` PRIMARY KEY (`id`)
    ) CHARACTER SET=utf8mb4 ROW_FORMAT=DYNAMIC;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260909210321_SiteContent') THEN

    CREATE TABLE `site_team` (
        `id` bigint unsigned NOT NULL AUTO_INCREMENT,
        `name` varchar(80) CHARACTER SET utf8mb4 NOT NULL,
        `initials` varchar(4) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
        `role` varchar(60) CHARACTER SET utf8mb4 NOT NULL,
        `focus` varchar(120) CHARACTER SET utf8mb4 NOT NULL,
        `photo` varchar(80) CHARACTER SET ascii COLLATE ascii_bin NOT NULL DEFAULT '',
        `sort_order` int unsigned NOT NULL DEFAULT 0,
        `updated_at` datetime(6) NOT NULL,
        `updated_by` varchar(64) CHARACTER SET utf8mb4 NOT NULL DEFAULT '',
        CONSTRAINT `PK_site_team` PRIMARY KEY (`id`)
    ) CHARACTER SET=utf8mb4 ROW_FORMAT=DYNAMIC;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260909210321_SiteContent') THEN

    CREATE INDEX `ix_site_companies_order` ON `site_companies` (`sort_order`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260909210321_SiteContent') THEN

    CREATE UNIQUE INDEX `uk_site_companies_file` ON `site_companies` (`logo_file`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260909210321_SiteContent') THEN

    CREATE INDEX `ix_site_faq_order` ON `site_faq` (`sort_order`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260909210321_SiteContent') THEN

    CREATE INDEX `ix_site_service_highlights_service` ON `site_service_highlights` (`service_id`, `sort_order`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260909210321_SiteContent') THEN

    CREATE INDEX `ix_site_service_steps_service` ON `site_service_steps` (`service_id`, `sort_order`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260909210321_SiteContent') THEN

    CREATE INDEX `ix_site_services_order` ON `site_services` (`sort_order`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260909210321_SiteContent') THEN

    CREATE UNIQUE INDEX `uk_site_services_slug` ON `site_services` (`slug`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260909210321_SiteContent') THEN

    CREATE INDEX `ix_site_team_order` ON `site_team` (`sort_order`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260909210321_SiteContent') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260909210321_SiteContent', '9.0.19');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;


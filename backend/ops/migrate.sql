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

COMMIT;


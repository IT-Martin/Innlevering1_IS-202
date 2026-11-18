SET NAMES utf8mb4;
-- Trinn 18: AuditEntry
-- Hentet fra den fungerende Reachout-databasen.
USE `reachout_sql_oving`;

CREATE TABLE `AuditEntry` (
  `audit_entry_id` int(11) NOT NULL AUTO_INCREMENT,
  `account_id` int(11) DEFAULT NULL,
  `action` varchar(80) NOT NULL,
  `target` varchar(100) NOT NULL,
  `occurred_at` datetime(6) NOT NULL,
  PRIMARY KEY (`audit_entry_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

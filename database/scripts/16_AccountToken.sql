SET NAMES utf8mb4;
-- Trinn 16: AccountToken
-- Hentet fra den fungerende Reachout-databasen.
USE `reachout_sql_oving`;

CREATE TABLE `AccountToken` (
  `account_token_id` int(11) NOT NULL AUTO_INCREMENT,
  `security_stamp` varchar(100) NOT NULL,
  `account_id` int(11) NOT NULL,
  `token_hash` varchar(100) NOT NULL,
  `purpose` varchar(30) NOT NULL,
  `expires_at` datetime(6) NOT NULL,
  `used_at` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`account_token_id`),
  UNIQUE KEY `IX_AccountToken_token_hash` (`token_hash`),
  KEY `IX_AccountToken_account_id` (`account_id`),
  CONSTRAINT `FK_AccountToken_Account_account_id` FOREIGN KEY (`account_id`) REFERENCES `Account` (`account_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

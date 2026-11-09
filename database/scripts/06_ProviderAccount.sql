SET NAMES utf8mb4;
-- Trinn 6: ProviderAccount
-- Hentet fra den fungerende Reachout-databasen.
USE `reachout_sql_oving`;

CREATE TABLE `ProviderAccount` (
  `account_id` int(11) NOT NULL,
  `provider_type` varchar(100) NOT NULL,
  `contact_name` varchar(200) NOT NULL,
  `phone` varchar(50) NOT NULL,
  `address` varchar(100) NOT NULL,
  `organization_name` varchar(200) NOT NULL,
  `organization_number` varchar(100) NOT NULL,
  PRIMARY KEY (`account_id`),
  CONSTRAINT `FK_ProviderAccount_Account_account_id` FOREIGN KEY (`account_id`) REFERENCES `Account` (`account_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

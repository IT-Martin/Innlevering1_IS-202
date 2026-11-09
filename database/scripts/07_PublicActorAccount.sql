SET NAMES utf8mb4;
-- Trinn 7: PublicActorAccount
-- Hentet fra den fungerende Reachout-databasen.
USE `reachout_sql_oving`;

CREATE TABLE `PublicActorAccount` (
  `account_id` int(11) NOT NULL,
  `public_organization_id` int(11) NOT NULL,
  `permission` varchar(30) NOT NULL,
  PRIMARY KEY (`account_id`),
  KEY `IX_PublicActorAccount_public_organization_id` (`public_organization_id`),
  CONSTRAINT `FK_PublicActorAccount_Account_account_id` FOREIGN KEY (`account_id`) REFERENCES `Account` (`account_id`),
  CONSTRAINT `FK_PublicActorAccount_PublicOrganization_public_organization_id` FOREIGN KEY (`public_organization_id`) REFERENCES `PublicOrganization` (`public_organization_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

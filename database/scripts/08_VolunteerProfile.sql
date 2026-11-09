SET NAMES utf8mb4;
-- Trinn 8: VolunteerProfile
-- Hentet fra den fungerende Reachout-databasen.
USE `reachout_sql_oving`;

CREATE TABLE `VolunteerProfile` (
  `volunteer_profile_id` int(11) NOT NULL AUTO_INCREMENT,
  `account_id` int(11) NOT NULL,
  `service_area` varchar(300) NOT NULL,
  `volunteer_role` varchar(200) NOT NULL,
  `additional_information` varchar(2000) NOT NULL,
  `archived` tinyint(1) NOT NULL,
  PRIMARY KEY (`volunteer_profile_id`),
  UNIQUE KEY `IX_VolunteerProfile_account_id` (`account_id`),
  CONSTRAINT `FK_VolunteerProfile_ProviderAccount_account_id` FOREIGN KEY (`account_id`) REFERENCES `ProviderAccount` (`account_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

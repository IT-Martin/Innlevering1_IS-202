SET NAMES utf8mb4;
-- Trinn 15: VolunteerInterest
-- Hentet fra den fungerende Reachout-databasen.
USE `reachout_sql_oving`;

CREATE TABLE `VolunteerInterest` (
  `interest_id` int(11) NOT NULL AUTO_INCREMENT,
  `volunteer_profile_id` int(11) NOT NULL,
  `resource_need_id` int(11) NOT NULL,
  `message` varchar(2000) NOT NULL,
  `interest_status` varchar(40) NOT NULL,
  `submitted_at` datetime(6) NOT NULL,
  PRIMARY KEY (`interest_id`),
  KEY `IX_VolunteerInterest_resource_need_id` (`resource_need_id`),
  KEY `IX_VolunteerInterest_volunteer_profile_id` (`volunteer_profile_id`),
  CONSTRAINT `FK_VolunteerInterest_ResourceNeed_resource_need_id` FOREIGN KEY (`resource_need_id`) REFERENCES `ResourceNeed` (`resource_need_id`),
  CONSTRAINT `FK_VolunteerInterest_VolunteerProfile_volunteer_profile_id` FOREIGN KEY (`volunteer_profile_id`) REFERENCES `VolunteerProfile` (`volunteer_profile_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

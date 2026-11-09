SET NAMES utf8mb4;
-- Trinn 11: Availability
-- Hentet fra den fungerende Reachout-databasen.
USE `reachout_sql_oving`;

CREATE TABLE `Availability` (
  `availability_id` int(11) NOT NULL AUTO_INCREMENT,
  `resource_id` int(11) DEFAULT NULL,
  `volunteer_profile_id` int(11) DEFAULT NULL,
  `available_from` datetime(6) NOT NULL,
  `available_to` datetime(6) NOT NULL,
  PRIMARY KEY (`availability_id`),
  KEY `IX_Availability_resource_id` (`resource_id`),
  KEY `IX_Availability_volunteer_profile_id` (`volunteer_profile_id`),
  CONSTRAINT `FK_Availability_Resource_resource_id` FOREIGN KEY (`resource_id`) REFERENCES `Resource` (`resource_id`),
  CONSTRAINT `FK_Availability_VolunteerProfile_volunteer_profile_id` FOREIGN KEY (`volunteer_profile_id`) REFERENCES `VolunteerProfile` (`volunteer_profile_id`),
  CONSTRAINT `CK_Availability_Owner` CHECK (`resource_id` is not null <> (`volunteer_profile_id` is not null)),
  CONSTRAINT `CK_Availability_Period` CHECK (`available_to` > `available_from`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

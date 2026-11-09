SET NAMES utf8mb4;
-- Trinn 12: VolunteerProfileSkill
-- Hentet fra den fungerende Reachout-databasen.
USE `reachout_sql_oving`;

CREATE TABLE `VolunteerProfileSkill` (
  `volunteer_profile_id` int(11) NOT NULL,
  `skill_id` int(11) NOT NULL,
  PRIMARY KEY (`volunteer_profile_id`,`skill_id`),
  KEY `IX_VolunteerProfileSkill_skill_id` (`skill_id`),
  CONSTRAINT `FK_VolunteerProfileSkill_Skill_skill_id` FOREIGN KEY (`skill_id`) REFERENCES `Skill` (`skill_id`),
  CONSTRAINT `FK_VolunteerProfileSkill_VolunteerProfile_volunteer_profile_id` FOREIGN KEY (`volunteer_profile_id`) REFERENCES `VolunteerProfile` (`volunteer_profile_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

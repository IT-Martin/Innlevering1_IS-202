-- Alternativ til å kjøre 00–21 enkeltvis. Ikke kjør begge fremgangsmåter.

-- FIL: 00_opprett_ovingsdatabase.sql
SET NAMES utf8mb4;
-- Kjør som administrator mot MariaDB 11.8.
-- Oppretter en NY øvingsdatabase, ikke den eksisterende Reachout-databasen.
CREATE DATABASE `reachout_sql_oving` CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `reachout_sql_oving`;

-- FIL: 01_Account.sql
SET NAMES utf8mb4;
-- Trinn 1: Account
-- Hentet fra den fungerende Reachout-databasen.
USE `reachout_sql_oving`;

CREATE TABLE `Account` (
  `account_id` int(11) NOT NULL AUTO_INCREMENT,
  `account_type` varchar(50) NOT NULL,
  `email` varchar(255) NOT NULL,
  `password_hash` varchar(255) NOT NULL,
  `contact_name` varchar(200) NOT NULL DEFAULT '',
  `phone` varchar(50) NOT NULL DEFAULT '',
  `access_revoked` tinyint(1) NOT NULL DEFAULT 0,
  `email_confirmed` tinyint(1) NOT NULL DEFAULT 0,
  `failed_attempts` int(11) NOT NULL DEFAULT 0,
  `locked_until` datetime(6) DEFAULT NULL,
  `security_stamp` varchar(100) NOT NULL DEFAULT '',
  PRIMARY KEY (`account_id`),
  UNIQUE KEY `IX_Account_email` (`email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- FIL: 02_PublicOrganization.sql
SET NAMES utf8mb4;
-- Trinn 2: PublicOrganization
-- Hentet fra den fungerende Reachout-databasen.
USE `reachout_sql_oving`;

CREATE TABLE `PublicOrganization` (
  `public_organization_id` int(11) NOT NULL AUTO_INCREMENT,
  `organization_name` varchar(200) NOT NULL,
  `organization_type` varchar(100) NOT NULL,
  `organization_number` varchar(20) NOT NULL,
  `email_domain` varchar(200) NOT NULL,
  `contact_email` varchar(255) NOT NULL,
  `contact_name` varchar(200) NOT NULL,
  `phone` varchar(50) NOT NULL,
  `approval_status` varchar(30) NOT NULL,
  `verification` varchar(1000) NOT NULL,
  `created_at` datetime(6) NOT NULL,
  PRIMARY KEY (`public_organization_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- FIL: 03_Location.sql
SET NAMES utf8mb4;
-- Trinn 3: Location
-- Hentet fra den fungerende Reachout-databasen.
USE `reachout_sql_oving`;

CREATE TABLE `Location` (
  `location_id` int(11) NOT NULL AUTO_INCREMENT,
  `address_or_area` varchar(500) NOT NULL,
  `latitude` decimal(9,6) NOT NULL,
  `longitude` decimal(10,7) NOT NULL,
  PRIMARY KEY (`location_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- FIL: 04_ResourceType.sql
SET NAMES utf8mb4;
-- Trinn 4: ResourceType
-- Hentet fra den fungerende Reachout-databasen.
USE `reachout_sql_oving`;

CREATE TABLE `ResourceType` (
  `resource_type_id` int(11) NOT NULL AUTO_INCREMENT,
  `category_name` varchar(200) NOT NULL,
  PRIMARY KEY (`resource_type_id`),
  UNIQUE KEY `IX_ResourceType_category_name` (`category_name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- FIL: 05_Skill.sql
SET NAMES utf8mb4;
-- Trinn 5: Skill
-- Hentet fra den fungerende Reachout-databasen.
USE `reachout_sql_oving`;

CREATE TABLE `Skill` (
  `skill_id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(200) NOT NULL,
  `description` varchar(1000) NOT NULL,
  PRIMARY KEY (`skill_id`),
  UNIQUE KEY `IX_Skill_name` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- FIL: 06_ProviderAccount.sql
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

-- FIL: 07_PublicActorAccount.sql
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

-- FIL: 08_VolunteerProfile.sql
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

-- FIL: 09_Resource.sql
SET NAMES utf8mb4;
-- Trinn 9: Resource
-- Hentet fra den fungerende Reachout-databasen.
USE `reachout_sql_oving`;

CREATE TABLE `Resource` (
  `resource_id` int(11) NOT NULL AUTO_INCREMENT,
  `account_id` int(11) NOT NULL,
  `resource_type_id` int(11) NOT NULL,
  `location_id` int(11) NOT NULL,
  `resource_name` varchar(200) NOT NULL,
  `description` varchar(4000) NOT NULL,
  `contact_details` varchar(1000) NOT NULL,
  `verification_status` varchar(75) NOT NULL,
  `archived` tinyint(1) NOT NULL DEFAULT 0,
  `availability_status` varchar(40) NOT NULL DEFAULT '',
  `image_content_type` varchar(30) DEFAULT NULL,
  `image_data` mediumblob DEFAULT NULL,
  PRIMARY KEY (`resource_id`),
  UNIQUE KEY `IX_Resource_location_id` (`location_id`),
  KEY `IX_Resource_account_id` (`account_id`),
  KEY `IX_Resource_resource_type_id` (`resource_type_id`),
  CONSTRAINT `FK_Resource_Location_location_id` FOREIGN KEY (`location_id`) REFERENCES `Location` (`location_id`),
  CONSTRAINT `FK_Resource_ProviderAccount_account_id` FOREIGN KEY (`account_id`) REFERENCES `ProviderAccount` (`account_id`),
  CONSTRAINT `FK_Resource_ResourceType_resource_type_id` FOREIGN KEY (`resource_type_id`) REFERENCES `ResourceType` (`resource_type_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- FIL: 10_ResourceNeed.sql
SET NAMES utf8mb4;
-- Trinn 10: ResourceNeed
-- Hentet fra den fungerende Reachout-databasen.
USE `reachout_sql_oving`;

CREATE TABLE `ResourceNeed` (
  `resource_need_id` int(11) NOT NULL AUTO_INCREMENT,
  `public_actor_account_id` int(11) NOT NULL,
  `resource_type_id` int(11) NOT NULL,
  `location_id` int(11) NOT NULL,
  `title` varchar(200) NOT NULL,
  `description` varchar(4000) NOT NULL,
  `priority` varchar(30) NOT NULL,
  `status` varchar(30) NOT NULL,
  `required_start` datetime(6) NOT NULL,
  `required_end` datetime(6) NOT NULL,
  `deadline` datetime(6) DEFAULT NULL,
  `archived` tinyint(1) NOT NULL,
  PRIMARY KEY (`resource_need_id`),
  UNIQUE KEY `AK_ResourceNeed_resource_need_id_public_actor_account_id` (`resource_need_id`,`public_actor_account_id`),
  KEY `IX_ResourceNeed_location_id` (`location_id`),
  KEY `IX_ResourceNeed_public_actor_account_id` (`public_actor_account_id`),
  KEY `IX_ResourceNeed_resource_type_id` (`resource_type_id`),
  CONSTRAINT `FK_ResourceNeed_Location_location_id` FOREIGN KEY (`location_id`) REFERENCES `Location` (`location_id`),
  CONSTRAINT `FK_ResourceNeed_PublicActorAccount_public_actor_account_id` FOREIGN KEY (`public_actor_account_id`) REFERENCES `PublicActorAccount` (`account_id`),
  CONSTRAINT `FK_ResourceNeed_ResourceType_resource_type_id` FOREIGN KEY (`resource_type_id`) REFERENCES `ResourceType` (`resource_type_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- FIL: 11_Availability.sql
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

-- FIL: 12_VolunteerProfileSkill.sql
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

-- FIL: 13_ResourceMatch.sql
SET NAMES utf8mb4;
-- Trinn 13: ResourceMatch
-- Hentet fra den fungerende Reachout-databasen.
USE `reachout_sql_oving`;

CREATE TABLE `ResourceMatch` (
  `resource_match_id` int(11) NOT NULL AUTO_INCREMENT,
  `resource_id` int(11) NOT NULL,
  `resource_need_id` int(11) NOT NULL,
  `public_actor_account_id` int(11) NOT NULL,
  `match_status` varchar(40) NOT NULL,
  `assessed_at` datetime(6) NOT NULL,
  PRIMARY KEY (`resource_match_id`),
  UNIQUE KEY `AK_ResourceMatch_resource_match_id_public_actor_account_id` (`resource_match_id`,`public_actor_account_id`),
  UNIQUE KEY `IX_ResourceMatch_resource_need_id` (`resource_need_id`),
  KEY `IX_ResourceMatch_public_actor_account_id` (`public_actor_account_id`),
  KEY `IX_ResourceMatch_resource_id` (`resource_id`),
  KEY `IX_ResourceMatch_resource_need_id_public_actor_account_id` (`resource_need_id`,`public_actor_account_id`),
  CONSTRAINT `FK_ResourceMatch_PublicActorAccount_public_actor_account_id` FOREIGN KEY (`public_actor_account_id`) REFERENCES `PublicActorAccount` (`account_id`),
  CONSTRAINT `FK_ResourceMatch_ResourceNeed_resource_need_id_public_actor_acc~` FOREIGN KEY (`resource_need_id`, `public_actor_account_id`) REFERENCES `ResourceNeed` (`resource_need_id`, `public_actor_account_id`),
  CONSTRAINT `FK_ResourceMatch_Resource_resource_id` FOREIGN KEY (`resource_id`) REFERENCES `Resource` (`resource_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- FIL: 14_Reservation.sql
SET NAMES utf8mb4;
-- Trinn 14: Reservation
-- Hentet fra den fungerende Reachout-databasen.
USE `reachout_sql_oving`;

CREATE TABLE `Reservation` (
  `reservation_id` int(11) NOT NULL AUTO_INCREMENT,
  `resource_match_id` int(11) NOT NULL,
  `public_actor_account_id` int(11) NOT NULL,
  `reservation_status` varchar(40) NOT NULL,
  `requested_start` datetime(6) NOT NULL,
  `requested_end` datetime(6) NOT NULL,
  `message` varchar(2000) NOT NULL,
  PRIMARY KEY (`reservation_id`),
  UNIQUE KEY `IX_Reservation_resource_match_id` (`resource_match_id`),
  UNIQUE KEY `IX_Reservation_resource_match_id_public_actor_account_id` (`resource_match_id`,`public_actor_account_id`),
  CONSTRAINT `FK_Reservation_ResourceMatch_resource_match_id_public_actor_acc~` FOREIGN KEY (`resource_match_id`, `public_actor_account_id`) REFERENCES `ResourceMatch` (`resource_match_id`, `public_actor_account_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- FIL: 15_VolunteerInterest.sql
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

-- FIL: 16_AccountToken.sql
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

-- FIL: 17_OrganizationInvitation.sql
SET NAMES utf8mb4;
-- Trinn 17: OrganizationInvitation
-- Hentet fra den fungerende Reachout-databasen.
USE `reachout_sql_oving`;

CREATE TABLE `OrganizationInvitation` (
  `invitation_id` int(11) NOT NULL AUTO_INCREMENT,
  `public_organization_id` int(11) NOT NULL,
  `email` varchar(255) NOT NULL,
  `token_hash` varchar(100) NOT NULL,
  `permission` varchar(30) NOT NULL,
  `expires_at` datetime(6) NOT NULL,
  `used_at` datetime(6) DEFAULT NULL,
  `revoked` tinyint(1) NOT NULL,
  PRIMARY KEY (`invitation_id`),
  UNIQUE KEY `IX_OrganizationInvitation_token_hash` (`token_hash`),
  KEY `IX_OrganizationInvitation_public_organization_id` (`public_organization_id`),
  CONSTRAINT `FK_OrganizationInvitation_PublicOrganization_public_organizatio~` FOREIGN KEY (`public_organization_id`) REFERENCES `PublicOrganization` (`public_organization_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- FIL: 18_AuditEntry.sql
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

-- FIL: 19_AppWriteLock.sql
SET NAMES utf8mb4;
-- Trinn 19: AppWriteLock
-- Hentet fra den fungerende Reachout-databasen.
USE `reachout_sql_oving`;

CREATE TABLE `AppWriteLock` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- FIL: 20___EFMigrationsHistory.sql
SET NAMES utf8mb4;
-- Trinn 20: __EFMigrationsHistory
-- Hentet fra den fungerende Reachout-databasen.
USE `reachout_sql_oving`;

CREATE TABLE `__EFMigrationsHistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;

-- FIL: 21_faste_grunndata.sql
SET NAMES utf8mb4;
-- Bare faste oppslagsdata, teknisk låserad og migrasjonshistorikk.
-- Ingen brukerkontoer, passord eller tokener kopieres.
USE `reachout_sql_oving`;

INSERT INTO `ResourceType` (`resource_type_id`, `category_name`) VALUES
('3', 'Frivillige'),
('2', 'Heisekran'),
('1', 'Kjøretøy'),
('6', 'Lokaler'),
('7', 'Strøm'),
('4', 'Tilhenger'),
('5', 'Utstyr'),
('8', 'Vann');

INSERT INTO `Skill` (`skill_id`, `name`, `description`) VALUES
('1', 'Førerkort', ''),
('2', 'Førstehjelp', '');

INSERT INTO `AppWriteLock` (`Id`) VALUES
('1');

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) VALUES
('20261031154113_InitialCreate', '9.0.0'),
('20261031160456_AddAccountContactDetails', '9.0.0'),
('20261103115153_ExpandedWorkflows', '9.0.0'),
('20261103121226_OneMatchPerNeed', '9.0.0');



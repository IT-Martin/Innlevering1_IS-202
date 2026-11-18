CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;
ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `Account` (
    `account_id` int NOT NULL AUTO_INCREMENT,
    `account_type` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `email` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `password_hash` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Account` PRIMARY KEY (`account_id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Location` (
    `location_id` int NOT NULL AUTO_INCREMENT,
    `address_or_area` varchar(500) CHARACTER SET utf8mb4 NOT NULL,
    `latitude` decimal(9,6) NOT NULL,
    `longitude` decimal(10,7) NOT NULL,
    CONSTRAINT `PK_Location` PRIMARY KEY (`location_id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `ResourceType` (
    `resource_type_id` int NOT NULL AUTO_INCREMENT,
    `category_name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_ResourceType` PRIMARY KEY (`resource_type_id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `ProviderAccount` (
    `account_id` int NOT NULL,
    `provider_type` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `contact_name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `phone` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `address` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `organization_name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `organization_number` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_ProviderAccount` PRIMARY KEY (`account_id`),
    CONSTRAINT `FK_ProviderAccount_Account_account_id` FOREIGN KEY (`account_id`) REFERENCES `Account` (`account_id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `Resource` (
    `resource_id` int NOT NULL AUTO_INCREMENT,
    `account_id` int NOT NULL,
    `resource_type_id` int NOT NULL,
    `location_id` int NOT NULL,
    `resource_name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `description` varchar(4000) CHARACTER SET utf8mb4 NOT NULL,
    `contact_details` varchar(1000) CHARACTER SET utf8mb4 NOT NULL,
    `verification_status` varchar(75) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Resource` PRIMARY KEY (`resource_id`),
    CONSTRAINT `FK_Resource_Account_account_id` FOREIGN KEY (`account_id`) REFERENCES `Account` (`account_id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Resource_Location_location_id` FOREIGN KEY (`location_id`) REFERENCES `Location` (`location_id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Resource_ResourceType_resource_type_id` FOREIGN KEY (`resource_type_id`) REFERENCES `ResourceType` (`resource_type_id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

INSERT INTO `ResourceType` (`resource_type_id`, `category_name`)
VALUES (1, 'Kjøretøy'),
(2, 'Heisekran'),
(3, 'Frivillige'),
(4, 'Tilhenger');

CREATE UNIQUE INDEX `IX_Account_email` ON `Account` (`email`);

CREATE INDEX `IX_Resource_account_id` ON `Resource` (`account_id`);

CREATE INDEX `IX_Resource_location_id` ON `Resource` (`location_id`);

CREATE INDEX `IX_Resource_resource_type_id` ON `Resource` (`resource_type_id`);

CREATE UNIQUE INDEX `IX_ResourceType_category_name` ON `ResourceType` (`category_name`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261031154113_InitialCreate', '9.0.0');

ALTER TABLE `Account` ADD `contact_name` varchar(200) CHARACTER SET utf8mb4 NOT NULL DEFAULT '';

ALTER TABLE `Account` ADD `phone` varchar(50) CHARACTER SET utf8mb4 NOT NULL DEFAULT '';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261031160456_AddAccountContactDetails', '9.0.0');

ALTER TABLE `Resource` DROP FOREIGN KEY `FK_Resource_Account_account_id`;

CREATE INDEX IX_Resource_location_support ON Resource(location_id);

ALTER TABLE `Resource` DROP INDEX `IX_Resource_location_id`;

ALTER TABLE `Resource` ADD `archived` tinyint(1) NOT NULL DEFAULT FALSE;

ALTER TABLE `Resource` ADD `availability_status` varchar(40) CHARACTER SET utf8mb4 NOT NULL DEFAULT '';

ALTER TABLE `Resource` ADD `image_content_type` varchar(30) CHARACTER SET utf8mb4 NULL;

ALTER TABLE `Resource` ADD `image_data` mediumblob NULL;

ALTER TABLE `Account` ADD `access_revoked` tinyint(1) NOT NULL DEFAULT FALSE;

ALTER TABLE `Account` ADD `email_confirmed` tinyint(1) NOT NULL DEFAULT FALSE;

ALTER TABLE `Account` ADD `failed_attempts` int NOT NULL DEFAULT 0;

ALTER TABLE `Account` ADD `locked_until` datetime(6) NULL;

ALTER TABLE `Account` ADD `security_stamp` varchar(100) CHARACTER SET utf8mb4 NOT NULL DEFAULT '';

CREATE TABLE `AccountToken` (
    `account_token_id` int NOT NULL AUTO_INCREMENT,
    `security_stamp` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `account_id` int NOT NULL,
    `token_hash` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `purpose` varchar(30) CHARACTER SET utf8mb4 NOT NULL,
    `expires_at` datetime(6) NOT NULL,
    `used_at` datetime(6) NULL,
    CONSTRAINT `PK_AccountToken` PRIMARY KEY (`account_token_id`),
    CONSTRAINT `FK_AccountToken_Account_account_id` FOREIGN KEY (`account_id`) REFERENCES `Account` (`account_id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `AppWriteLock` (
    `Id` int NOT NULL AUTO_INCREMENT,
    CONSTRAINT `PK_AppWriteLock` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `AuditEntry` (
    `audit_entry_id` int NOT NULL AUTO_INCREMENT,
    `account_id` int NULL,
    `action` varchar(80) CHARACTER SET utf8mb4 NOT NULL,
    `target` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `occurred_at` datetime(6) NOT NULL,
    CONSTRAINT `PK_AuditEntry` PRIMARY KEY (`audit_entry_id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `PublicOrganization` (
    `public_organization_id` int NOT NULL AUTO_INCREMENT,
    `organization_name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `organization_type` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `organization_number` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
    `email_domain` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `contact_email` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `contact_name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `phone` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `approval_status` varchar(30) CHARACTER SET utf8mb4 NOT NULL,
    `verification` varchar(1000) CHARACTER SET utf8mb4 NOT NULL,
    `created_at` datetime(6) NOT NULL,
    CONSTRAINT `PK_PublicOrganization` PRIMARY KEY (`public_organization_id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Skill` (
    `skill_id` int NOT NULL AUTO_INCREMENT,
    `name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `description` varchar(1000) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Skill` PRIMARY KEY (`skill_id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `VolunteerProfile` (
    `volunteer_profile_id` int NOT NULL AUTO_INCREMENT,
    `account_id` int NOT NULL,
    `service_area` varchar(300) CHARACTER SET utf8mb4 NOT NULL,
    `volunteer_role` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `additional_information` varchar(2000) CHARACTER SET utf8mb4 NOT NULL,
    `archived` tinyint(1) NOT NULL,
    CONSTRAINT `PK_VolunteerProfile` PRIMARY KEY (`volunteer_profile_id`),
    CONSTRAINT `FK_VolunteerProfile_ProviderAccount_account_id` FOREIGN KEY (`account_id`) REFERENCES `ProviderAccount` (`account_id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `OrganizationInvitation` (
    `invitation_id` int NOT NULL AUTO_INCREMENT,
    `public_organization_id` int NOT NULL,
    `email` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `token_hash` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `permission` varchar(30) CHARACTER SET utf8mb4 NOT NULL,
    `expires_at` datetime(6) NOT NULL,
    `used_at` datetime(6) NULL,
    `revoked` tinyint(1) NOT NULL,
    CONSTRAINT `PK_OrganizationInvitation` PRIMARY KEY (`invitation_id`),
    CONSTRAINT `FK_OrganizationInvitation_PublicOrganization_public_organizatio~` FOREIGN KEY (`public_organization_id`) REFERENCES `PublicOrganization` (`public_organization_id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `PublicActorAccount` (
    `account_id` int NOT NULL,
    `public_organization_id` int NOT NULL,
    `permission` varchar(30) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_PublicActorAccount` PRIMARY KEY (`account_id`),
    CONSTRAINT `FK_PublicActorAccount_Account_account_id` FOREIGN KEY (`account_id`) REFERENCES `Account` (`account_id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_PublicActorAccount_PublicOrganization_public_organization_id` FOREIGN KEY (`public_organization_id`) REFERENCES `PublicOrganization` (`public_organization_id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `Availability` (
    `availability_id` int NOT NULL AUTO_INCREMENT,
    `resource_id` int NULL,
    `volunteer_profile_id` int NULL,
    `available_from` datetime(6) NOT NULL,
    `available_to` datetime(6) NOT NULL,
    CONSTRAINT `PK_Availability` PRIMARY KEY (`availability_id`),
    CONSTRAINT `CK_Availability_Owner` CHECK ((resource_id IS NOT NULL) <> (volunteer_profile_id IS NOT NULL)),
    CONSTRAINT `CK_Availability_Period` CHECK (available_to > available_from),
    CONSTRAINT `FK_Availability_Resource_resource_id` FOREIGN KEY (`resource_id`) REFERENCES `Resource` (`resource_id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Availability_VolunteerProfile_volunteer_profile_id` FOREIGN KEY (`volunteer_profile_id`) REFERENCES `VolunteerProfile` (`volunteer_profile_id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `VolunteerProfileSkill` (
    `volunteer_profile_id` int NOT NULL,
    `skill_id` int NOT NULL,
    CONSTRAINT `PK_VolunteerProfileSkill` PRIMARY KEY (`volunteer_profile_id`, `skill_id`),
    CONSTRAINT `FK_VolunteerProfileSkill_Skill_skill_id` FOREIGN KEY (`skill_id`) REFERENCES `Skill` (`skill_id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_VolunteerProfileSkill_VolunteerProfile_volunteer_profile_id` FOREIGN KEY (`volunteer_profile_id`) REFERENCES `VolunteerProfile` (`volunteer_profile_id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `ResourceNeed` (
    `resource_need_id` int NOT NULL AUTO_INCREMENT,
    `public_actor_account_id` int NOT NULL,
    `resource_type_id` int NOT NULL,
    `location_id` int NOT NULL,
    `title` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `description` varchar(4000) CHARACTER SET utf8mb4 NOT NULL,
    `priority` varchar(30) CHARACTER SET utf8mb4 NOT NULL,
    `status` varchar(30) CHARACTER SET utf8mb4 NOT NULL,
    `required_start` datetime(6) NOT NULL,
    `required_end` datetime(6) NOT NULL,
    `deadline` datetime(6) NULL,
    `archived` tinyint(1) NOT NULL,
    CONSTRAINT `PK_ResourceNeed` PRIMARY KEY (`resource_need_id`),
    CONSTRAINT `AK_ResourceNeed_resource_need_id_public_actor_account_id` UNIQUE (`resource_need_id`, `public_actor_account_id`),
    CONSTRAINT `FK_ResourceNeed_Location_location_id` FOREIGN KEY (`location_id`) REFERENCES `Location` (`location_id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_ResourceNeed_PublicActorAccount_public_actor_account_id` FOREIGN KEY (`public_actor_account_id`) REFERENCES `PublicActorAccount` (`account_id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_ResourceNeed_ResourceType_resource_type_id` FOREIGN KEY (`resource_type_id`) REFERENCES `ResourceType` (`resource_type_id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `ResourceMatch` (
    `resource_match_id` int NOT NULL AUTO_INCREMENT,
    `resource_id` int NOT NULL,
    `resource_need_id` int NOT NULL,
    `public_actor_account_id` int NOT NULL,
    `match_status` varchar(40) CHARACTER SET utf8mb4 NOT NULL,
    `assessed_at` datetime(6) NOT NULL,
    CONSTRAINT `PK_ResourceMatch` PRIMARY KEY (`resource_match_id`),
    CONSTRAINT `AK_ResourceMatch_resource_match_id_public_actor_account_id` UNIQUE (`resource_match_id`, `public_actor_account_id`),
    CONSTRAINT `FK_ResourceMatch_PublicActorAccount_public_actor_account_id` FOREIGN KEY (`public_actor_account_id`) REFERENCES `PublicActorAccount` (`account_id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_ResourceMatch_ResourceNeed_resource_need_id_public_actor_acc~` FOREIGN KEY (`resource_need_id`, `public_actor_account_id`) REFERENCES `ResourceNeed` (`resource_need_id`, `public_actor_account_id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_ResourceMatch_Resource_resource_id` FOREIGN KEY (`resource_id`) REFERENCES `Resource` (`resource_id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `VolunteerInterest` (
    `interest_id` int NOT NULL AUTO_INCREMENT,
    `volunteer_profile_id` int NOT NULL,
    `resource_need_id` int NOT NULL,
    `message` varchar(2000) CHARACTER SET utf8mb4 NOT NULL,
    `interest_status` varchar(40) CHARACTER SET utf8mb4 NOT NULL,
    `submitted_at` datetime(6) NOT NULL,
    CONSTRAINT `PK_VolunteerInterest` PRIMARY KEY (`interest_id`),
    CONSTRAINT `FK_VolunteerInterest_ResourceNeed_resource_need_id` FOREIGN KEY (`resource_need_id`) REFERENCES `ResourceNeed` (`resource_need_id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_VolunteerInterest_VolunteerProfile_volunteer_profile_id` FOREIGN KEY (`volunteer_profile_id`) REFERENCES `VolunteerProfile` (`volunteer_profile_id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `Reservation` (
    `reservation_id` int NOT NULL AUTO_INCREMENT,
    `resource_match_id` int NOT NULL,
    `public_actor_account_id` int NOT NULL,
    `reservation_status` varchar(40) CHARACTER SET utf8mb4 NOT NULL,
    `requested_start` datetime(6) NOT NULL,
    `requested_end` datetime(6) NOT NULL,
    `message` varchar(2000) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Reservation` PRIMARY KEY (`reservation_id`),
    CONSTRAINT `FK_Reservation_ResourceMatch_resource_match_id_public_actor_acc~` FOREIGN KEY (`resource_match_id`, `public_actor_account_id`) REFERENCES `ResourceMatch` (`resource_match_id`, `public_actor_account_id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

INSERT INTO `AppWriteLock` (`Id`)
VALUES (1);

INSERT INTO `ResourceType` (`resource_type_id`, `category_name`)
VALUES (5, 'Utstyr'),
(6, 'Lokaler'),
(7, 'Strøm'),
(8, 'Vann');

CREATE UNIQUE INDEX `IX_Resource_location_id` ON `Resource` (`location_id`);

DROP INDEX IX_Resource_location_support ON Resource;

CREATE INDEX `IX_AccountToken_account_id` ON `AccountToken` (`account_id`);

CREATE UNIQUE INDEX `IX_AccountToken_token_hash` ON `AccountToken` (`token_hash`);

CREATE INDEX `IX_Availability_resource_id` ON `Availability` (`resource_id`);

CREATE INDEX `IX_Availability_volunteer_profile_id` ON `Availability` (`volunteer_profile_id`);

CREATE INDEX `IX_OrganizationInvitation_public_organization_id` ON `OrganizationInvitation` (`public_organization_id`);

CREATE UNIQUE INDEX `IX_OrganizationInvitation_token_hash` ON `OrganizationInvitation` (`token_hash`);

CREATE INDEX `IX_PublicActorAccount_public_organization_id` ON `PublicActorAccount` (`public_organization_id`);

CREATE UNIQUE INDEX `IX_Reservation_resource_match_id` ON `Reservation` (`resource_match_id`);

CREATE UNIQUE INDEX `IX_Reservation_resource_match_id_public_actor_account_id` ON `Reservation` (`resource_match_id`, `public_actor_account_id`);

CREATE INDEX `IX_ResourceMatch_public_actor_account_id` ON `ResourceMatch` (`public_actor_account_id`);

CREATE INDEX `IX_ResourceMatch_resource_id` ON `ResourceMatch` (`resource_id`);

CREATE INDEX `IX_ResourceMatch_resource_need_id_public_actor_account_id` ON `ResourceMatch` (`resource_need_id`, `public_actor_account_id`);

CREATE INDEX `IX_ResourceNeed_location_id` ON `ResourceNeed` (`location_id`);

CREATE INDEX `IX_ResourceNeed_public_actor_account_id` ON `ResourceNeed` (`public_actor_account_id`);

CREATE INDEX `IX_ResourceNeed_resource_type_id` ON `ResourceNeed` (`resource_type_id`);

CREATE UNIQUE INDEX `IX_Skill_name` ON `Skill` (`name`);

CREATE INDEX `IX_VolunteerInterest_resource_need_id` ON `VolunteerInterest` (`resource_need_id`);

CREATE INDEX `IX_VolunteerInterest_volunteer_profile_id` ON `VolunteerInterest` (`volunteer_profile_id`);

CREATE UNIQUE INDEX `IX_VolunteerProfile_account_id` ON `VolunteerProfile` (`account_id`);

CREATE INDEX `IX_VolunteerProfileSkill_skill_id` ON `VolunteerProfileSkill` (`skill_id`);

INSERT IGNORE INTO ProviderAccount
                (account_id, provider_type, contact_name, phone, address, organization_name, organization_number)
                SELECT account_id, 'Privatperson', contact_name, phone, '', '', '' FROM Account WHERE account_type='Private';
                UPDATE Account SET security_stamp=UUID() WHERE security_stamp='';
                UPDATE Resource SET availability_status='Etter avtale' WHERE availability_status='';
                INSERT INTO Availability (resource_id, volunteer_profile_id, available_from, available_to)
                SELECT resource_id, NULL, NOW(), DATE_ADD(NOW(), INTERVAL 30 DAY) FROM Resource;

ALTER TABLE `Resource` ADD CONSTRAINT `FK_Resource_ProviderAccount_account_id` FOREIGN KEY (`account_id`) REFERENCES `ProviderAccount` (`account_id`) ON DELETE RESTRICT;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261103115153_ExpandedWorkflows', '9.0.0');

CREATE UNIQUE INDEX `IX_ResourceMatch_resource_need_id` ON `ResourceMatch` (`resource_need_id`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261103121226_OneMatchPerNeed', '9.0.0');

COMMIT;


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

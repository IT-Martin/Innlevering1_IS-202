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

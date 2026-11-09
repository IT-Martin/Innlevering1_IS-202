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

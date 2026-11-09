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

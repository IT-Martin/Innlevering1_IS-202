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

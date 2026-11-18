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

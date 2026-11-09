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

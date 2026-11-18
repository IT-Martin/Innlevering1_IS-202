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

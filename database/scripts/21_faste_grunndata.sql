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


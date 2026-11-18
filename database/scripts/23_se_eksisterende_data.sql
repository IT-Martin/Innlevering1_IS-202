-- LESER eksisterende Reachout-data; endrer ingenting.
USE `reachout_database`;
SHOW TABLES;
SELECT r.resource_id,r.resource_name,t.category_name,r.availability_status,r.archived,
       l.address_or_area,l.latitude,l.longitude
FROM Resource r JOIN ResourceType t ON t.resource_type_id=r.resource_type_id
JOIN Location l ON l.location_id=r.location_id ORDER BY r.resource_id;
SELECT resource_need_id,title,priority,status,required_start,required_end FROM ResourceNeed;
SELECT reservation_id,resource_match_id,reservation_status,requested_start,requested_end FROM Reservation;
SELECT volunteer_profile_id,service_area,volunteer_role,archived FROM VolunteerProfile;
SELECT interest_id,volunteer_profile_id,resource_need_id,interest_status,submitted_at FROM VolunteerInterest;

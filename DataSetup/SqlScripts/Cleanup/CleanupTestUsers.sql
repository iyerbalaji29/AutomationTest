-- Sample cleanup script
-- Removes test data after test execution

-- Delete test users
DELETE FROM TestUsers
WHERE Email LIKE '%@example.com';

-- Optionally drop the table
-- DROP TABLE IF EXISTS TestUsers;

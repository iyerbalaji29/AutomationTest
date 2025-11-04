-- Sample SQL script for creating test users
-- This is an example script to demonstrate the data setup structure

-- Create test users table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TestUsers')
BEGIN
    CREATE TABLE TestUsers (
        UserId INT PRIMARY KEY IDENTITY(1,1),
        UserName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(255) NOT NULL,
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE()
    );
END

-- Insert sample test users
IF NOT EXISTS (SELECT * FROM TestUsers WHERE Email = 'testuser1@example.com')
BEGIN
    INSERT INTO TestUsers (UserName, Email, IsActive)
    VALUES ('Test User 1', 'testuser1@example.com', 1);
END

IF NOT EXISTS (SELECT * FROM TestUsers WHERE Email = 'testuser2@example.com')
BEGIN
    INSERT INTO TestUsers (UserName, Email, IsActive)
    VALUES ('Test User 2', 'testuser2@example.com', 1);
END

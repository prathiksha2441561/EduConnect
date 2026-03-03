CREATE DATABASE EduConnectDB;
GO

USE EduConnectDB;
GO

-- Users: login credentials and role (used for both login and registration)
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(100) NOT NULL,
    Role NVARCHAR(20) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    DisplayName NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Courses (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Fee DECIMAL(18,2) NOT NULL
);

CREATE TABLE Students (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NULL FOREIGN KEY REFERENCES Users(Id),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    CourseId INT NULL FOREIGN KEY REFERENCES Courses(Id),
    CONSTRAINT UQ_Students_Email UNIQUE (Email)
);

-- Enrollments: student enrolls in a course
CREATE TABLE Enrollments (
    Id INT PRIMARY KEY IDENTITY(1,1),
    StudentId INT NOT NULL FOREIGN KEY REFERENCES Students(Id),
    CourseId INT NOT NULL FOREIGN KEY REFERENCES Courses(Id),
    EnrolledDate DATETIME NOT NULL DEFAULT GETDATE(),
    Status NVARCHAR(20) NOT NULL DEFAULT 'Enrolled'
);

CREATE TABLE Payments (
    Id INT PRIMARY KEY IDENTITY(1,1),
    EnrollmentId INT NOT NULL FOREIGN KEY REFERENCES Enrollments(Id),
    Amount DECIMAL(18,2) NOT NULL,
    PaidDate DATETIME NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending'
);

CREATE TABLE Grades (
    Id INT PRIMARY KEY IDENTITY(1,1),
    StudentId INT NOT NULL FOREIGN KEY REFERENCES Students(Id),
    CourseId INT NOT NULL FOREIGN KEY REFERENCES Courses(Id),
    GradePoint DECIMAL(4,2) NOT NULL,
    Semester NVARCHAR(50) NULL,
    CONSTRAINT CK_GradePoint CHECK (GradePoint >= 0 AND GradePoint <= 10)
);

INSERT INTO Courses (Name, Fee) VALUES ('B.Tech Computer Science', 150000);
INSERT INTO Courses (Name, Fee) VALUES ('B.Tech Mechanical', 120000);
INSERT INTO Courses (Name, Fee) VALUES ('B.Sc Physics', 80000);

-- Seed student for mock login (student/password) when no DB users exist
INSERT INTO Students (Name, Email, CourseId) VALUES ('John Doe', 'john@edu.com', 1);

GO

SELECT Username, PasswordHash, Role, Email
FROM Users;

DELETE FROM Enrollments WHERE StudentId = 1;
DELETE FROM Students WHERE StudentId = 1;
-- Create the database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'EmployeeManagement')
BEGIN
    CREATE DATABASE EmployeeManagement;
END
GO

USE EmployeeManagement;
GO

-- Create Departments table (every employee belongs to a department)
CREATE TABLE Departments (
    DepartmentId INT PRIMARY KEY IDENTITY(1,1),
    DepartmentName NVARCHAR(100) NOT NULL,
    Budget DECIMAL(18,2),
    CreatedDate DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);
GO

-- Create Employees table (the main table with employee info)
CREATE TABLE Employees (
    EmployeeId INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PhoneNumber NVARCHAR(15),
    HireDate DATE NOT NULL,
    Salary DECIMAL(18,2),
    DepartmentId INT,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME,
    FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId)
);
GO

-- Create an index to make searches faster
CREATE INDEX IX_Employee_LastName ON Employees(LastName);
CREATE INDEX IX_Employee_DepartmentId ON Employees(DepartmentId);
GO
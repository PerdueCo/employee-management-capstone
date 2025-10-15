USE EmployeeManagement;
GO

-- Stored Procedure: Get all active employees
CREATE PROCEDURE sp_GetAllActiveEmployees
AS
BEGIN
    SELECT 
        e.EmployeeId,
        e.FirstName,
        e.LastName,
        e.Email,
        e.PhoneNumber,
        e.HireDate,
        e.Salary,
        d.DepartmentName,
        e.IsActive
    FROM Employees e
    LEFT JOIN Departments d ON e.DepartmentId = d.DepartmentId
    WHERE e.IsActive = 1
    ORDER BY e.LastName, e.FirstName;
END
GO

-- Stored Procedure: Add a new employee
CREATE PROCEDURE sp_AddEmployee
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @Email NVARCHAR(100),
    @PhoneNumber NVARCHAR(15),
    @HireDate DATE,
    @Salary DECIMAL(18,2),
    @DepartmentId INT
AS
BEGIN
    -- Check if email already exists
    IF EXISTS (SELECT 1 FROM Employees WHERE Email = @Email)
    BEGIN
        RAISERROR('Email already exists in the system', 16, 1);
        RETURN;
    END

    INSERT INTO Employees (FirstName, LastName, Email, PhoneNumber, HireDate, Salary, DepartmentId)
    VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @HireDate, @Salary, @DepartmentId);
    
    SELECT SCOPE_IDENTITY() AS NewEmployeeId;
END
GO

-- Stored Procedure: Update employee information
CREATE PROCEDURE sp_UpdateEmployee
    @EmployeeId INT,
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @Email NVARCHAR(100),
    @PhoneNumber NVARCHAR(15),
    @Salary DECIMAL(18,2),
    @DepartmentId INT
AS
BEGIN
    UPDATE Employees
    SET 
        FirstName = @FirstName,
        LastName = @LastName,
        Email = @Email,
        PhoneNumber = @PhoneNumber,
        Salary = @Salary,
        DepartmentId = @DepartmentId,
        ModifiedDate = GETDATE()
    WHERE EmployeeId = @EmployeeId;
    
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- Stored Procedure: Soft delete (mark as inactive instead of deleting)
CREATE PROCEDURE sp_DeactivateEmployee
    @EmployeeId INT
AS
BEGIN
    UPDATE Employees
    SET IsActive = 0, ModifiedDate = GETDATE()
    WHERE EmployeeId = @EmployeeId;
    
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- Stored Procedure: Get employees by department
CREATE PROCEDURE sp_GetEmployeesByDepartment
    @DepartmentId INT
AS
BEGIN
    SELECT 
        e.EmployeeId,
        e.FirstName,
        e.LastName,
        e.Email,
        e.Salary,
        e.HireDate
    FROM Employees e
    WHERE e.DepartmentId = @DepartmentId 
    AND e.IsActive = 1
    ORDER BY e.LastName;
END
GO

-- Stored Procedure: Get department summary with employee count and average salary
CREATE PROCEDURE sp_GetDepartmentSummary
AS
BEGIN
    SELECT 
        d.DepartmentId,
        d.DepartmentName,
        d.Budget,
        COUNT(e.EmployeeId) AS EmployeeCount,
        AVG(e.Salary) AS AverageSalary,
        SUM(e.Salary) AS TotalSalaries
    FROM Departments d
    LEFT JOIN Employees e ON d.DepartmentId = e.DepartmentId AND e.IsActive = 1
    WHERE d.IsActive = 1
    GROUP BY d.DepartmentId, d.DepartmentName, d.Budget
    ORDER BY d.DepartmentName;
END
GO
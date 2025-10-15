USE EmployeeManagement;
GO

-- =============================================
-- REPORT 1: Employee Salary Analysis by Department
-- Shows min, max, avg salary and headcount per department
-- =============================================
CREATE PROCEDURE sp_Report_SalaryAnalysisByDepartment
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        d.DepartmentName,
        COUNT(e.EmployeeId) AS TotalEmployees,
        MIN(e.Salary) AS MinimumSalary,
        MAX(e.Salary) AS MaximumSalary,
        AVG(e.Salary) AS AverageSalary,
        SUM(e.Salary) AS TotalPayroll,
        d.Budget,
        CASE 
            WHEN SUM(e.Salary) > d.Budget THEN 'Over Budget'
            WHEN SUM(e.Salary) > (d.Budget * 0.9) THEN 'Warning'
            ELSE 'Within Budget'
        END AS BudgetStatus,
        CAST((SUM(e.Salary) / NULLIF(d.Budget, 0) * 100) AS DECIMAL(5,2)) AS BudgetUtilization
    FROM Departments d
    LEFT JOIN Employees e ON d.DepartmentId = e.DepartmentId AND e.IsActive = 1
    WHERE d.IsActive = 1
    GROUP BY d.DepartmentId, d.DepartmentName, d.Budget
    ORDER BY TotalPayroll DESC;
END
GO

-- =============================================
-- REPORT 2: Employee Tenure Report
-- Shows how long employees have been with company
-- =============================================
CREATE PROCEDURE sp_Report_EmployeeTenure
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.EmployeeId,
        e.FirstName + ' ' + e.LastName AS EmployeeName,
        e.Email,
        d.DepartmentName,
        e.HireDate,
        DATEDIFF(DAY, e.HireDate, GETDATE()) AS DaysEmployed,
        DATEDIFF(MONTH, e.HireDate, GETDATE()) AS MonthsEmployed,
        DATEDIFF(YEAR, e.HireDate, GETDATE()) AS YearsEmployed,
        e.Salary,
        CASE 
            WHEN DATEDIFF(YEAR, e.HireDate, GETDATE()) < 1 THEN 'New Hire'
            WHEN DATEDIFF(YEAR, e.HireDate, GETDATE()) BETWEEN 1 AND 3 THEN 'Junior'
            WHEN DATEDIFF(YEAR, e.HireDate, GETDATE()) BETWEEN 4 AND 7 THEN 'Mid-Level'
            WHEN DATEDIFF(YEAR, e.HireDate, GETDATE()) BETWEEN 8 AND 15 THEN 'Senior'
            ELSE 'Veteran'
        END AS TenureCategory
    FROM Employees e
    INNER JOIN Departments d ON e.DepartmentId = d.DepartmentId
    WHERE e.IsActive = 1
    ORDER BY DaysEmployed DESC;
END
GO

-- =============================================
-- REPORT 3: Hiring Trends by Month/Year
-- Shows when employees were hired over time
-- =============================================
CREATE PROCEDURE sp_Report_HiringTrends
    @StartYear INT = NULL,
    @EndYear INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Default to last 5 years if not specified
    IF @StartYear IS NULL SET @StartYear = YEAR(GETDATE()) - 5;
    IF @EndYear IS NULL SET @EndYear = YEAR(GETDATE());
    
    SELECT 
        YEAR(e.HireDate) AS HireYear,
        MONTH(e.HireDate) AS HireMonth,
        DATENAME(MONTH, e.HireDate) AS MonthName,
        COUNT(*) AS EmployeesHired,
        AVG(e.Salary) AS AverageStartingSalary,
        STRING_AGG(d.DepartmentName, ', ') AS Departments
    FROM Employees e
    INNER JOIN Departments d ON e.DepartmentId = d.DepartmentId
    WHERE YEAR(e.HireDate) BETWEEN @StartYear AND @EndYear
    GROUP BY YEAR(e.HireDate), MONTH(e.HireDate), DATENAME(MONTH, e.HireDate)
    ORDER BY HireYear DESC, HireMonth DESC;
END
GO

-- =============================================
-- REPORT 4: Salary Distribution Report
-- Shows salary ranges and how many employees in each
-- =============================================
CREATE PROCEDURE sp_Report_SalaryDistribution
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        CASE 
            WHEN Salary < 50000 THEN '1. Under $50K'
            WHEN Salary BETWEEN 50000 AND 59999 THEN '2. $50K - $59K'
            WHEN Salary BETWEEN 60000 AND 69999 THEN '3. $60K - $69K'
            WHEN Salary BETWEEN 70000 AND 79999 THEN '4. $70K - $79K'
            WHEN Salary BETWEEN 80000 AND 89999 THEN '5. $80K - $89K'
            WHEN Salary >= 90000 THEN '6. $90K+'
        END AS SalaryRange,
        COUNT(*) AS EmployeeCount,
        CAST(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM Employees WHERE IsActive = 1) AS DECIMAL(5,2)) AS Percentage,
        MIN(Salary) AS MinSalary,
        MAX(Salary) AS MaxSalary,
        AVG(Salary) AS AvgSalary
    FROM Employees
    WHERE IsActive = 1
    GROUP BY 
        CASE 
            WHEN Salary < 50000 THEN '1. Under $50K'
            WHEN Salary BETWEEN 50000 AND 59999 THEN '2. $50K - $59K'
            WHEN Salary BETWEEN 60000 AND 69999 THEN '3. $60K - $69K'
            WHEN Salary BETWEEN 70000 AND 79999 THEN '4. $70K - $79K'
            WHEN Salary BETWEEN 80000 AND 89999 THEN '5. $80K - $89K'
            WHEN Salary >= 90000 THEN '6. $90K+'
        END
    ORDER BY SalaryRange;
END
GO

-- =============================================
-- REPORT 5: Department Performance Dashboard
-- Comprehensive department metrics
-- =============================================
CREATE PROCEDURE sp_Report_DepartmentDashboard
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        d.DepartmentName,
        d.Budget,
        COUNT(e.EmployeeId) AS ActiveEmployees,
        SUM(e.Salary) AS TotalPayroll,
        AVG(e.Salary) AS AvgSalary,
        MIN(e.Salary) AS MinSalary,
        MAX(e.Salary) AS MaxSalary,
        AVG(DATEDIFF(YEAR, e.HireDate, GETDATE())) AS AvgYearsOfService,
        SUM(CASE WHEN DATEDIFF(YEAR, e.HireDate, GETDATE()) < 1 THEN 1 ELSE 0 END) AS NewHires,
        SUM(CASE WHEN DATEDIFF(YEAR, e.HireDate, GETDATE()) >= 10 THEN 1 ELSE 0 END) AS Veterans,
        CAST((SUM(e.Salary) / NULLIF(d.Budget, 0) * 100) AS DECIMAL(5,2)) AS BudgetUsedPercent,
        CASE 
            WHEN SUM(e.Salary) > d.Budget THEN '? Over Budget'
            WHEN SUM(e.Salary) > (d.Budget * 0.9) THEN '?? Warning'
            ELSE '? Good'
        END AS Status
    FROM Departments d
    LEFT JOIN Employees e ON d.DepartmentId = e.DepartmentId AND e.IsActive = 1
    WHERE d.IsActive = 1
    GROUP BY d.DepartmentId, d.DepartmentName, d.Budget
    ORDER BY d.DepartmentName;
END
GO

-- =============================================
-- REPORT 6: Email Directory Report
-- For contact lists and organizational charts
-- =============================================
CREATE PROCEDURE sp_Report_EmployeeDirectory
    @DepartmentId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.EmployeeId,
        e.LastName + ', ' + e.FirstName AS FullName,
        e.Email,
        e.PhoneNumber,
        d.DepartmentName,
        e.HireDate,
        DATEDIFF(YEAR, e.HireDate, GETDATE()) AS YearsOfService,
        CASE 
            WHEN DATEDIFF(YEAR, e.HireDate, GETDATE()) >= 10 THEN 'Veteran'
            WHEN DATEDIFF(YEAR, e.HireDate, GETDATE()) >= 5 THEN 'Senior'
            WHEN DATEDIFF(YEAR, e.HireDate, GETDATE()) >= 2 THEN 'Experienced'
            ELSE 'New'
        END AS SeniorityLevel
    FROM Employees e
    INNER JOIN Departments d ON e.DepartmentId = d.DepartmentId
    WHERE e.IsActive = 1
        AND (@DepartmentId IS NULL OR d.DepartmentId = @DepartmentId)
    ORDER BY d.DepartmentName, e.LastName, e.FirstName;
END
GO

PRINT 'Reporting stored procedures created successfully!';
USE EmployeeManagement;
GO

-- Add sample departments
INSERT INTO Departments (DepartmentName, Budget) VALUES
('Information Technology', 500000.00),
('Human Resources', 250000.00),
('Finance', 400000.00),
('Operations', 350000.00),
('Customer Service', 200000.00);
GO

-- Add sample employees
INSERT INTO Employees (FirstName, LastName, Email, PhoneNumber, HireDate, Salary, DepartmentId) VALUES
('John', 'Smith', 'john.smith@company.com', '555-0101', '2015-03-15', 75000.00, 1),
('Sarah', 'Johnson', 'sarah.johnson@company.com', '555-0102', '2016-07-22', 68000.00, 2),
('Michael', 'Williams', 'michael.williams@company.com', '555-0103', '2017-01-10', 82000.00, 1),
('Emily', 'Brown', 'emily.brown@company.com', '555-0104', '2018-05-18', 71000.00, 3),
('David', 'Jones', 'david.jones@company.com', '555-0105', '2019-09-03', 65000.00, 4),
('Jennifer', 'Garcia', 'jennifer.garcia@company.com', '555-0106', '2020-02-14', 58000.00, 5),
('Robert', 'Martinez', 'robert.martinez@company.com', '555-0107', '2018-11-27', 79000.00, 1),
('Lisa', 'Anderson', 'lisa.anderson@company.com', '555-0108', '2019-04-08', 62000.00, 2),
('James', 'Taylor', 'james.taylor@company.com', '555-0109', '2020-06-30', 88000.00, 3),
('Mary', 'Thomas', 'mary.thomas@company.com', '555-0110', '2021-01-12', 54000.00, 5);
GO
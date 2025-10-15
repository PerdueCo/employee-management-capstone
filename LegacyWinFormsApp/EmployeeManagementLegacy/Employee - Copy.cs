using System;

namespace EmployeeManagementLegacy
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        //public object EmployeeId { get; internal set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }
        public string DepartmentName { get; set; }
        public bool IsActive { get; set; }

        // This makes it easier to display in lists
        public string FullName => $"{FirstName} {LastName}";

        public override string ToString()
        {
            return $"{FullName} - {DepartmentName}";
        }
    }
}
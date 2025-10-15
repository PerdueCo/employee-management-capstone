using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EmployeeManagementLegacy
{
    public class DatabaseHelper
    {
        // This is how we connect to the database
        private string connectionString = "Server=localhost,1433;Database=EmployeeManagement;User Id=sa;Password=YourStrong@Passw0rd;";

        // Get all active employees
        public List<Employee> GetAllEmployees()
        {
            List<Employee> employees = new List<Employee>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetAllActiveEmployees", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            employees.Add(new Employee
                            {
                                EmployeeId = reader["Employeeid"] != DBNull.Value ? (int)reader["Employeeid"] : 0,
                                PhoneNumber = reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : "",
                                Salary = reader["Salary"] != DBNull.Value ? (decimal)reader["Salary"] : 0,
                                IsActive = reader["IsActive"] != DBNull.Value && (bool)reader["IsActive"],

                               // EmployeeId = (int)reader["Employeeid"],
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Email = reader["Email"].ToString(),
                               // PhoneNumber = reader["PhoneNumber"].ToString(),
                                HireDate = (DateTime)reader["HireDate"],
                              //  Salary = (decimal)reader["Salary"],
                                DepartmentName = reader["DepartmentName"].ToString(),
                               // IsActive = (bool)reader["IsActive"]
                            });
                        }
                    }
                }
            }

            return employees;
        }

        // Add a new employee
        public int AddEmployee(Employee employee, int departmentId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_AddEmployee", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@FirstName", employee.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", employee.LastName);
                    cmd.Parameters.AddWithValue("@Email", employee.Email);
                    cmd.Parameters.AddWithValue("@PhoneNumber", employee.PhoneNumber ?? "");
                    cmd.Parameters.AddWithValue("@HireDate", employee.HireDate);
                    cmd.Parameters.AddWithValue("@Salary", employee.Salary);
                    cmd.Parameters.AddWithValue("@DepartmentId", departmentId);

                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        // Update an existing employee
        public void UpdateEmployee(Employee employee, int departmentId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_UpdateEmployee", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId);
                    cmd.Parameters.AddWithValue("@FirstName", employee.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", employee.LastName);
                    cmd.Parameters.AddWithValue("@Email", employee.Email);
                    cmd.Parameters.AddWithValue("@PhoneNumber", employee.PhoneNumber ?? "");
                    cmd.Parameters.AddWithValue("@Salary", employee.Salary);
                    cmd.Parameters.AddWithValue("@DepartmentId", departmentId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Deactivate an employee (soft delete)
        public void DeactivateEmployee(int employeeId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_DeactivateEmployee", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Get all departments
        public DataTable GetDepartments()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT DepartmentId, DepartmentName FROM Departments WHERE IsActive = 1 ORDER BY DepartmentName";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }

            return dt;
        }
    }
}
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;

//namespace EmployeeManagementLegacy
//{
//    public class DatabaseHelper
//    {
//        // This is how we connect to the database
//        private string connectionString = "Server=localhost,1433;Database=EmployeeManagement;User Id=sa;Password=YourStrong@Passw0rd;";

//        // Get all active employees
//        public List<Employee> GetAllEmployees()
//        {
//            List<Employee> employees = new List<Employee>();

//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                using (SqlCommand cmd = new SqlCommand("sp_GetAllActiveEmployees", conn))
//                {
//                    cmd.CommandType = CommandType.StoredProcedure;

//                    conn.Open();
//                    using (SqlDataReader reader = cmd.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            employees.Add(new Employee
//                            {
//                                EmployeeId = (int)reader["Employeeid"],
//                                FirstName = reader["FirstName"].ToString(),
//                                LastName = reader["LastName"].ToString(),
//                                Email = reader["Email"].ToString(),
//                                PhoneNumber = reader["PhoneNumber"].ToString(),
//                                HireDate = (DateTime)reader["HireDate"],
//                                Salary = (decimal)reader["Salary"],
//                                DepartmentName = reader["DepartmentName"].ToString(),
//                                IsActive = (bool)reader["IsActive"]
//                            });
//                        }
//                    }
//                }
//            }

//            return employees;
//        }

//        // Add a new employee
//        public int AddEmployee(Employee employee, int departmentId)
//        {
//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                using (SqlCommand cmd = new SqlCommand("sp_AddEmployee", conn))
//                {
//                    cmd.CommandType = CommandType.StoredProcedure;

//                    cmd.Parameters.AddWithValue("@FirstName", employee.FirstName);
//                    cmd.Parameters.AddWithValue("@LastName", employee.LastName);
//                    cmd.Parameters.AddWithValue("@Email", employee.Email);
//                    cmd.Parameters.AddWithValue("@PhoneNumber", employee.PhoneNumber ?? "");
//                    cmd.Parameters.AddWithValue("@HireDate", employee.HireDate);
//                    cmd.Parameters.AddWithValue("@Salary", employee.Salary);
//                    cmd.Parameters.AddWithValue("@DepartmentId", departmentId);

//                    conn.Open();
//                    object result = cmd.ExecuteScalar();
//                    return Convert.ToInt32(result);
//                }
//            }
//        }

//        // Update an existing employee
//        public void UpdateEmployee(Employee employee, int departmentId)
//        {
//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                using (SqlCommand cmd = new SqlCommand("sp_UpdateEmployee", conn))
//                {
//                    cmd.CommandType = CommandType.StoredProcedure;

//                    cmd.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId);
//                    cmd.Parameters.AddWithValue("@FirstName", employee.FirstName);
//                    cmd.Parameters.AddWithValue("@LastName", employee.LastName);
//                    cmd.Parameters.AddWithValue("@Email", employee.Email);
//                    cmd.Parameters.AddWithValue("@PhoneNumber", employee.PhoneNumber ?? "");
//                    cmd.Parameters.AddWithValue("@Salary", employee.Salary);
//                    cmd.Parameters.AddWithValue("@DepartmentId", departmentId);

//                    conn.Open();
//                    cmd.ExecuteNonQuery();
//                }
//            }
//        }

//        // Deactivate an employee (soft delete)
//        public void DeactivateEmployee(int employeeId)
//        {
//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                using (SqlCommand cmd = new SqlCommand("sp_DeactivateEmployee", conn))
//                {
//                    cmd.CommandType = CommandType.StoredProcedure;
//                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

//                    conn.Open();
//                    cmd.ExecuteNonQuery();
//                }
//            }
//        }

//        // Get all departments
//        public DataTable GetDepartments()
//        {
//            DataTable dt = new DataTable();

//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                string query = "SELECT DepartmentId, DepartmentName FROM Departments WHERE IsActive = 1 ORDER BY DepartmentName";
//                using (SqlCommand cmd = new SqlCommand(query, conn))
//                {
//                    conn.Open();
//                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
//                    adapter.Fill(dt);
//                }
//            }

//            return dt;
//        }
//    }
//}
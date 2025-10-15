using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace EmployeeManagementLegacy
{
    public partial class Form1 : Form
    {
        private DatabaseHelper dbHelper;
        private DataTable departmentsTable;

        public Form1()
        {
            InitializeComponent();
            dbHelper = new DatabaseHelper();
            LoadDepartments();
            LoadEmployees();
        }

        private void LoadDepartments()
        {
            try
            {
                departmentsTable = dbHelper.GetDepartments();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading departments: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadEmployees()
        {
            try
            {
                dgvEmployees.DataSource = dbHelper.GetAllEmployees();
                dgvEmployees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Simple, safe column hiding/resizing
                if (dgvEmployees.Columns.Count > 0)
                {
                    foreach (DataGridViewColumn col in dgvEmployees.Columns)
                    {
                        if (col.Name == "IsActive")
                            col.Visible = false;

                        if (col.Name == "Employeeid")
                            col.Width = 85;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employees: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //try
            //{
            //    dgvEmployees.DataSource = dbHelper.GetAllEmployees();
            //    dgvEmployees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            //    // Hide IsActive if it exists
            //    foreach (DataGridViewColumn col in dgvEmployees.Columns)
            //    {
            //        if (col.Name.Equals("IsActive", StringComparison.OrdinalIgnoreCase))
            //            col.Visible = false;
            //    }

            //    // Set width for EmployeeId if it exists
            //    var idColumn = dgvEmployees.Columns
            //        .Cast<DataGridViewColumn>()
            //        .FirstOrDefault(c => c.Name.Equals("EmployeeId", StringComparison.OrdinalIgnoreCase));

            //    if (idColumn != null)
            //        idColumn.Width = 85;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Error loading employees: {ex.Message}", "Error",
            //        MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            //try
            //{
            //    dgvEmployees.DataSource = dbHelper.GetAllEmployees();
            //    dgvEmployees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            //    if (dgvEmployees.Columns.Contains("IsActive"))
            //        dgvEmployees.Columns["IsActive"].Visible = false;
            //    if (dgvEmployees.Columns.Contains("Employeeid"))
            //        dgvEmployees.Columns["Employeeid"].Width = 80;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Error loading employees: {ex.Message}", "Error",
            //        MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadEmployees();
            MessageBox.Show("Employee list refreshed!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                EmployeeForm employeeForm = new EmployeeForm(dbHelper, departmentsTable);

                if (employeeForm.ShowDialog() == DialogResult.OK)
                {
                    Employee newEmployee = employeeForm.Employee;
                    int departmentId = employeeForm.SelectedDepartmentId;

                    int newId = dbHelper.AddEmployee(newEmployee, departmentId);

                    MessageBox.Show($"Employee '{newEmployee.FirstName} {newEmployee.LastName}' added successfully!\nEmployee ID: {newId}",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadEmployees();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding employee: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

     

       

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void dgvEmployees_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void btnEdit_Click(object sender, EventArgs e)
           
        {
            if (dgvEmployees.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an employee to edit", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DataGridViewRow row = dgvEmployees.SelectedRows[0];

                Employee currentEmployee = new Employee
                {
                    EmployeeId = (int)row.Cells["Employeeid"].Value,
                    FirstName = row.Cells["FirstName"].Value.ToString(),
                    LastName = row.Cells["LastName"].Value.ToString(),
                    Email = row.Cells["Email"].Value.ToString(),
                    PhoneNumber = row.Cells["PhoneNumber"].Value?.ToString() ?? "",
                    HireDate = (DateTime)row.Cells["HireDate"].Value,
                    Salary = (decimal)row.Cells["Salary"].Value,
                    DepartmentName = row.Cells["DepartmentName"].Value.ToString()
                };

                int currentDeptId = 1;
                foreach (DataRow deptRow in departmentsTable.Rows)
                {
                    if (deptRow["DepartmentName"].ToString() == currentEmployee.DepartmentName)
                    {
                        currentDeptId = (int)deptRow["DepartmentId"];
                        break;
                    }
                }

                EmployeeForm employeeForm = new EmployeeForm(dbHelper, departmentsTable, currentEmployee, currentDeptId);

                if (employeeForm.ShowDialog() == DialogResult.OK)
                {
                    Employee updatedEmployee = employeeForm.Employee;
                    int departmentId = employeeForm.SelectedDepartmentId;

                    dbHelper.UpdateEmployee(updatedEmployee, departmentId);

                    MessageBox.Show($"Employee '{updatedEmployee.FirstName} {updatedEmployee.LastName}' updated successfully!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadEmployees();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating employee: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an employee to deactivate", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DataGridViewRow row = dgvEmployees.SelectedRows[0];
                int employeeId = (int)row.Cells["EmployeeId"].Value;
                string fullName = row.Cells["FullName"].Value.ToString();

                var result = MessageBox.Show(
                    $"Are you sure you want to deactivate '{fullName}'?\n\nThis will mark them as inactive but keep their records.",
                    "Confirm Deactivation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    dbHelper.DeactivateEmployee(employeeId);
                    MessageBox.Show($"Employee '{fullName}' has been deactivated successfully!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadEmployees();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deactivating employee: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

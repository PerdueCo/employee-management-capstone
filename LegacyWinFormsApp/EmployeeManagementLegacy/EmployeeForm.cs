using System;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace EmployeeManagementLegacy
{
    public partial class EmployeeForm : Form
    {
        private DatabaseHelper dbHelper;
        private DataTable departmentsTable;
        private Employee existingEmployee; // For edit mode
        private bool isEditMode;

        // Controls
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private TextBox txtEmail;
        private TextBox txtPhoneNumber;
        private DateTimePicker dtpHireDate;
        private TextBox txtSalary;
        private ComboBox cboDepartment;

        private Label lblFirstNameError;
        private Label lblLastNameError;
        private Label lblEmailError;
        private Label lblSalaryError;

        private Button btnSave;
        private Button btnCancel;

        public Employee Employee { get; private set; }
        public int SelectedDepartmentId { get; private set; }

        // Constructor for ADD mode
        public EmployeeForm(DatabaseHelper helper, DataTable departments)
        {
            dbHelper = helper;
            departmentsTable = departments;
            isEditMode = false;
            InitializeForm();
            this.Text = "Add New Employee";
        }

        // Constructor for EDIT mode
        public EmployeeForm(DatabaseHelper helper, DataTable departments, Employee employee, int currentDeptId)
        {
            dbHelper = helper;
            departmentsTable = departments;
            existingEmployee = employee;
            isEditMode = true;
            InitializeForm();
            PopulateFields();
            this.Text = "Edit Employee";
        }

        private void InitializeForm()
        {
            // Form settings
            this.Width = 500;
            this.Height = 550;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            int labelWidth = 120;
            int controlLeft = 140;
            int controlWidth = 320;
            int currentTop = 20;
            int spacing = 60;

            // First Name
            Label lblFirstName = new Label
            {
                Text = "First Name: *",
                Left = 20,
                Top = currentTop + 3,
                Width = labelWidth,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            txtFirstName = new TextBox
            {
                Left = controlLeft,
                Top = currentTop,
                Width = controlWidth,
                Font = new Font("Segoe UI", 10)
            };
            lblFirstNameError = new Label
            {
                Left = controlLeft,
                Top = currentTop + 25,
                Width = controlWidth,
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 8),
                Text = ""
            };
            currentTop += spacing;

            // Last Name
            Label lblLastName = new Label
            {
                Text = "Last Name: *",
                Left = 20,
                Top = currentTop + 3,
                Width = labelWidth,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            txtLastName = new TextBox
            {
                Left = controlLeft,
                Top = currentTop,
                Width = controlWidth,
                Font = new Font("Segoe UI", 10)
            };
            lblLastNameError = new Label
            {
                Left = controlLeft,
                Top = currentTop + 25,
                Width = controlWidth,
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 8),
                Text = ""
            };
            currentTop += spacing;

            // Email
            Label lblEmail = new Label
            {
                Text = "Email: *",
                Left = 20,
                Top = currentTop + 3,
                Width = labelWidth,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            txtEmail = new TextBox
            {
                Left = controlLeft,
                Top = currentTop,
                Width = controlWidth,
                Font = new Font("Segoe UI", 10)
            };
            lblEmailError = new Label
            {
                Left = controlLeft,
                Top = currentTop + 25,
                Width = controlWidth,
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 8),
                Text = ""
            };
            currentTop += spacing;

            // Phone Number
            Label lblPhoneNumber = new Label
            {
                Text = "Phone Number:",
                Left = 20,
                Top = currentTop + 3,
                Width = labelWidth,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            txtPhoneNumber = new TextBox
            {
                Left = controlLeft,
                Top = currentTop,
                Width = controlWidth,
                Font = new Font("Segoe UI", 10)
            };
            currentTop += 40;

            // Hire Date
            Label lblHireDate = new Label
            {
                Text = "Hire Date: *",
                Left = 20,
                Top = currentTop + 3,
                Width = labelWidth,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            dtpHireDate = new DateTimePicker
            {
                Left = controlLeft,
                Top = currentTop,
                Width = controlWidth,
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };
            currentTop += 40;

            // Salary
            Label lblSalary = new Label
            {
                Text = "Salary: *",
                Left = 20,
                Top = currentTop + 3,
                Width = labelWidth,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            txtSalary = new TextBox
            {
                Left = controlLeft,
                Top = currentTop,
                Width = controlWidth,
                Font = new Font("Segoe UI", 10)
            };
            lblSalaryError = new Label
            {
                Left = controlLeft,
                Top = currentTop + 25,
                Width = controlWidth,
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 8),
                Text = ""
            };
            currentTop += spacing;

            // Department
            Label lblDepartment = new Label
            {
                Text = "Department: *",
                Left = 20,
                Top = currentTop + 3,
                Width = labelWidth,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            cboDepartment = new ComboBox
            {
                Left = controlLeft,
                Top = currentTop,
                Width = controlWidth,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "DepartmentName",
                ValueMember = "DepartmentId",
                DataSource = departmentsTable
            };
            currentTop += 50;

            // Required field note
            Label lblRequired = new Label
            {
                Text = "* Required fields",
                Left = 20,
                Top = currentTop,
                Width = 200,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9, FontStyle.Italic)
            };
            currentTop += 30;

            // Buttons
            btnSave = new Button
            {
                Text = isEditMode ? "Update" : "Save",
                Left = 280,
                Top = currentTop,
                Width = 90,
                Height = 35,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 120, 212),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Left = 380,
                Top = currentTop,
                Width = 90,
                Height = 35,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(230, 230, 230),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += BtnCancel_Click;

            // Add all controls to form
            this.Controls.AddRange(new Control[]
            {
                lblFirstName, txtFirstName, lblFirstNameError,
                lblLastName, txtLastName, lblLastNameError,
                lblEmail, txtEmail, lblEmailError,
                lblPhoneNumber, txtPhoneNumber,
                lblHireDate, dtpHireDate,
                lblSalary, txtSalary, lblSalaryError,
                lblDepartment, cboDepartment,
                lblRequired,
                btnSave, btnCancel
            });

            // Add real-time validation
            txtFirstName.TextChanged += (s, e) => ValidateField(txtFirstName, lblFirstNameError, "First name is required");
            txtLastName.TextChanged += (s, e) => ValidateField(txtLastName, lblLastNameError, "Last name is required");
            txtEmail.TextChanged += (s, e) => ValidateEmail();
            txtSalary.TextChanged += (s, e) => ValidateSalary();
        }

        private void PopulateFields()
        {
            if (existingEmployee != null)
            {
                txtFirstName.Text = existingEmployee.FirstName;
                txtLastName.Text = existingEmployee.LastName;
                txtEmail.Text = existingEmployee.Email;
                txtPhoneNumber.Text = existingEmployee.PhoneNumber;
                dtpHireDate.Value = existingEmployee.HireDate;
                txtSalary.Text = existingEmployee.Salary.ToString("F2");

                // Set department (will be selected from the database helper)
                if (cboDepartment.Items.Count > 0)
                    cboDepartment.SelectedIndex = 0;
            }
        }

        private void ValidateField(TextBox textBox, Label errorLabel, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                errorLabel.Text = errorMessage;
                textBox.BackColor = Color.FromArgb(255, 240, 240);
            }
            else
            {
                errorLabel.Text = "";
                textBox.BackColor = Color.White;
            }
        }

        private void ValidateEmail()
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                lblEmailError.Text = "Email is required";
                txtEmail.BackColor = Color.FromArgb(255, 240, 240);
            }
            else if (!IsValidEmail(txtEmail.Text))
            {
                lblEmailError.Text = "Invalid email format (e.g., name@company.com)";
                txtEmail.BackColor = Color.FromArgb(255, 240, 240);
            }
            else
            {
                lblEmailError.Text = "";
                txtEmail.BackColor = Color.White;
            }
        }

        private void ValidateSalary()
        {
            if (string.IsNullOrWhiteSpace(txtSalary.Text))
            {
                lblSalaryError.Text = "Salary is required";
                txtSalary.BackColor = Color.FromArgb(255, 240, 240);
            }
            else if (!decimal.TryParse(txtSalary.Text, out decimal salary))
            {
                lblSalaryError.Text = "Salary must be a valid number";
                txtSalary.BackColor = Color.FromArgb(255, 240, 240);
            }
            else if (salary < 0)
            {
                lblSalaryError.Text = "Salary cannot be negative";
                txtSalary.BackColor = Color.FromArgb(255, 240, 240);
            }
            else
            {
                lblSalaryError.Text = "";
                txtSalary.BackColor = Color.White;
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            // Validate First Name
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                lblFirstNameError.Text = "First name is required";
                txtFirstName.BackColor = Color.FromArgb(255, 240, 240);
                isValid = false;
            }

            // Validate Last Name
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                lblLastNameError.Text = "Last name is required";
                txtLastName.BackColor = Color.FromArgb(255, 240, 240);
                isValid = false;
            }

            // Validate Email
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                lblEmailError.Text = "Email is required";
                txtEmail.BackColor = Color.FromArgb(255, 240, 240);
                isValid = false;
            }
            else if (!IsValidEmail(txtEmail.Text))
            {
                lblEmailError.Text = "Invalid email format";
                txtEmail.BackColor = Color.FromArgb(255, 240, 240);
                isValid = false;
            }

            // Validate Salary
            if (string.IsNullOrWhiteSpace(txtSalary.Text))
            {
                lblSalaryError.Text = "Salary is required";
                txtSalary.BackColor = Color.FromArgb(255, 240, 240);
                isValid = false;
            }
            else if (!decimal.TryParse(txtSalary.Text, out decimal salary) || salary < 0)
            {
                lblSalaryError.Text = "Salary must be a valid positive number";
                txtSalary.BackColor = Color.FromArgb(255, 240, 240);
                isValid = false;
            }

            // Validate Department
            if (cboDepartment.SelectedValue == null)
            {
                MessageBox.Show("Please select a department", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                isValid = false;
            }

            return isValid;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
            {
                MessageBox.Show("Please correct the errors before saving.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Create employee object
                Employee = new Employee
                {
                    FirstName = txtFirstName.Text.Trim(),
                    LastName = txtLastName.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    PhoneNumber = string.IsNullOrWhiteSpace(txtPhoneNumber.Text) ? "" : txtPhoneNumber.Text.Trim(),
                    HireDate = dtpHireDate.Value,
                    Salary = decimal.Parse(txtSalary.Text)
                };

                if (isEditMode)
                {
                    Employee.EmployeeId = existingEmployee.EmployeeId;
                }

                SelectedDepartmentId = (int)cboDepartment.SelectedValue;

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving employee: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // EmployeeForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "EmployeeForm";
            this.Load += new System.EventHandler(this.EmployeeForm_Load);
            this.ResumeLayout(false);

        }

        private void EmployeeForm_Load(object sender, EventArgs e)
        {

        }
    }
}
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace EmployeeManagementLegacy
{
    public partial class ReportsForm : Form
    {
        private ReportHelper reportHelper;
        private ComboBox cboReportType;
        private DataGridView dgvReport;
        private Button btnGenerate;
        private Button btnExportCSV;
        private Button btnExportHTML;
        private Label lblRecordCount;

        public ReportsForm()
        {
            reportHelper = new ReportHelper();
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = "Employee Reports";
            this.Width = 1000;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterParent;

            // Report Type Dropdown
            Label lblReport = new Label
            {
                Text = "Select Report:",
                Left = 20,
                Top = 20,
                Width = 100,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            cboReportType = new ComboBox
            {
                Left = 130,
                Top = 18,
                Width = 350,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            cboReportType.Items.AddRange(new object[]
            {
                "Salary Analysis by Department",
                "Employee Tenure Report",
                "Hiring Trends",
                "Salary Distribution",
                "Department Dashboard",
                "Employee Directory"
            });
            cboReportType.SelectedIndex = 0;

            // Generate Button
            btnGenerate = new Button
            {
                Text = "Generate Report",
                Left = 500,
                Top = 15,
                Width = 140,
                Height = 30,
                BackColor = Color.FromArgb(0, 120, 212),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnGenerate.FlatAppearance.BorderSize = 0;
            btnGenerate.Click += BtnGenerate_Click;

            // Export Buttons
            btnExportCSV = new Button
            {
                Text = "Export CSV",
                Left = 660,
                Top = 15,
                Width = 120,
                Height = 30,
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnExportCSV.FlatAppearance.BorderSize = 0;
            btnExportCSV.Click += BtnExportCSV_Click;

            btnExportHTML = new Button
            {
                Text = "Export HTML",
                Left = 790,
                Top = 15,
                Width = 120,
                Height = 30,
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnExportHTML.FlatAppearance.BorderSize = 0;
            btnExportHTML.Click += BtnExportHTML_Click;

            // Record Count Label
            lblRecordCount = new Label
            {
                Left = 20,
                Top = 60,
                Width = 300,
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.Gray
            };

            // DataGridView
            dgvReport = new DataGridView
            {
                Left = 20,
                Top = 90,
                Width = 940,
                Height = 450,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            this.Controls.AddRange(new Control[]
            {
                lblReport, cboReportType, btnGenerate,
                btnExportCSV, btnExportHTML,
                lblRecordCount, dgvReport
            });
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = null;
                string reportName = cboReportType.SelectedItem.ToString();

                switch (cboReportType.SelectedIndex)
                {
                    case 0: // Salary Analysis
                        dt = reportHelper.GetSalaryAnalysisReport();
                        break;
                    case 1: // Employee Tenure
                        dt = reportHelper.GetEmployeeTenureReport();
                        break;
                    case 2: // Hiring Trends
                        dt = reportHelper.GetHiringTrendsReport();
                        break;
                    case 3: // Salary Distribution
                        dt = reportHelper.GetSalaryDistributionReport();
                        break;
                    case 4: // Department Dashboard
                        dt = reportHelper.GetDepartmentDashboardReport();
                        break;
                    case 5: // Employee Directory
                        dt = reportHelper.GetEmployeeDirectoryReport();
                        break;
                }

                dgvReport.DataSource = dt;
                lblRecordCount.Text = $"Report: {reportName} | Records: {dt.Rows.Count} | Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExportCSV_Click(object sender, EventArgs e)
        {
            if (dgvReport.DataSource == null)
            {
                MessageBox.Show("Please generate a report first", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DataTable dt = (DataTable)dgvReport.DataSource;
                string fileName = $"Report_{cboReportType.SelectedItem.ToString().Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                string filePath = reportHelper.ExportToCSV(dt, fileName);

                MessageBox.Show($"Report exported successfully!\n\nLocation: {filePath}", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                Process.Start("explorer.exe", $"/select,\"{filePath}\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to CSV: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExportHTML_Click(object sender, EventArgs e)
        {
            if (dgvReport.DataSource == null)
            {
                MessageBox.Show("Please generate a report first", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DataTable dt = (DataTable)dgvReport.DataSource;
                string reportTitle = cboReportType.SelectedItem.ToString();
                string html = reportHelper.GenerateHTMLReport(dt, reportTitle);

                string fileName = $"Report_{reportTitle.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.html";
                string filePath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    fileName);

                System.IO.File.WriteAllText(filePath, html);

                MessageBox.Show($"Report exported successfully!\n\nLocation: {filePath}", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                Process.Start(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to HTML: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ReportsForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "ReportsForm";
            this.Load += new System.EventHandler(this.ReportsForm_Load);
            this.ResumeLayout(false);

        }

        private void ReportsForm_Load(object sender, EventArgs e)
        {

        }
    }
}
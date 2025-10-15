using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace EmployeeManagementLegacy
{
    public class ReportHelper
    {
        private string connectionString = "Server=localhost,1433;Database=EmployeeManagement;User Id=sa;Password=YourStrong@Passw0rd;";

        // Report 1: Salary Analysis by Department
        public DataTable GetSalaryAnalysisReport()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_Report_SalaryAnalysisByDepartment", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        // Report 2: Employee Tenure Report
        public DataTable GetEmployeeTenureReport()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_Report_EmployeeTenure", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        // Report 3: Hiring Trends
        public DataTable GetHiringTrendsReport(int? startYear = null, int? endYear = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_Report_HiringTrends", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (startYear.HasValue)
                        cmd.Parameters.AddWithValue("@StartYear", startYear.Value);
                    if (endYear.HasValue)
                        cmd.Parameters.AddWithValue("@EndYear", endYear.Value);

                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        // Report 4: Salary Distribution
        public DataTable GetSalaryDistributionReport()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_Report_SalaryDistribution", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        // Report 5: Department Dashboard
        public DataTable GetDepartmentDashboardReport()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_Report_DepartmentDashboard", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        // Report 6: Employee Directory
        public DataTable GetEmployeeDirectoryReport(int? departmentId = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_Report_EmployeeDirectory", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (departmentId.HasValue)
                        cmd.Parameters.AddWithValue("@DepartmentId", departmentId.Value);

                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        // Export report to CSV
        public string ExportToCSV(DataTable dt, string fileName)
        {
            StringBuilder sb = new StringBuilder();

            // Add column headers
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sb.Append(dt.Columns[i].ColumnName);
                if (i < dt.Columns.Count - 1)
                    sb.Append(",");
            }
            sb.AppendLine();

            // Add rows
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string value = row[i].ToString().Replace(",", ";");
                    sb.Append(value);
                    if (i < dt.Columns.Count - 1)
                        sb.Append(",");
                }
                sb.AppendLine();
            }

            string filePath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                fileName);

            System.IO.File.WriteAllText(filePath, sb.ToString());
            return filePath;
        }

        // Generate HTML Report
        public string GenerateHTMLReport(DataTable dt, string reportTitle)
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html><head>");
            html.AppendLine("<title>" + reportTitle + "</title>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
            html.AppendLine("h1 { color: #0078d4; }");
            html.AppendLine("table { border-collapse: collapse; width: 100%; margin-top: 20px; }");
            html.AppendLine("th { background-color: #0078d4; color: white; padding: 12px; text-align: left; }");
            html.AppendLine("td { border: 1px solid #ddd; padding: 8px; }");
            html.AppendLine("tr:nth-child(even) { background-color: #f2f2f2; }");
            html.AppendLine("tr:hover { background-color: #e8f4ff; }");
            html.AppendLine(".generated { color: #666; font-size: 12px; margin-top: 20px; }");
            html.AppendLine("</style>");
            html.AppendLine("</head><body>");
            html.AppendLine($"<h1>{reportTitle}</h1>");
            html.AppendLine($"<p class='generated'>Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
            html.AppendLine("<table>");

            // Headers
            html.AppendLine("<tr>");
            foreach (DataColumn col in dt.Columns)
            {
                html.AppendLine($"<th>{col.ColumnName}</th>");
            }
            html.AppendLine("</tr>");

            // Rows
            foreach (DataRow row in dt.Rows)
            {
                html.AppendLine("<tr>");
                foreach (var item in row.ItemArray)
                {
                    html.AppendLine($"<td>{item}</td>");
                }
                html.AppendLine("</tr>");
            }

            html.AppendLine("</table>");
            html.AppendLine("</body></html>");

            return html.ToString();
        }
    }
}
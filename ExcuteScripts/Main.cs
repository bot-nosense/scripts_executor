using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using System.IO;
using System.Diagnostics;

namespace ExcuteScripts
{
    public partial class Main : Form
    {
        private string connectionString;
        private string dataFolderPath;
        private string logFolder;
        private string logFilePath;

        public Main()
        {
            InitializeComponent();
            connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=ideapad3)(PORT=1522)))(CONNECT_DATA=(SID=db)));User Id=sys;Password=@nHhung123;DBA Privilege=SYSDBA;";
            dataFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datas");
            logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            logFilePath = Path.Combine(logFolder, "log.txt");

            Directory.CreateDirectory(dataFolderPath);
            Directory.CreateDirectory(logFolder);
            StreamWriter sw = File.CreateText(logFilePath);
        }

        private void LogToTextFile(string fileName, bool success)
        {
            try
            {
                string logInfo = $"{DateTime.Now.ToString("yyyyMMdd-HH:mm")}, {fileName}, {(success ? "chạy thành công" : "chạy thất bại")}";

                using (StreamWriter sw = File.AppendText(logFilePath))
                {
                    sw.WriteLine(logInfo);
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ khi ghi log
                Console.WriteLine("Error writing log: " + ex.Message);
            }
        }


        private void bt_conn_Click(object sender, EventArgs e)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(connectionString))
                {
                    con.Open();
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        tb_stt.Text = "Kết nối thành công";
                    }
                    else
                    {
                        tb_stt.Text = "Không thể kết nối";
                    }
                }
            }
            catch (Exception ex)
            {
                tb_stt.Text = "Lỗi: " + ex.Message;
            }
        }

        private void bt_import_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    lb_Data.Text = dataFolderPath;
                    tb_view_ip.Clear();

                    foreach (string file in openFileDialog.FileNames)
                    {
                        if (Path.GetExtension(file).Equals(".sql", StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                string destinationFile = Path.Combine(dataFolderPath, Path.GetFileName(file));
                                File.Copy(file, destinationFile, true);
                                tb_view_ip.AppendText(Path.GetFileName(file) + Environment.NewLine);
                            }
                            catch (Exception ex)
                            {
                                tb_stt.Text = "Lỗi khi sao chép file: " + ex.Message;
                                return;
                            }
                        }
                        else
                        {
                            tb_stt.Text = "Chỉ được phép import các file có đuôi là .sql";
                        }
                    }

                    tb_stt.Text = "Đã import file.";
                }
            }
        }

        private void bt_submit_Click(object sender, EventArgs e)
        {
            try
            {
                string[] sqlFiles = Directory.GetFiles(dataFolderPath, "*.sql");

                foreach (string file in sqlFiles)
                {
                    RunSqlPlusScript(file, connectionString);
                    string fileName = Path.GetFileName(file);
                    LogToTextFile(fileName, true);
                }
                tb_stt.Text = "Chạy thành công, đã ghi vào file logs";
            }
            catch (Exception ex)
            {
                tb_stt.Text = "Error executing scripts: " + ex.Message;
            }
        }

        private void RunSqlPlusScript(string scriptFilePath, string connectionString)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "sqlplus";
                startInfo.Arguments = connectionString + " @" + scriptFilePath;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.CreateNoWindow = true;

                using (Process process = new Process())
                {
                    process.StartInfo = startInfo;
                    process.Start();

                    // Đọc kết quả trả về từ quá trình SQLPlus
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(output))
                    {
                        Console.WriteLine("Output: " + output);
                    }

                    if (!string.IsNullOrEmpty(error))
                    {
                        Console.WriteLine("Error: " + error);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }


        private void bt_clear_Click(object sender, EventArgs e)
        {
            try
            {
                string[] files = Directory.GetFiles(dataFolderPath);

                foreach (string file in files)
                {
                    File.Delete(file);
                }

                tb_stt.Text = "Đã xóa tất cả các file trong thư mục Data.";
                tb_view_ip.Clear();
            }
            catch (Exception ex)
            {
                tb_stt.Text = "Lỗi khi xóa file: " + ex.Message;
            }
        }

        private void bt_log_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(logFilePath);
                tb_stt.Text = "Đang mở file ...";
                lb_Data.Text = logFilePath;
            }
            catch (Exception ex)
            {
                tb_stt.Text = "Lỗi khi mở file log: " + ex.Message;
            }
        }
    }
}





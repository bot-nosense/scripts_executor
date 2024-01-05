﻿using System;
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
using ExcuteScripts.DataAccess.OracleDatabase;

namespace ExcuteScripts
{
    public partial class Main : Form
    {
        private string dataFolderPath;
        private string logFolder;
        private string logFilePath;
        private OracleDBManager dbManager;
        private OracleConnection connection;

        public Main()
        {
            InitializeComponent();
            dbManager = new OracleDBManager();
            dbManager.SetConnectionParameters("ideapad3", 1521, "vdoandb", "sys", "@nHhung123", true);
            connection = dbManager.GetConnection();
            dataFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datas");
            logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            logFilePath = Path.Combine(logFolder, "log.txt");

            Directory.CreateDirectory(dataFolderPath);
            Directory.CreateDirectory(logFolder);
            StreamWriter sw = File.CreateText(logFilePath);

            ClearFolder(dataFolderPath);
        }

        private void ClearFolder(string dataFolderPath)
        {
            string[] files = Directory.GetFiles(dataFolderPath);

            foreach (string file in files)
            {
                File.Delete(file);
            }
        }

        private void LogToTextFile(string fileName, string success) // "success" : "fail"
        {
            try
            {
                string logInfo = $"{DateTime.Now.ToString("yyyyMMdd-HH:mm")}, {fileName}, {success}";

                using (StreamWriter sw = File.AppendText(logFilePath))
                {
                    sw.WriteLine(logInfo);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing log: " + ex.Message);
            }
        }

        private void bt_conn_Click(object sender, EventArgs e)
        {
            try
            {
                dbManager.OpenConnection();
                if (dbManager.GetConnection().State == System.Data.ConnectionState.Open)
                {
                    tb_stt.Text = "Kết nối thành công";
                    LogToTextFile("Connect", "success");
                }
                else
                {
                    tb_stt.Text = "Không thể kết nối";
                    LogToTextFile("Connect", "fail");
                    dbManager.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                tb_stt.Text = "Lỗi: " + ex.Message;
                LogToTextFile("Error connect database: " + ex.Message + "\nConnect", "fail");
                dbManager.CloseConnection();
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
                                LogToTextFile("Import file", "fail");
                                return;
                            }
                        }
                        else
                        {
                            tb_stt.Text = "Chỉ được phép import các file có đuôi là .sql";
                        }
                    }

                    tb_stt.Text = "Đã import file.";
                    LogToTextFile("Import file", "success");
                }
            }
        }

        private void bt_submit_Click(object sender, EventArgs e)
        {
            try
            {
                string[] sqlFiles = Directory.GetFiles(dataFolderPath, "*.sql");

                if (sqlFiles.Length == 0)
                {
                    tb_stt.Text = "Không có tệp .sql để thực thi";
                    return;
                }

                foreach (string file in sqlFiles)
                {
                    RunSqlPlusScript(file, connection);
                    string fileName = Path.GetFileName(file);
                    LogToTextFile("Executing script" + fileName, "success");
                }
                tb_stt.Text = "Chạy thành công, đã ghi vào file logs";
            }
            catch (Exception ex)
            {
                tb_stt.Text = "Error executing scripts: " + ex.Message;
                LogToTextFile("Error executing scripts: " + ex.Message + "\nExecuting script", "fail");
            }
        }

        private void RunCommandScript(string scriptFilePath, string connectionString)
        {
            using (OracleConnection con = new OracleConnection(connectionString))
                try
                {
                    con.Open();
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        string sqlScript = System.IO.File.ReadAllText(logFilePath);
                        using (OracleCommand cmd = new OracleCommand(sqlScript, con))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        tb_stt.Text = "Kết nối thành công";
                        LogToTextFile("Connect", "success");
                    }
                    else
                    {
                        tb_stt.Text = "Không thể kết nối";
                        LogToTextFile("Connect", "fail");
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    tb_stt.Text = "Lỗi: " + ex.Message;
                    LogToTextFile("Error connect database: " + ex.Message + "\nConnect", "fail");
                    con.Close();
                }
        }

        private void RunSqlPlusScript(string scriptFilePath, OracleConnection connectionString)
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

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(output))
                    {
                        Console.WriteLine("Output: " + output);
                        LogToTextFile("Output: " + output + "\nRun script on sqlplus", "fail");
                    }

                    if (!string.IsNullOrEmpty(error))
                    {
                        Console.WriteLine("Error: " + error);
                        LogToTextFile("Error: " + error + "\nRun script on sqlplus", "fail");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                LogToTextFile("Exception: " + ex.Message + "\nRun script on sqlplus", "fail");
            }
        }


        private void bt_clear_Click(object sender, EventArgs e)
        {
            try
            {
                ClearFolder(dataFolderPath);

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





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
using ExcuteScripts.DataAccess.OracleDatabase;
using ExcuteScripts.Config;

namespace ExcuteScripts
{
    public partial class Main : Form
    {
        private string dataFolderPath;
        private string logFullPath;
        private OracleDBManager dbManager;

        public Main()
        {
            InitializeComponent();
            Dictionary<string, string> dbConfig = ConstantsReader.ReadConstantsFromFile(Path.GetFullPath(Constants.DBCONFIGPATH));

            string host = dbConfig["HOST"];
            string port = dbConfig["PORT"];
            string sid = dbConfig["SID"];
            string userId = dbConfig["USER_ID"];
            string password = dbConfig["PASSWORD"];
            bool sysDba = true;

            dbManager = new OracleDBManager();
            dbManager.SetConnectionParameters( host, port, sid, userId, password, sysDba);
            dataFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datas");
            Directory.CreateDirectory(dataFolderPath);
            logFullPath = Path.GetFullPath(Constants.LOGFILEPATH);

            ClearFolder(dataFolderPath);

            //tb_stt.Text = int.Parse(dbConfig["PORT"]);
            WriteToLogFile("--------", "");
            WriteToLogFile(" \t \t \t NEW SEESION", "");
            WriteToLogFile("Login Param: "+ dbConfig["HOST"] + ", " + dbConfig["PORT"] + ", " + dbConfig["SERVICE_NAME"] + ", " + dbConfig["USER_ID"] + ", " + dbConfig["PASSWORD"], "");
        }

        private void ClearFolder(string dataFolderPath)
        {
            string[] files = Directory.GetFiles(dataFolderPath);

            foreach (string file in files)
            {
                File.Delete(file);
            }
        }

        private void WriteToLogFile(string info, string detail) 
        {
            try
            {
                string logInfo = $"{DateTime.Now.ToString("yyyyMMdd-HH:mm")}, {info}, {detail}";

                using (StreamWriter sw = File.AppendText(logFullPath))
                {
                    sw.WriteLine(logInfo);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing log: " + ex.Message);
            }
        }

        private void ReturnStatus(string info, string detail = "")
        { 
            tb_stt.Text = info;
            WriteToLogFile(info, detail);
        }

        private void bt_conn_Click(object sender, EventArgs e)
        {
            try
            {
                dbManager.OpenConnection();
                if (dbManager.GetState() == ConnectionState.Open)
                {
                    ReturnStatus("Connect success");
                }
                else
                {
                    ReturnStatus("Connect fail");
                    dbManager.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                ReturnStatus("Connect fail", ex.Message);
                dbManager.CloseConnection();
            }
        }

        private void bt_import_Click(object sender, EventArgs e)
        {
            ClearFolder(dataFolderPath);
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
                                ReturnStatus("Import files fail", ex.Message);
                                return;
                            }
                        }
                        else
                        {
                            tb_stt.Text = "Chỉ được phép import các file có đuôi là .sql";
                        }
                    }

                    ReturnStatus("Import fils success");
                }
            }
        }

        private void bt_submit_Click(object sender, EventArgs e)
        {
            string[] sqlFiles = Directory.GetFiles(dataFolderPath, "*.sql");

            if (sqlFiles.Length == 0)
            {
                tb_stt.Text = "Không có tệp .sql để thực thi";
                return;
            }

            foreach (string file in sqlFiles)
            {
                RunCommandScript(file);
                //RunSqlPlusScript(file, connection);  // anh Hưng không dùng được sqlplus
            }
        }

        private void RunCommandScript(string file)
        {
            string fileName = Path.GetFileName(file);
            try
            {
                dbManager.OpenConnection();
                if (dbManager.GetState() == ConnectionState.Open)
                {
                    string sqlScript = System.IO.File.ReadAllText(file);
                    string[] sqlStatements = sqlScript.Split(';');

                    foreach (string sqlStatement in sqlStatements)
                    {
                        if (!string.IsNullOrWhiteSpace(sqlStatement))
                        {
                            using (OracleCommand cmd = new OracleCommand(sqlStatement, dbManager.GetConnection()))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    ReturnStatus("File: " + fileName + " excute success");
                    dbManager.CloseConnection();
                }
                else
                {
                    dbManager.CloseConnection();
                }
            }
            catch (OracleException ex)
            {
                string errorMessage = $"Oracle error occurred: {ex.Message}\nStackTrace: {ex.StackTrace}";
                ReturnStatus("File: " + fileName + " execute failed", errorMessage);
                dbManager.CloseConnection();
            }
            catch (Exception ex)
            {
                ReturnStatus("File: " + fileName + " execute failed", ex.Message);
                // DS lỗi
                //if (ex.Message == "ORA-65021: illegal use of SHARING clause")
                //{
                //    tb_stt.Text = "ORA-65021: Không cho phép sử dụng mệnh đề SHARING";
                //}    
                //else if (ex.Message == "ORA-00933: SQL command not properly ended")
                //{
                //    tb_stt.Text = "ORA-00933: Query bị lỗi, cú pháp kết thúc sai";
                //}    
                //else if (ex.Message == "ORA-00922: missing or invalid option")
                //{
                //    tb_stt.Text = "ORA-00922: Cú pháp thiếu hoặc sai";
                //}    
                dbManager.CloseConnection();
            }
        }

        private void RunSqlPlusScript(string scriptFilePath, OracleConnection connectionString)
        {
            Console.WriteLine("connectionString", connectionString);
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
                           
                    }

                    if (!string.IsNullOrEmpty(error))
                    {
                        ReturnStatus("Run script on sqlplus fail");
                    }
                }
            }
            catch (Exception ex)
            {
                ReturnStatus("Run script on sqlplus fail");
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
                Process.Start(logFullPath);
                tb_stt.Text = "Đang mở file ...";
                lb_Data.Text = logFullPath;
            }
            catch (Exception ex)
            {
                tb_stt.Text = "Lỗi khi mở file log: " + ex.Message;
            }
        }

        private void bt_cof_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(logFullPath);
                tb_stt.Text = "Đang mở file ...";
                lb_Data.Text = logFullPath;
            }
            catch (Exception ex)
            {
                tb_stt.Text = "Lỗi khi mở file log: " + ex.Message;
            }
        }
    }
}





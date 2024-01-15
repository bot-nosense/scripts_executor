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
using ExcuteScripts.DataAccess;

namespace ExcuteScripts
{
    public partial class Main : Form
    {
        #region Props
        private OracleDBManager dbManager;
        private OracleConnection connection;
        private OracleTransaction transaction;
        #endregion

        #region Events
        public Main()
        {
            InitializeComponent();
            Utils.ClearFolder(Constants.dataFolderPath);
            Dictionary<string, string> dbConfig = ConstantsReader.ReadConstantsFromFile(Path.GetFullPath(Constants.DBCONFIGPATH));

            string host = dbConfig["HOST"];
            string port = dbConfig["PORT"];
            string sid = dbConfig["SID"];
            string serviceName = dbConfig["SERVICE_NAME"];
            string userId = dbConfig["USER_ID"];
            string password = dbConfig["PASSWORD"];
            string isSid = dbConfig["IS_SID"];
            bool sysDba = true;

            dbManager = new OracleDBManager();
            dbManager.SetConnectionParameters(host, port, sid, serviceName, userId, password, isSid, sysDba);
            connection = dbManager.GetConnection();
            Directory.CreateDirectory(Constants.dataFolderPath);

            Utils.WriteToLogFile("--------", "");
            Utils.WriteToLogFile(" \t \t \t NEW SEESION", "");
            Utils.WriteToLogFile("Login Param: " + dbConfig["HOST"] + ", " + dbConfig["PORT"] + ", " + dbConfig["SERVICE_NAME"] + ", " + dbConfig["USER_ID"] + ", " + dbConfig["PASSWORD"], "");
        }

        private void bt_conn_Click(object sender, EventArgs e)
        {
            try
            {
                dbManager.OpenConnection();
                if (dbManager.GetState() == ConnectionState.Open)
                {
                    Utils.ReturnStatus("Connect success", "", tb_stt);
                }
                else
                {
                    Utils.ReturnStatus("Connect fail", "", tb_stt);
                    dbManager.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                Utils.ReturnStatus("Connect fail", ex.Message, tb_stt);
                dbManager.CloseConnection();
            }
        }

        private void bt_import_Click(object sender, EventArgs e)
        {
            Utils.ClearFolder(Constants.dataFolderPath);
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    lb_Data.Text = Constants.dataFolderPath;
                    tb_view_ip.Clear();

                    foreach (string file in openFileDialog.FileNames)
                    {
                        if (Path.GetExtension(file).Equals(".sql", StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                string destinationFile = Path.Combine(Constants.dataFolderPath, Path.GetFileName(file));
                                File.Copy(file, destinationFile, true);
                                tb_view_ip.AppendText(Path.GetFileName(file) + Environment.NewLine);
                            }
                            catch (Exception ex)
                            {
                                Utils.ReturnStatus("Import files fail", ex.Message, tb_stt);
                                return;
                            }
                        }
                        else
                        {
                            tb_stt.Text = "Chỉ được phép import các file có đuôi là .sql";
                        }
                    }

                    Utils.ReturnStatus("Import fils success", "", tb_stt);
                }
            }
        }

        private void bt_submit_Click(object sender, EventArgs e)
        {
            string[] sqlFiles = Directory.GetFiles(Constants.dataFolderPath, "*.sql");
            connection = dbManager.GetConnection();
            dbManager.OpenConnection();
            transaction = connection.BeginTransaction();

            if (sqlFiles.Length == 0)
            {
                tb_stt.Text = "Không có tệp .sql để thực thi";
                return;
            }

            //if (dbManager.GetState() == ConnectionState.Open)
            //{
            //    foreach (string file in sqlFiles)
            //    {
            //        ExcuteOracleCommand(file, transaction);
            //    }
            //}

            //dbManager.CloseConnection();

            try
            {
                using (transaction = connection.BeginTransaction())
                {
                    if (dbManager.GetState() == ConnectionState.Open)
                    {
                        foreach (string file in sqlFiles)
                        {
                            string fileName = Path.GetFileName(file);
                            string sqlScript = File.ReadAllText(file);

                            try
                            {
                                using (OracleCommand command = connection.CreateCommand())
                                {
                                    command.CommandType = CommandType.Text;
                                    command.Transaction = transaction;

                                    string[] commands = sqlScript.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries); // thêm điều kiện tách scripts

                                    foreach (string commandText in commands)
                                    {
                                        command.CommandText = commandText; 

                                        try
                                        {
                                            command.ExecuteNonQuery();
                                        }
                                        catch (Exception ex)
                                        {
                                            Utils.ReturnStatus("File: " + fileName + " execute fail", ex.Message, tb_stt);
                                            transaction.Rollback();
                                            return;
                                        }
                                    }

                                    Utils.ReturnStatus("Transaction committed. File: " + fileName + " executed successfully", "", tb_stt);
                                }
                            }
                            catch (OracleException ex)
                            {
                                string errorMessage = $"Oracle error occurred: {ex.Message}\nStackTrace: {ex.StackTrace}";
                                Utils.ReturnStatus("File: " + fileName + " execute failed", errorMessage, tb_stt);
                            }
                            catch (Exception ex)
                            {
                                Utils.ReturnStatus("File: " + fileName + " execute failed", ex.Message, tb_stt);
                            }
                        }

                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.ReturnStatus("Error occurred", ex.Message, tb_stt);
            }
            finally
            {
                transaction?.Dispose();
                dbManager.CloseConnection();
            }
        }

        private void bt_clear_Click(object sender, EventArgs e)
        {
            try
            {
                Utils.ClearFolder(Constants.dataFolderPath);

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
                Process.Start(Constants.logFullPath);
                tb_stt.Text = "Đang mở file ...";
                lb_Data.Text = Constants.logFullPath;
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
                Process.Start(Constants.dbConfigFullPath);
                tb_stt.Text = "Đang mở file ...";
                lb_Data.Text = Constants.dbConfigFullPath;
            }
            catch (Exception ex)
            {
                tb_stt.Text = "Lỗi khi mở file config: " + ex.Message;
            }
        }
        #endregion

        #region Methods
        private void ExcuteOracleCommand(string file, OracleTransaction transaction)
        {
            string fileName = Path.GetFileName(file);
            string sqlScript = File.ReadAllText(file);
            try
            {
                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    //command.Transaction = transaction;
                    //string[] commands = sqlScript.Split(new[] { ";\r\n" }, StringSplitOptions.RemoveEmptyEntries); // đặt thêm điều kiện chia nhỏ scripts
                    string[] commands = sqlScript.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string commandText in commands)
                    {
                        command.CommandText = commandText;
                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            Utils.ReturnStatus("File: " + fileName + " execute fail", ex.Message, tb_stt);
                            transaction.Rollback(); // cân nhắc đặt rollback ở catch ngoài ?
                            return;
                        }
                    }
                    transaction.Commit();
                    Utils.ReturnStatus("Transaction committed. File: " + fileName + " executed successfully", "", tb_stt);
                }
            }
            catch (OracleException ex)
            {
                string errorMessage = $"Oracle error occurred: {ex.Message}\nStackTrace: {ex.StackTrace}";
                Utils.ReturnStatus("File: " + fileName + " execute failed", errorMessage, tb_stt);
            }
            catch (Exception ex)
            {
                Utils.ReturnStatus("File: " + fileName + " execute failed", ex.Message, tb_stt);
            }
            finally
            {
                transaction?.Dispose();
            }
        }

        private void ExcuteCommandScript(string file)
        {
            //string fileName = Path.GetFileName(file);
            //try
            //{
            //    dbManager.OpenConnection();
            //    if (dbManager.GetState() == ConnectionState.Open)
            //    {
            //        string sqlScript = System.IO.File.ReadAllText(file);
            //        string[] sqlStatements = sqlScript.Split(';');

            //        foreach (string sqlStatement in sqlStatements)
            //        {
            //            if (!string.IsNullOrWhiteSpace(sqlStatement))
            //            {
            //                using (OracleCommand cmd = new OracleCommand(sqlStatement, dbManager.GetConnection()))
            //                {
            //                    cmd.ExecuteNonQuery();
            //                }
            //            }
            //        }

            //        Utils.ReturnStatus("File: " + fileName + " excute success", "", tb_stt);
            //        dbManager.CloseConnection();
            //    }
            //    else
            //    {
            //        dbManager.CloseConnection();
            //    }
            //}
            //catch (OracleException ex)
            //{
            //    string errorMessage = $"Oracle error occurred: {ex.Message}\nStackTrace: {ex.StackTrace}";
            //    Utils.ReturnStatus("File: " + fileName + " execute failed", errorMessage, tb_stt);
            //    dbManager.CloseConnection();
            //}
            //catch (Exception ex)
            //{
            //    Utils.ReturnStatus("File: " + fileName + " execute failed", ex.Message, tb_stt);
            //    dbManager.CloseConnection();
            //}
        }

        private void ExcuteSqlPlusScript(string scriptFilePath, OracleConnection connectionString)
        {
            //Console.WriteLine("connectionString", connectionString);
            //try
            //{
            //    ProcessStartInfo startInfo = new ProcessStartInfo();
            //    startInfo.FileName = "sqlplus";
            //    startInfo.Arguments = connectionString + " @" + scriptFilePath;
            //    startInfo.UseShellExecute = false;
            //    startInfo.RedirectStandardOutput = true;
            //    startInfo.RedirectStandardError = true;
            //    startInfo.CreateNoWindow = true;

            //    using (Process process = new Process())
            //    {
            //        process.StartInfo = startInfo;
            //        process.Start();

            //        string output = process.StandardOutput.ReadToEnd();
            //        string error = process.StandardError.ReadToEnd();

            //        process.WaitForExit();

            //        if (!string.IsNullOrEmpty(output))
            //        {

            //        }

            //        if (!string.IsNullOrEmpty(error))
            //        {
            //            Utils.ReturnStatus("Run script on sqlplus fail", "", tb_stt);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Utils.ReturnStatus("Run script on sqlplus fail", ex.Message, tb_stt);
            //}
        }

        #endregion


    }
}
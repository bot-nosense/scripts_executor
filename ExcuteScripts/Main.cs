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
using System.Text.RegularExpressions;

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
            Directory.CreateDirectory(Constants.DATAFOLDERPATH);
            Utils.ClearFolder(Constants.DATAFOLDERPATH);
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
                    return;
                }
            }
            catch (Exception ex)
            {
                Utils.ReturnStatus("Connect fail", ex.Message, tb_stt);
            }
            finally
            {
                dbManager.CloseConnection();
            }
        }

        private void bt_import_Click(object sender, EventArgs e)
        {
            Utils.ClearFolder(Constants.DATAFOLDERPATH);
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    lb_Data.Text = Constants.DATAFOLDERPATH;
                    tb_view_ip.Clear();

                    foreach (string file in openFileDialog.FileNames)
                    {
                        if (Path.GetExtension(file).Equals(".sql", StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                string destinationFile = Path.Combine(Constants.DATAFOLDERPATH, Path.GetFileName(file));
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
            string[] sqlFiles = Directory.GetFiles(Constants.DATAFOLDERPATH, "*.sql");
            List<string> commands = new List<string>();
            connection = dbManager.GetConnection();
            dbManager.OpenConnection();

            if (sqlFiles.Length == 0)
            {
                tb_stt.Text = "Không có tệp .sql để thực thi";
                return;
            }
            else
            {
                ExcuteOracleCommand(sqlFiles, transaction, commands);
            }
        }

        private void bt_clear_Click(object sender, EventArgs e)
        {
            try
            {
                Utils.ClearFolder(Constants.DATAFOLDERPATH);

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
                Process.Start(Constants.LOGFULLPATH);
                tb_stt.Text = "Đang mở file ...";
                lb_Data.Text = Constants.LOGFULLPATH;
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
                Process.Start(Constants.DBCONFIGFULLPATH);
                tb_stt.Text = "Đang mở file ...";
                lb_Data.Text = Constants.DBCONFIGFULLPATH;
            }
            catch (Exception ex)
            {
                tb_stt.Text = "Lỗi khi mở file config: " + ex.Message;
            }
        }
        #endregion

        #region Methods
        private void ExcuteOracleCommand(string[] sqlFiles, OracleTransaction transaction, List<string> commands)
        {
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
                                    commands = SplitString(sqlScript);
                                    
                                    if (commands.Count < 1)
                                    {
                                        Utils.ReturnStatus("File: " + fileName + " execute failed", " No valid data", tb_stt);
                                    }
                                    else
                                    {
                                        foreach (string commandText in commands)
                                        {
                                            command.CommandText = commandText.Trim().Replace(";", "").Replace("/", "");

                                            try
                                            {
                                                command.ExecuteNonQuery();
                                            }
                                            catch (OracleException ex)
                                            {
                                                Utils.ReturnStatus("File: " + fileName + " execute fail", ex.Message, tb_stt);
                                                transaction.Rollback();
                                                return;
                                            }
                                        }

                                        Utils.ReturnStatus("Transaction committed. File: " + fileName + " executed successfully", "", tb_stt);
                                    } 
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

        private string BuildRegexPattern(string[] scriptKeys)
        {
            StringBuilder patternBuilder = new StringBuilder("(");

            foreach (string key in scriptKeys)
            {
                patternBuilder.Append($"{key}[^/;]+[/;]|");
            }

            patternBuilder.Remove(patternBuilder.Length - 1, 1); 
            patternBuilder.Append(")");

            return patternBuilder.ToString();
        }   
        
        private List<string> SplitString(string sqlScript)
        {
            List<string> result = new List<string>();

            //string[] commands = sqlScript.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries); // thêm điều kiện tách scripts
            // tách từng đoạn, sau đó tìm cách loại bỏ cmt trong script được tách
            //string[] commands = sqlScript.Split(new[] { ";", "/" }, StringSplitOptions.RemoveEmptyEntries);
            //foreach (var item in commands)
            //{
            //    string updateItem = item.Trim() + ";";

            //    if (Constants.SCRIPTKEYS.Any(key => updateItem.TrimStart().StartsWith(key, StringComparison.OrdinalIgnoreCase)))
            //    {

            //        string[] parts = updateItem.Split(new string[] { matchingKeywords }, StringSplitOptions.RemoveEmptyEntries);

            //    }
            //    if (!string.IsNullOrWhiteSpace(updateItem))
            //    {
            //        result.Add(updateItem);
            //    }

            //}

            string pattern = BuildRegexPattern(Constants.SCRIPTKEYS);
            MatchCollection matches = Regex.Matches(sqlScript, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            string[] scriptParts = new string[matches.Count];
            for (int i = 0; i < matches.Count; i++)
            {
                scriptParts[i] = matches[i].Value.Trim();
                result.Add(scriptParts[i]);
            }

            return result;
        }
        #endregion


    }
}
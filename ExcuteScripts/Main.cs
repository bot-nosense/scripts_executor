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
        //private OracleTransaction transaction;
        private bool sysDba;
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
            string isClient = dbConfig["IS_CLIENT"];
            string serverName = dbConfig["SERVER"];

            if (int.Parse(isClient) == 0)
            {
                sysDba = true;
            }
            else
            {
                sysDba = false;
            }

            dbManager = new OracleDBManager();
            dbManager.SetConnectionParameters(host, port, sid, serviceName, userId, password, isSid, isClient, serverName, sysDba);

            Utils.WriteToLogFile("--------", "");
            Utils.WriteToLogFile(" \t \t \t NEW SEESION", "");
            Utils.WriteToLogFile("Login Param: " + dbConfig["HOST"] + ", " + dbConfig["PORT"] + ", " + dbConfig["SERVICE_NAME"] + ", " + dbConfig["USER_ID"] + ", " + dbConfig["PASSWORD"], "");
        }

        private void bt_conn_Click(object sender, EventArgs e)
        {
            if (dbManager == null)
            {
                Utils.ReturnStatus("OracleDBManager is null", "", tb_stt);
                return;
            }
            else
            {
                connection = dbManager.GetConnection();
                if (connection != null && connection.State != ConnectionState.Open)
                {
                    try
                    {
                        connection.Open();

                        if (connection.State == ConnectionState.Open)
                        {
                            Utils.ReturnStatus("Connect success", "", tb_stt);
                        }
                        else
                        {
                            Utils.ReturnStatus("Connect fail", " connection cannot be opened", tb_stt);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Utils.ReturnStatus("Connect fail", ex.Message, tb_stt);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                else
                {
                    Utils.ReturnStatus("Connection null", "", tb_stt);
                }
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

            if (sqlFiles.Length == 0)
            {
                tb_stt.Text = "Không có tệp .sql để thực thi";
                return;
            }
            else
            {

                ExecuteScriptFiles();
            }
        }

        private void bt_clear_Click(object sender, EventArgs e)
        {
            try
            {
                Utils.ClearFolder(Constants.DATAFOLDERPATH);
                Utils.ReturnStatus("Deleted all files in the Data folder.", "", tb_stt);
                tb_view_ip.Clear();
            }
            catch (Exception ex)
            {
                Utils.ReturnStatus("Error deleting file", ex.Message, tb_stt);
            }
        }

        private void bt_log_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Constants.LOGFULLPATH);
                Utils.ReturnStatus("Opening log file...", "", tb_stt);
                lb_Data.Text = Constants.LOGFULLPATH;
            }
            catch (Exception ex)
            {
                Utils.ReturnStatus("Error opening log file", ex.Message, tb_stt);
            }
        }

        private void bt_cof_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Constants.DBCONFIGFULLPATH);
                Utils.ReturnStatus("Opening config file...", "", tb_stt);
                lb_Data.Text = Constants.DBCONFIGFULLPATH;
            }
            catch (Exception ex)
            {
                Utils.ReturnStatus("Error opening config file", ex.Message, tb_stt);
            }
        }
        #endregion

        #region Methods
        private void ExcuteOracleCommand(string[] sqlFiles, List<string> commands) //, OracleTransaction transaction
        {
            connection = dbManager.GetConnection();
            if (connection != null && connection.State != ConnectionState.Open)
            {
                try
                {
                    connection.Open();

                    using (OracleCommand command = connection.CreateCommand())
                    {
                        using (OracleTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                        {
                            command.Transaction = transaction;

                            if (connection.State == ConnectionState.Open)
                            {
                                foreach (string file in sqlFiles)
                                {
                                    string fileName = Path.GetFileName(file);
                                    string sqlScript = File.ReadAllText(file);
                                    commands = SplitString(sqlScript);

                                    if (commands != null && commands.Count > 0)
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
                                                transaction.Rollback();
                                                Utils.ReturnStatus("File: " + fileName + " execute fail", ex.Message, tb_stt);
                                                return;
                                            }
                                        }

                                        Utils.ReturnStatus("Transaction committed. File: " + fileName + " executed successfully", "", tb_stt);
                                    }
                                }

                                transaction.Commit();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utils.ReturnStatus("Error occurred", ex.Message, tb_stt);
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                Utils.ReturnStatus("Connecting fail", "", tb_stt);
            }
        }

        private async void ExcuteSqlPlusScript(string scriptFilePath, OracleConnection connection)
        {
            try
            {
                using (Process process = new Process())
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = "sqlplus",
                        Arguments = connection.ConnectionString + " @" + scriptFilePath,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };

                    process.StartInfo = startInfo;
                    process.Start();

                    string output = await process.StandardOutput.ReadToEndAsync();
                    string error = await process.StandardError.ReadToEndAsync();

                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(output))
                    {
                        // xử lý ouput ...
                    }

                    if (!string.IsNullOrEmpty(error))
                    {
                        Utils.ReturnStatus("Run script on sqlplus fail", "", tb_stt);
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.ReturnStatus("Run script on sqlplus fail", ex.Message, tb_stt);
            }
        }

        private string BuildRegexPattern(string[] scriptKeys)
        {
            StringBuilder patternBuilder = new StringBuilder("(");

            foreach (string key in scriptKeys)
            {
                // bắt đoạn proc
                if (key == "CREATE OR REPLACE")
                {
                    patternBuilder.Append($@"\b{key}\b.*?(END;\s*/)\s*|"); // bắt đoạn mở bằng CREATE OR REPLACE, kết thúc bằng END; /
                    break;
                }
                else
                {
                    patternBuilder.Append($@"\b{key}\b[^/;]+[/;]|"); // kết thúc bằng ';' hoặc '/'
                }
            }

            patternBuilder.Remove(patternBuilder.Length - 1, 1);
            patternBuilder.Append(")");

            //// Loại bỏ các đoạn bắt đầu bằng '/*' và kết thúc bằng '*/'
            //patternBuilder.Insert(0, @"(?s)"); // Cho phép '.' trong biểu thức chính quy tương ứng với ký tự newline
            //patternBuilder.Insert(0, @"(?:(?!/\*).)*"); // Không khớp với '/*' ở bất kỳ vị trí nào
            //patternBuilder.Insert(0, @"(?:(?!\*/).)*"); // Không khớp với '*/' ở bất kỳ vị trí nào
            //patternBuilder.Insert(0, @"(?:/\*(?:(?!\*/).)*\*/)?"); // Khớp với '/* ... */' ở bất kỳ vị trí nào hoặc không có

            return patternBuilder.ToString();
        }

        private List<string> SplitString(string sqlScript)
        {
            List<string> result = new List<string>();
            string pattern = BuildRegexPattern(Constants.SCRIPTKEYS);  // thêm các key để bắt query 
            MatchCollection matches = Regex.Matches(sqlScript, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            string[] scriptParts = new string[matches.Count];
            for (int i = 0; i < matches.Count; i++)
            {
                scriptParts[i] = matches[i].Value.Trim();
                result.Add(scriptParts[i]);
            }

            return result;
        }

        private void ExecuteScriptFiles()
        {
            connection = dbManager.GetConnection();
            if (connection != null && connection.State != ConnectionState.Open)
            {
                try
                {
                    connection.Open();

                    string[] sqlFiles = Directory.GetFiles(Constants.DATAFOLDERPATH, "*.sql");

                    foreach (string sqlFile in sqlFiles)
                    {
                        string fileName = Path.GetFileName(sqlFile);
                        string scriptContent = File.ReadAllText(sqlFile);

                        using (OracleCommand command = new OracleCommand(scriptContent, connection))
                        {
                            command.CommandType = CommandType.Text;
                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch (OracleException ex)
                            {
                                Utils.ReturnStatus("File: " + fileName + " execute fail", ex.Message, tb_stt);
                                return;
                            }
                            Utils.ReturnStatus("File: " + fileName + " executed successfully", "", tb_stt);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utils.ReturnStatus("Error occurred", ex.Message, tb_stt);
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                Utils.ReturnStatus("Connecting fail", "", tb_stt);
            }
        }    

        #endregion

    }
}
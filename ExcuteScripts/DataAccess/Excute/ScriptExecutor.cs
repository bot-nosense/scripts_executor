using ExcuteScripts.DataAccess.OracleDatabase;

using Oracle.ManagedDataAccess.Client;

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcuteScripts.DataAccess.Excute
{
    public class ScriptExecutor
    {
        private static OracleDBManager dbManager;
        private static OracleConnection connection;
        private static OracleTransaction transaction;

        public static void ExcuteOracleScript(string file)
        {
            connection = dbManager.GetConnection();
            string fileName = Path.GetFileName(file);
            string sqlScript = System.IO.File.ReadAllText(file);
            try
            {
                dbManager.OpenConnection();
                transaction = connection.BeginTransaction();

                if (dbManager.GetState() == ConnectionState.Open)
                {
                    using (OracleCommand command = connection.CreateCommand())
                    {
                        command.CommandType = System.Data.CommandType.Text;
                        command.Transaction = transaction;
                        string[] commands = sqlScript.Split(new[] { ";\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string commandText in commands)
                        {
                            command.CommandText = commandText;
                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                Utils.ReturnStatus("File: " + fileName + " execute fail");
                                return;
                            }
                        }
                        transaction.Commit();
                        Utils.ReturnStatus("All commands executed successfully. Transaction committed. File: " + fileName + " execute success");
                    }
                }
                dbManager.CloseConnection();
            }
            catch (OracleException ex)
            {
                string errorMessage = $"Oracle error occurred: {ex.Message}\nStackTrace: {ex.StackTrace}";
                Utils.ReturnStatus("File: " + fileName + " execute failed", errorMessage);
            }
            catch (Exception ex)
            {
                Utils.ReturnStatus("File: " + fileName + " execute failed", ex.Message);
            }
            finally
            {
                transaction?.Dispose();
                dbManager.CloseConnection();
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

                    Utils.ReturnStatus("File: " + fileName + " excute success");
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
                Utils.ReturnStatus("File: " + fileName + " execute failed", errorMessage);
                dbManager.CloseConnection();
            }
            catch (Exception ex)
            {
                Utils.ReturnStatus("File: " + fileName + " execute failed", ex.Message);
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
                        Utils.ReturnStatus("Run script on sqlplus fail");
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.ReturnStatus("Run script on sqlplus fail");
            }
        }


    }
}

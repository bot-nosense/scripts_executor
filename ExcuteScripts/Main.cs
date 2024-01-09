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
using ExcuteScripts.DataAccess.Excute;

namespace ExcuteScripts
{
    public partial class Main : Form
    {
        private OracleDBManager dbManager;

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
            
            dbManager.SetConnectionParameters( host, port, sid, serviceName, userId, password, isSid, sysDba);
            Directory.CreateDirectory(Constants.dataFolderPath);

            Utils.WriteToLogFile("--------", "");
            Utils.WriteToLogFile(" \t \t \t NEW SEESION", "");
            Utils.WriteToLogFile("Login Param: "+ dbConfig["HOST"] + ", " + dbConfig["PORT"] + ", " + dbConfig["SERVICE_NAME"] + ", " + dbConfig["USER_ID"] + ", " + dbConfig["PASSWORD"], "");
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

            if (sqlFiles.Length == 0)
            {
                tb_stt.Text = "Không có tệp .sql để thực thi";
                return;
            }

            foreach (string file in sqlFiles)
            {
                //RunCommandScript(file); // xử lý từng lệnh đơn lẻ không nhất quán, custom nhiều, dễ lỗi
                //RunSqlPlusScript(file, connection);  // anh Hưng không dùng được sqlplus
                ScriptExecutor.ExcuteOracleScript(file);
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
    }
}
using ExcuteScripts.Config;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcuteScripts.DataAccess
{
    public class Utils
    {
        public Utils()
        {

        }

        public static void ClearFolder(string dataFolderPath)
        {
            string[] files = Directory.GetFiles(dataFolderPath);

            foreach (string file in files)
            {
                File.Delete(file);
            }
        }

        public static void WriteToLogFile(string info, string detail = "")
        {
            try
            {
                string logInfo = $"{DateTime.Now.ToString("yyyyMMdd-HH:mm")}, {info}, {detail}";


                using (StreamWriter sw = File.AppendText(Constants.LOGFULLPATH))
                {
                    sw.WriteLine(logInfo);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing log: " + ex.Message);
            }
        }

        public static void ReturnStatus(string info, string detail = "", TextBox tb_stt = null)
        {
            tb_stt.Text = info;
            WriteToLogFile(info, detail);
        }
    }
}

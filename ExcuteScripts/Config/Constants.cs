using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcuteScripts.Config
{
    public static class Constants
    {
        public static readonly string LOGFILEPATH = "../../Logs/log.txt";
        public static readonly string DBCONFIGPATH = "../../Config/dbConfig.txt";
        public static readonly string SYSCONFIGPATH = "../../Config/sysConfig.txt";
        public static readonly string dataFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datas");
        public static readonly string logFullPath = Path.GetFullPath(LOGFILEPATH);
        public static readonly string dbConfigFullPath = Path.GetFullPath(DBCONFIGPATH);
    }
}

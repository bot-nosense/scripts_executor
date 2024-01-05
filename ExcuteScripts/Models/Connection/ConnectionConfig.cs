using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcuteScripts.Models.Connection
{
    class ConnectionConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string SID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsUsingSSL { get; set; } // using sysDBA
    }
}

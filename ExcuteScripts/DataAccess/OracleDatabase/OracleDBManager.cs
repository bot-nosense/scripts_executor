using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;


namespace ExcuteScripts.DataAccess.OracleDatabase
{
    class OracleDBManager
    {
        private OracleConnection connection;
        private OracleConnectionStringBuilder connectionStringBuilder;

        public OracleDBManager()
        {
            connectionStringBuilder = new OracleConnectionStringBuilder();
        }

        public void SetConnectionParameters(string host, string port, string sid, string service_name, string user, string password, string is_sid)
        {
            if (int.Parse(is_sid) == 0)
            {
                connectionStringBuilder.DataSource = $"(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port})))(CONNECT_DATA=(SERVICE_NAME={service_name})))";
            }
            else
            {
                connectionStringBuilder.DataSource = $"(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port})))(CONNECT_DATA=(SID={sid})))";
            }
            connectionStringBuilder.UserID = user;
            connectionStringBuilder.Password = password;
        }

        public void SetConnectionParameters(string host, string port, string sid, string service_name, string user, string password, string is_sid, bool sysDba = false)
        {
            Console.WriteLine(host + ", " + port + ", " + sid + ", ", user + ", " + password);
            if (int.Parse(is_sid) == 0)
            {
                connectionStringBuilder.DataSource = $"(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port})))(CONNECT_DATA=(SERVICE_NAME={service_name})))";
            }
            else
            {
                connectionStringBuilder.DataSource = $"(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port})))(CONNECT_DATA=(SID={sid})))";
            }
            connectionStringBuilder.UserID = user;
            connectionStringBuilder.Password = password;

            if (sysDba)
            {
                connectionStringBuilder.DBAPrivilege = "SYSDBA";
            }
        }

        public void OpenConnection()
        {
            try
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    Console.WriteLine("Connection is already open.");
                }

                connection = new OracleConnection(connectionStringBuilder.ConnectionString);
                connection.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to Oracle: " + ex.Message);
            }
        }

        public void CloseConnection()
        {
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public OracleConnection GetConnection()
        {
            return connection;
        }
        public ConnectionState GetState()
        {
            return GetConnection().State;
        }
    }
}
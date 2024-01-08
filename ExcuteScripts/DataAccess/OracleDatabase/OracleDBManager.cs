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

        public void SetConnectionParameters(string host, int port, string sid, string user, string password)
        {
            connectionStringBuilder.DataSource = $"(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port})))(CONNECT_DATA=(SID={sid})))";
            connectionStringBuilder.UserID = user;
            connectionStringBuilder.Password = password;
        }

        public void SetConnectionParameters(string host, string port, string sid, string userId, string password, bool sysDba = false)
        {
            connectionStringBuilder.DataSource = $"(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port})))(CONNECT_DATA=(SID={sid})))";
            connectionStringBuilder.UserID = userId;
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
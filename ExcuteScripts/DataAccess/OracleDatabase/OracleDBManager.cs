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
        private OracleConnection connection ;
        private OracleConnectionStringBuilder connectionStringBuilder;

        public OracleDBManager()
        {
            connectionStringBuilder = new OracleConnectionStringBuilder();
        }

        public void SetConnectionParameters(string host, string port, string sid, string service_name, string user, string password, string is_sid)
        {
            if (int.Parse(is_sid) == 0)
            {
                connectionStringBuilder.DataSource = $"(DESCRIPTION=(ADDRESS=(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port})))(CONNECT_DATA=(SERVICE_NAME={service_name})))";
            }
            else
            {
                connectionStringBuilder.DataSource = $"(DESCRIPTION=(ADDRESS=(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port})))(CONNECT_DATA=(SID={sid})))";
            }
            connectionStringBuilder.UserID = user;
            connectionStringBuilder.Password = password;
            connection = new OracleConnection(connectionStringBuilder.ConnectionString);
        }

        public void SetConnectionParameters(string host, string port, string sid, string serviceName, string user, string password, string isSid, string isClient, string serverName, bool sysDba = false)
        {
            if (int.Parse(isClient) == 1)
            {
                if (int.Parse(isSid) == 0)
                {
                    connectionStringBuilder.DataSource = $"(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port}))(CONNECT_DATA=(SERVICE_NAME={serviceName}))(SERVER={serverName}))";
                }
                else
                {
                    connectionStringBuilder.DataSource = $"(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port}))(CONNECT_DATA=(SID={sid}))(SERVER={serverName}))";
                }
            }
            else
            {
                if (int.Parse(isSid) == 0)
                {
                    connectionStringBuilder.DataSource = $"(DESCRIPTION=(ADDRESS=(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port})))(CONNECT_DATA=(SERVICE_NAME={serviceName})))";
                }
                else
                {
                    connectionStringBuilder.DataSource = $"(DESCRIPTION=(ADDRESS=(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port})))(CONNECT_DATA=(SID={sid})))";
                }
            }

            if (sysDba)
            {
                connectionStringBuilder.DBAPrivilege = "SYSDBA";
            }

            connectionStringBuilder.UserID = user;
            connectionStringBuilder.Password = password;
            connection = new OracleConnection(connectionStringBuilder.ConnectionString);
        }

        public void OpenConnection()
        {
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                Utils.WriteToLogFile("Error connecting to Oracle: ", ex.Message);
            }
        }

        public void CloseConnection()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
                connection.Dispose();
                Utils.WriteToLogFile("Connection closed successfully.");
            }
        }

        public OracleConnection GetConnection()
        {
            return connection; // new OracleConnection(connectionStringBuilder.ConnectionString);
        }

        public ConnectionState GetState()
        {
            return GetConnection().State;
        }
    }
}
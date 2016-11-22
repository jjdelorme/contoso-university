using System;
using System.Configuration;
using System.Web.Configuration;
using Microsoft.Extensions.Configuration;
using Steeltoe.Extensions.Configuration;

namespace ContosoUniversity
{
    public class CloudSqlConnection
    {
        private static string actualConnection = string.Empty;

        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(actualConnection))
                {
                    actualConnection = LoadConnectionString();
                }
                return actualConnection;
            }
        }

        private static string LoadConnectionString()
        {
            var connStr = LoadCloudConnectionString();
            if (string.IsNullOrEmpty(connStr))
            {
                connStr = LoadWebConfigConnectionString();
                if (string.IsNullOrEmpty(connStr))
                {
                    throw new ConfigurationErrorsException(
                        "No Contoso SQL Server connection string was found. Did you forget to bind the " +
                        "user provided service named 'schoolcontext' to this app?");
                }
            }
            return connStr;
        }

        private static string LoadCloudConnectionString()
        {
            var config = new ConfigurationBuilder().AddCloudFoundry().Build();
           if (config["vcap:services:user-provided:0:name"] == "schoolcontext")
            {
                return config["vcap:services:user-provided:0:credentials:connectionString"];
            }
            return "";
        }

        private static string LoadWebConfigConnectionString()
        {
            return WebConfigurationManager.ConnectionStrings["SchoolContext"]?.ConnectionString;
        }
    }
}
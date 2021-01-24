using System;
using System.Collections.Generic;

namespace DataQuery.Net.Tests
{
    public class DataQueryConfigFixture : IDisposable
    {
        private readonly string _cnx = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=dataquery-test;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|\ngpadataquery-test.mdf";

        public DataQueryConfigFixture()
        {
            Config = new DataQueryConfig(_cnx) { };

            Config.Tables["User"] = new Table()
            {
                Name = "User",
                Alias = "U",
                Props = new List<DatabaseProp>
        {
          new DatabaseProp()
          {
            Alias = "UserId",
            Column = "U.Id",
            Description = "User's id",
            Label="Userid",
            Aff = true,
            SqlJoin = new Dictionary<string, string>
            {
              {"User_Stat", "UserId" }
            }
          },
          new DatabaseProp()
          {
            Alias = "Name",
            Column = "U.Name",
            Description = "User's name",
            Label="Username",
            Aff = true
          },
          new DatabaseProp()
          {
            Alias = "Email",
            Column = "U.Email",
            Description = "Email",
            Label="Email",
            Aff = true
          }
        }
            };

            Config.Tables["User_Stat"] = new Table()
            {
                Name = "User_Stat",
                Alias = "US",
                Props = new List<DatabaseProp>
        {
          new DatabaseProp()
          {
            Alias = "UserIdStat",
            Column = "US.UserId",
            Aff = true,
            SqlJoin = new Dictionary<string, string>
            {
              {"User", "UserId" }
            }
          },
          new DatabaseProp()
          {
            Alias = "Date",
            Column = "US.Date",
            Description = "Date",
            UsedToFilterDate = true,
            SqlType = System.Data.SqlDbType.Date,
            Aff = true
          },
          new DatabaseProp()
          {
            Alias = "NbConnexion",
            Column = "SUM(U.NbConnexion)",
            Description = "NbConnexion",
            Label="NbConnexion",
            IsMetric = true,
            Aff = true
          }
        }
            };

        }

        public void Dispose()
        {
            this.Config = null;
        }

        private void ProvisionDatabase()
        {

        }

        public DataQueryConfig Config { get; private set; }
    }
}

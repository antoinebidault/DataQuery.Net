using System;
using System.Collections.Generic;

namespace DataQuery.Net.Tests
{
    public class DataQueryConfigFixture : IDisposable
    {
        public DataQueryConfigFixture()
        {
            Config = new DataQueryCollections() { };

            Config.Tables["User"] = new Table()
            {
                Name = "User",
                Alias = "U",
                Props = new List<Column>
        {
          new Column()
          {
            Alias = "UserId",
            ColumnName = "U.Id",
            Description = "User's id",
            Label="Userid",
            Displayed = true,
            SqlJoins = new Dictionary<string, string>
            {
              {"User_Stat", "UserId" }
            }
          },
          new Column()
          {
            Alias = "Name",
            ColumnName = "U.Name",
            Description = "User's name",
            Label="Username",
            Displayed = true
          },
          new Column()
          {
            Alias = "Email",
            ColumnName = "U.Email",
            Description = "Email",
            Label="Email",
            Displayed = true
          }
        }
            };

            Config.Tables["User_Stat"] = new Table()
            {
                Name = "User_Stat",
                Alias = "US",
                Props = new List<Column>
        {
          new Column()
          {
            Alias = "UserIdStat",
            ColumnName = "US.UserId",
            Displayed = true,
            SqlJoins = new Dictionary<string, string>
            {
              {"User", "UserId" }
            }
          },
          new Column()
          {
            Alias = "Date",
            ColumnName = "US.Date",
            Description = "Date",
            UsedToFilterDate = true,
            SqlType = System.Data.SqlDbType.Date,
            Displayed = true
          },
          new Column()
          {
            Alias = "NbConnexion",
            ColumnName = "SUM(U.NbConnexion)",
            Description = "NbConnexion",
            Label="NbConnexion",
            IsMetric = true,
            Displayed = true
          }
        }
            };

        }

        public void Dispose()
        {
            this.Config = null;
        }

        public DataQueryCollections Config { get; private set; }
    }
}

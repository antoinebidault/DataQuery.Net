using System;
using System.Collections.Generic;

namespace DataQuery.Net.Tests
{
    public class DataQueryConfigFixture : IDisposable
    {
        public DataQueryConfigFixture()
        {
            Config = new DataQuerySchema() { };

            Config.AddTable(new Table("User")
            {
                Alias = "U",
               Root = true,
                Columns = new List<Column>
                {
                    new Column()
                    {
                    Alias = "UserId",
                    ColumnName = "U.Id",
                    Description = "User's id",
                    DisplayName="Userid",
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
                    DisplayName="Username",
                    Displayed = true
                    },
                    new Column()
                    {
                    Alias = "Email",
                    ColumnName = "U.Email",
                    Description = "Email",
                    DisplayName="Email",
                    Displayed = true
                    }
                }
            });

            Config.AddTable(new Table("User_Stat")
            {
                Alias = "US",
                Columns = new List<Column>
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
                        DisplayName="NbConnexion",
                        IsMetric = true,
                        Displayed = true
                    }
                }
            });

        }

        public void Dispose()
        {
            this.Config = null;
        }

        public DataQuerySchema Config { get; set; }
    }
}

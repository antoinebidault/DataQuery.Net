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
                Columns = new List<Dimension>
                {
                    new Dimension()
                    {
                    Name = "UserId",
                    SqlName = "U.Id",
                    Description = "User's id",
                    DisplayName="Userid",
                    Displayed = true,
                    SqlJoins = new Dictionary<string, string>
                    {
                        {"User_Stat", "UserId" }
                    }
                    },
                    new Dimension()
                    {
                    Name = "Name",
                    SqlName = "U.Name",
                    Description = "User's name",
                    DisplayName="Username",
                    Displayed = true
                    },
                    new Dimension()
                    {
                    Name = "Email",
                    SqlName = "U.Email",
                    Description = "Email",
                    DisplayName="Email",
                    Displayed = true
                    }
                }
            });

            Config.AddTable(new Table("User_Stat")
            {
                Alias = "US",
                Columns = new List<Dimension>
                {
                    new Dimension()
                    {
                         Name = "UserIdStat",
                        SqlName = "US.UserId",
                        Displayed = true,
                        SqlJoins = new Dictionary<string, string>
                        {
                            {"User", "UserId" }
                        }
                    },
                    new Dimension()
                    {
                        Name = "Date",
                        SqlName = "US.Date",
                        Description = "Date",
                        UsedToFilterDate = true,
                        SqlType = System.Data.SqlDbType.Date,
                        Displayed = true
                    },
                    new Dimension()
                    {
                        Name = "NbConnexion",
                        SqlName = "SUM(U.NbConnexion)",
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

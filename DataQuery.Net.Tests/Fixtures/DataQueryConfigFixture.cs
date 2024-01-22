using System;
using System.Collections.Generic;

namespace DataQuery.Net.Tests
{
    public class DataQueryConfigFixture : IDisposable
    {
        public DataQueryConfigFixture()
        {
            Config = new DataQuerySchema() { };

            var tableUser = Config.AddTable(new Table("User")
            {
                Alias = "U",
                DisplayName = "Users",
                Root = true
            });

            tableUser.AddDimension(
                new Dimension()
                {
                    Name = "UserId",
                    SqlName = "U.Id",
                    Description = "User's id",
                    DisplayName = "Userid",
                    Displayed = true,
                    SqlJoins = new Dictionary<string, string>
                        {
                            {"US", "UserId" }
                        }
                });

            tableUser.AddDimension(
                new Dimension()
                {
                    Name = "Name",
                    SqlName = "U.Name",
                    Description = "User's name",
                    DisplayName = "Username",
                    Displayed = true
                });

            tableUser.AddDimension(
                    new Dimension()
                    {
                        Name = "Email",
                        SqlName = "U.Email",
                        Description = "Email",
                        DisplayName = "Email",
                        Displayed = true
                    });

            var table = Config.AddTable(new Table("User_Stat")
            {
                Alias = "US"
            });

            table.AddDimension(new Dimension()
            {
                Name = "UserIdStat",
                SqlName = "US.UserId",
                Displayed = true,
                SqlJoins = new Dictionary<string, string>
                        {
                            {"U", "UserId" }
                        }
            });


            table.AddDimension(new Dimension()
            {
                Name = "Date",
                SqlName = "US.Date",
                Description = "Date",
                UsedToFilterDate = true,
                SqlType = System.Data.SqlDbType.Date,
                Displayed = true
            });

            table.AddMetric(new Metric()
            {
                Name = "NbConnexion",
                SqlName = "SUM(U.NbConnexion)",
                Description = "NbConnexion",
                DisplayName = "NbConnexion",
            });

        }

        public void Dispose()
        {
            this.Config = null;
        }

        public DataQuerySchema Config { get; set; }
    }
}

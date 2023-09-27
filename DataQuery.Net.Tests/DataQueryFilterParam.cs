using System;
using Xunit;

namespace DataQuery.Net.Tests
{
    public class DataQueryFilterParam_Tests : IClassFixture<DataQueryConfigFixture>
    {
        DataQueryConfigFixture fixture;

        public DataQueryFilterParam_Tests(DataQueryConfigFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void DataQueryFilterPAramToBeWellParsed()
        {
            DataQueryFilterParams result = new DataQueryFilterParams()
            {
                Aggregate = false,
                Size = 10,
                Page = 1,
                Dimensions = "User.Name,User.Email,User_Stat.Date",
                Asc = true,
                Filters = "User.Name=~Jean Marc%",
                Metrics = "User_Stat.NbConnexion"
            };

            var cleanFilter = result.Parse(fixture.Config);

            Assert.Equal(3, cleanFilter.Dimensions.Count);
            Assert.Single(cleanFilter.Metrics);
            //  Assert.Equal(cleanFilter.DateDebut.Value.Date, DateTime.Now.AddDays(-2).Date);
        }
    }
}

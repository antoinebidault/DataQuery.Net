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
                Dimensions = new[] { "U_Name", "U_Email", "US_Date", "US_NbConnexion" },
                Asc = true,
                Filters = "U_Name=~Jean Marc%"
            };

            var cleanFilter = result.Parse(fixture.Config);

            Assert.Equal(4, cleanFilter.Dimensions.Count);
            //  Assert.Equal(cleanFilter.DateDebut.Value.Date, DateTime.Now.AddDays(-2).Date);
        }
    }
}

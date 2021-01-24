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
        Dimensions = "Name,Email,Date",
        Asc = true,
        Period = "2d",
        Filters = "Name=~Jean Marc%",
        Metrics = "NbConnexion"
      };

      var cleanFilter = new DataQueryFilter();
      result.BindTo(cleanFilter, fixture.Config);

      Assert.Equal(3, cleanFilter.Dimensions.Count);
      Assert.Single(cleanFilter.Metrics);
      Assert.Equal(cleanFilter.DateDebut.Value.Date, DateTime.Now.AddDays(-2).Date);
    }
  }
}

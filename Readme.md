
[![Build status](https://ci.appveyor.com/api/projects/status/c91s60qb1aj8bsps?svg=true)](https://ci.appveyor.com/project/antoinebidault/dataquery-net)
![Nuget](https://img.shields.io/nuget/v/DataQuery.Net)

![Logo dataquery](logo-dataquery.png)
# DataQuery.Net
The data query is an ASP.Net Core library for querying dynamically huge database using a basic querying language similar to Google analytics's API Explorer querying language (dimensions, metrics, filters...) .
This tool was particularly useful for building a custom analytic tools on a bug database using millions of lines. If you need to do lots of dynamics queries, time series, pie charts, this library can be particularly useful.

# Disclaimer
Use it at your own risk, this library is still WIP and needs some refactoring, testing improvements. 

# Prerequisite
You need an SQL database on SQL Server 2012+
ASP.Net Core 3.1
You'll need an sql database structured as a star :
[https://en.wikipedia.org/wiki/Star_schema](https://en.wikipedia.org/wiki/Star_schema)
![Database star model](database-star-model.png)

# Quickstart 

Installer le package nuget
```
package-install DataQuery.Net
```

Sample configuration in Startup.cs ConfigureServices() method :
```CSharp
services.RegisterSqlDataQueryServices(options => {
    options.ConnectionString = "{your SQL Server connection string here}";
});
services.RegisterDataQueryProvider<MyAwesomeDataQueryProvider>();
```
Implement the IDataQueryProvider interface to provide the metrics and dimensions lists to query :
```CSharp

  public class MyAwesomeDataQueryProvider : IDataQueryProvider
  {
    public DataQueryCollections Provide()
    {
      var cnx = "Ma chaine de connexion à la BDD ici";
      var config = new DataQueryCollections() { };

      config.Tables["User"] = new Table()
      {
        // The table name, it must match the key name
        Name = "User",
        // The AS alias "select from table AS {alias}"
        Alias = "U",
        // The properties you would like to query (Just the columns you need to query or used in relationships)
        Props = new List<DatabaseProp>
        {
          new Column()
          {
			// ALias : The unique name of the dim or metric
            Alias = "UserId",
			// La colonne : correspond à ce qui va être sélectionné par le requêteur. If it's a metric, you must use the proper aggregation operator : e.g. SUM(), AVG(), COUNT()...
            ColumnName = "U.Id",
			// Field description
            Description = "User's id",
            Label="Userid",
            // Le type SQL du champ sera utile pour parser les dimensions sélectionnés.
            SqlType = SqlDbType.Int,
            // Set as visible or no
            Displayed = true,
            // True if it's a metric
            IsMetric = false,
            // SQL join. The key is the "Name" of the target table, the value is the name of the prop (IN SQL, do not take the alias).
            // The sql join must be done in the both side. In this use case, in "User_Stat" => to User.id and "User" => to User_State.UserId.
            SqlJoin = new Dictionary<string, string>
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

      config.Tables["User_Stat"] = new Table()
      {
        Name = "User_Stat",
        Alias = "US",
        Props = new List<DatabaseProp>
        {
          new Column()
          {
            Alias = "UserRef",
            ColumnName = "US.UserId",
            Displayed = true,
            SqlJoin = new Dictionary<string, string>
            {
              {"User", "UserId" }
            }
          },
          new Column()
          {
            Alias = "Date",
            Column = "US.Date",  
			// This dimension will be used to filter date by default
			UsedToFilterDate = true,
            Description = "Date",
            SqlType = System.Data.SqlDbType.Date,
            Displayed = true
          },
          new Column()
          {
            Alias = "NbConnexion",
            Column = "SUM(U.NbConnexion)",
            Description = "NbConnexion",
            Label="NbConnexion",
            IsMetric = true,
            Displayed = true
          }
        }
      };


      return config;
    }
  }
```

In this sample, we have configured two tables : 
- User: Name, Email, UserId
- User_Stat: Date, NbConnexion, UserRef

**Important note**
The metric's alias must be unique, because it will be used for querying data

# Querying the data
For executing the data in a sample webapp : 
```CSharp

public TestController : Controller
{
  public IDataQuery _dataQuery;
	public TestController(IDataQuery dataQuery){
		_dataQuery = dataQuery;
	}

	[HttpGet]
	public IActionResult GetStats(DataQueryFilterParam params){
	    var results =	_dataQuery.Query(params);
		return Ok(results);
	}

}

```

Here is a sample query to get the nb connexions per date on 2 weeks for Jean-Marc :
```CSharp
/Test?dimensions=Date&metrics=NbConnexion&period=2w&asc=false&sort=Date&filters=Name%3DJean-Marc
```


# Paramètre des requêtes
Query params list :
- aggregate : wether the data are grouped or not
- size : size of the recordset (paginated results)
- page : page index
- query: full text query (using FullText index)
- queryConstraint : for limiting field used in full text query.
- start : start date for period filtering
- end :  end  date for period filtering
- period : Perdiods : 1w = 1 week, 3m = 3 months
- sort : name of the metric or dimension used to sort data
- dimensions : comma separated dimensions to select, ex: UserName, Email
- metrics : list of metrics to query, ex: NbConnexions
- filters : filter used, ex: (Name==Toto,Name!=Titi);NbViews>12;Date>01/01/2020
, = OR / ; = AND / == = equal / != = different / =~ = LIKE (with a % on the value)

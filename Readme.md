# DataQuery 
Le data query est un outil de requêtage SQL permettant de requêter des stats de manière totalement dynamique à l'aide d'un langugage de requêtage extrêmement simple pour le client et la possibilité d'exposer une API de requêtage dynamique au client avec un maximum de sécurité.

# Prérequis
Vous devez avoir une base de données SQL Server avec des volumes importants de données à analyser. 
Il faut que ces données soient structurées dans un modèle en étoile ou en flocon. En savoir plus sur ces types de modèle : (https://fr.wikipedia.org/wiki/%C3%89toile_(mod%C3%A8le_de_donn%C3%A9es))[https://fr.wikipedia.org/wiki/%C3%89toile_(mod%C3%A8le_de_donn%C3%A9es)]

# Démarrage rapide

Installer le package nuget
```
package-install DataQuery.Net
```

Mettre en place les dépendances dans le ConfigureServices :
```CSharp
services.RegisterSqlDataQueryServices(options => {
    options.ConnectionString = "{your SQL Server connection string here}";
});
```

Créer un Config provider en implémentant la classe IDataQueryConfigProvider :
```CSharp

  public class MyAwesomeDataQueryProvider : IDataQueryConfigProvider
  {
    public DataQueryCollections Provide()
    {
      var cnx = "Ma chaine de connexion à la BDD ici";
      var config = new DataQueryConfig(cnx) { };

      config.Tables["User"] = new Table()
      {
        // Le nom de la table et la clé passée au dictionnaire de table doivent matcher 
        Name = "User",
        // L'alias correspond à l'instruction AS du "select from table AS {alias}"
        Alias = "U",
        // Les propriétés correspondent à l'ensemble des dimensions ou métriques contenus dans la table que vous souhaitez requêter (Inutile de mettre l'intégralité des colonnes, juste ce dont vous avez besoin)
        Props = new List<DatabaseProp>
        {
          new DatabaseProp()
          {
					  // ALias : le nom avec lequel vous souhaitez requêter la propriété
            Alias = "UserId",
						// La colonne : correspond à ce qui va être sélectionné par le requêteur. Si c'est uune métrique, il faudra mettre une requête d'aggrégation SUM ou un COUNT
            Column = "U.Id",
						// La description du champ
            Description = "User's id",
            Label="Userid",
						// Le type SQL du champ sera utile pour parser les dimensions sélectionnés.
						SqlType = SqlDbType.Int,
						// Ce flag permet de déterminer si c'est une dimension visible ou non
            Aff = true,
						// A false par défaut, cette variable permet de déterminer si c'est une métrique ou non. Si s'en est une elle sera exclue automatiquement de la clause groupby. Si elle n'aggrège rien, il y aura une erreur
						IsMetric = false,
						// La jointure SQL à effectuer. La clé correspond au "Name" de la table, la valeur correspond à la propriété de jointure (En SQL, attention, ne pas prendre l'alias de colonne).
						// Pour que la jointure soit effective, il faut que cette jointure soit faite des deux côtés, dans "User_Stat" et dans "User".
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

      config.Tables["User_Stat"] = new Table()
      {
        Name = "User_Stat",
        Alias = "US",
        Props = new List<DatabaseProp>
        {
          new DatabaseProp()
          {
            Alias = "UserRef",
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
						// En passant ce flag à true, cette dimension sera utilisée pour filtrer les dates
						UsedToFilterDate = true,
            Description = "Date",
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


      return config;
    }
  }
```

Dans cet exemple, on a configurer deux tables de base de données avec les dimensions et métriques suivantes: 
User: Name, Email, UserId
User_Stat: Date, NbConnexion, UserRef

**Note importante**
Il faut que vos alias de métrique ou de dimensions soient uniques. Sinon, vous allez vous retrouver avec des fusions de dimensions, des comportements non voulus. Il ne lèvera aucune exception pour le moment.
A l'idéal, il faudra implémenter un système de validation de configuration.


# Requête des données
Pour exécuter une première requête SQL, vous pouvez procéder de cette façon :
```CSharp

public TestController : Controller
{
  public MyAwesomeDataQueryProvider _configProvider;
  public IDataQueryRepository _configProvider;
	public TestController(MyAwesomeDataQueryProvider configProvider, IDataQueryRepository repo){
		this._configProvider = configProvider;
		this._repo = repo;
	}


	[HttpGet]
	public IActionResult GetStats(DataQueryFilterParam params){
	  var results =	_repo.Query(_configProvider.Get(), params);
		return Ok(results);
	}

}

```

Exemple de requête :
```CSharp
// Récupération sur une semaine des connexions regroupées à la journée des utilisateurs avec un nom = Jean-Marc
/Test?dimensions=Date&metrics=NbConnexion&period=2w&asc=false&sort=Date&filters=Name%3DJean-Marc
```


# Paramètre des requêtes
Les paramètres de requêtage sont les suivants :
- aggregate : active ou désactive l'aggrégation automatique des données (le group by en SQL), par défaut à true;
- size : la taille du recordset (Max 10 000)
- page : l'index de la page (par défaut 1)
- query: Rquête full text (nécessite l'activation des requêtes full text dans la table)
- queryConstraint : pour limiter les champs de recherche full text (alias des dimensions séparées par des ,)
- start : date de début de la requête (Pas utilisé si "period" != null)
- end : date de fin de la requête (Pas utilisé si "period" != null)
- period : Période : 1w = 1 semaine, 3m = 3mois
- sort : Alias de métrique ou dimension sur laquelle on effectue le tri
- dimensions : liste des alias de dimensions à sélectionner séparées par des virgules, ex: Toto,Titi
- metrics : liste des alias de métriques à sélectionner séparées par des virgules, ex: Toto,Titi
- filters : filtre sur les dimensions ou métrique, ex: (Name==Toto,Name!=Titi);NbViews>12;Date>01/01/2020
, = OU / ; = ET / == = égal / != = différent / =~ = LIKE (avec un % sur la valeur)

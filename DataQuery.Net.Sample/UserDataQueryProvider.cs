using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DataQuery.Net.Sample
{
    public class UserDataQueryProvider : IDataQueryProvider
    {
        public DataQuerySchema Provide()
        {
            var config = new DataQuerySchema() { };

            config.AddTable(new Table("User")
            {
                // L'alias correspond à l'instruction AS du "select from table AS {alias}"
                Alias = "U",
                DisplayName = "Users",
                Root = true,
                // Les propriétés correspondent à l'ensemble des dimensions ou métriques contenus dans la table que vous souhaitez requêter (Inutile de mettre l'intégralité des colonnes, juste ce dont vous avez besoin)
                Columns = new List<Dimension>
                {
                    new Dimension()
                    {
                        // ALias : le nom avec lequel vous souhaitez requêter la propriété
                        Name = "UserId",
                        // La colonne : correspond à ce qui va être sélectionné par le requêteur. Si c'est uune métrique, il faudra mettre une requête d'aggrégation SUM ou un COUNT
                        SqlName = "U.Id",
                        // La description du champ
                        Description = "User's id",
                        DisplayName="Userid",
                        // Le type SQL du champ sera utile pour parser les dimensions sélectionnés.
                        SqlType = SqlDbType.Int,
                        // Ce flag permet de déterminer si c'est une dimension visible ou non
                        Displayed = true,
                        // A false par défaut, cette variable permet de déterminer si c'est une métrique ou non. Si s'en est une elle sera exclue automatiquement de la clause groupby. Si elle n'aggrège rien, il y aura une erreur
                        IsMetric = false,
                        // La jointure SQL à effectuer. La clé correspond au "Name" de la table, la valeur correspond à la propriété de jointure (En SQL, attention, ne pas prendre l'alias de colonne).
                        // Pour que la jointure soit effective, il faut que cette jointure soit faite des deux côtés, dans "User_Stat" et dans "User".
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

            config.AddTable(new Table("User_Stat")
            {
                Alias = "US",
                DisplayName = "User stats",
                Columns = new List<Dimension>
                {
                    new Dimension()
                    {
                        Name = "UserRef",
                        SqlName = "US.UserId",
                        Displayed = true,
                        SqlJoins = new Dictionary<string, string>
                        {
                            {"User", "Id" }
                        }
                    },
                    new Dimension()
                    {
                        Name = "Date",
                        SqlName = "US.Date",  
                        // En passant ce flag à true, cette dimension sera utilisée pour filtrer les dates
                        UsedToFilterDate = true,
                        Description = "Date",
                        SqlType = System.Data.SqlDbType.Date,
                        Displayed = true
                    },
                    new Dimension()
                    {
                        Name = "NbConnexion",
                        SqlName = "SUM(US.NbConnexion)",
                        Description = "NbConnexion",
                        DisplayName="NbConnexion",
                        IsMetric = true,
                        Displayed = true
                    }
                }
            });


            return config;
        }
    }
}

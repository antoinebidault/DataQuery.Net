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
                // Les propriétés correspondent à l'ensemble des dimensions ou métriques contenus dans la table que vous souhaitez requêter (Inutile de mettre l'intégralité des colonnes, juste ce dont vous avez besoin)
                Columns = new List<Column>
                {
                new Column()
                {
                // ALias : le nom avec lequel vous souhaitez requêter la propriété
                Alias = "UserId",
                // La colonne : correspond à ce qui va être sélectionné par le requêteur. Si c'est uune métrique, il faudra mettre une requête d'aggrégation SUM ou un COUNT
                ColumnName = "U.Id",
                // La description du champ
                Description = "User's id",
                Label="Userid",
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
            });

            config.AddTable(new Table("User_Stat")
            {
                Alias = "US",
                Columns = new List<Column>
                {
                new Column()
                {
                Alias = "UserRef",
                ColumnName = "US.UserId",
                Displayed = true,
                SqlJoins = new Dictionary<string, string>
                {
                {"User", "Id" }
                }
                },
                new Column()
                {
                Alias = "Date",
                ColumnName = "US.Date",  
                // En passant ce flag à true, cette dimension sera utilisée pour filtrer les dates
                UsedToFilterDate = true,
                Description = "Date",
                SqlType = System.Data.SqlDbType.Date,
                Displayed = true
                },
                new Column()
                {
                Alias = "NbConnexion",
                ColumnName = "SUM(US.NbConnexion)",
                Description = "NbConnexion",
                Label="NbConnexion",
                IsMetric = true,
                Displayed = true
                }
                }
            });


            return config;
        }
    }
}

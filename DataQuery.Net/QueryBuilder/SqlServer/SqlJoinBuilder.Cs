using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQuery.Net
{

    /// <summary>
    /// This generates the SQL tables join
    /// The most complex part is here
    /// Uses the djiskra algorithm in order to find the right path
    /// </summary>
    public class SqlJoinBuilder
    {
        readonly DataQuerySchema _config;
        IDictionary<string, Table> tablesToLink;
        IDictionary<string, Table> tablesIncluded;

        /// <summary>
        /// Nécessite la config du dataquery
        /// </summary>
        /// <param name="config">Config du DataQuery</param>
        public SqlJoinBuilder(DataQuerySchema config)
        {
            this._config = config;
        }



        /// <summary>
        /// Récupération de la liste des filtres par défaut contenus 
        /// dans les tables inclues dans la requêtes
        /// </summary>
        public string GetDefaultFilters()
        {
            string output = "";
            foreach (var table in this.tablesIncluded.Values)
            {
                if (table.DefaultFilterUsedIfTableUsed == null)
                    continue;

                var filters = table.DefaultFilterUsedIfTableUsed.Replace("<table>", table.Alias);
                if (!string.IsNullOrEmpty(filters))
                {
                    if (output != "")
                        output += " AND ";

                    output += string.Format(" ({0}) ", filters);
                }
            }
            return output;
        }


        /// <summary>
        /// Génère les jointures sql
        /// </summary>
        /// <param name="param"></param>
        /// <returns>table1 A1 INNER JOIN table2 A2 ON A1.id = A2.id </returns>
        public string JoinSqlString(List<string> param)
        {
            this.tablesToLink = new Dictionary<string, Table>();

            //Créer la liste des tables à lier
            foreach (string val in param)
            {
                foreach (var table in _config.Tables.Values)
                {
                    foreach (var prop in table.Columns)
                    {
                        if (val == prop.Alias)
                        {
                            tablesToLink[table.Alias] = table;
                            break;
                        }
                    }
                }
            }

            this.tablesIncluded = new Dictionary<string, Table>();
            List<TableJoining> joins = new List<TableJoining>();

            if (tablesToLink.Count == 1)
            {
                tablesIncluded[tablesToLink.Values.First().Alias] = tablesToLink.Values.First();
                joins = new List<TableJoining>
        {
          new TableJoining {  From = tablesToLink.Values.First() }
        };
            }
            else
            {
                //Récupère la liste des neurones
                var neurons = GetNeuralNetwork();

                //Pour chaque table il va calculer le chemin le pluS court
                foreach (var tableFrom in tablesToLink.Values.OrderBy(m => m.Size))
                {
                    foreach (var tableTo in tablesToLink.Values.Where(m => m != tableFrom))
                    {
                        List<Table> tmpList = new List<Table>();
                        if (tableFrom == null)
                            break;

                        tmpList = Djiskra(tableFrom, tableTo, neurons);

                        if (tmpList.Count > 0)
                        {
                            int j = 0;
                            foreach (Table elt in tmpList)
                            {
                                var to = tmpList.ElementAtOrDefault(j + 1);
                                joins.Add(new TableJoining() { From = elt, To = to ?? null, Number = j });
                                tablesIncluded[elt.Alias] = elt;
                                j++;
                            }
                        }
                    }
                }
            }

            string output = "";
            var tableClones = tablesIncluded.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
            output += GenerateJoin(tableClones, joins);

            return output;
        }
        /// <summary>
        /// Récupère la liste des chemins possibles sous forme de liste affectés à une pondération
        /// Dans mon système, la pondération est toujours égale à 1.
        /// </summary>
        /// <returns></returns>
        private Dictionary<Table, Dictionary<Table, int>> GetNeuralNetwork()
        {
            var neurons = new Dictionary<Table, Dictionary<Table, int>>();
            foreach (Table table in _config.Tables.Values)
            {
                neurons[table] = new Dictionary<Table, int>();
                foreach (var link in table.Columns.SelectMany(m => m.SqlJoins))
                {
                    if (_config.Tables.ContainsKey(link.Key))
                    {
                        neurons[table].Add(_config.Tables[link.Key], 1);
                    }
                }
            }
            return neurons;
        }

        /// <summary>
        /// Compilation du chemin det génération de la jointure SQL
        /// </summary>
        /// <param name="tablesIncluded"></param>
        /// <param name="paths"></param>
        private string GenerateJoin(IDictionary<string, Table> tablesIncluded, List<TableJoining> paths)
        {
            string output = "";
            List<Table> visited = new List<Table>();

            //Si aucun chemin
            if (!paths.Any())
                return null;

            //Première table
            output += $"[{paths.First().From.SqlName}] AS {paths.First().From.Alias}  ";
            tablesIncluded.Remove(paths.First().From.Alias);

            //Compilation du chemin
            while (tablesIncluded.Values.Count > 0)
            {
                foreach (var join in paths)
                {
                    if (tablesIncluded.ContainsKey(join.From.Alias))
                        tablesIncluded.Remove(join.From.Alias);

                    visited.Add(join.From);

                    if (join.To != null && tablesIncluded.ContainsKey(join.To.Alias))
                    {
                        tablesIncluded.Remove(join.To.Alias);
                        visited.Add(join.To);
                        if (join.To.TypeJoin == TypeJoin.LEFTOUTER || join.From.TypeJoin == TypeJoin.LEFTOUTER)
                        {
                            output += " LEFT OUTER JOIN ";
                        }
                        else
                        {
                            output += " INNER JOIN ";
                        }
                        output += "[" + join.To.SqlName + "] AS " + join.To.Alias + " ";
                        output += " ON ";
                        output += GetOnStatement(join);
                        break;
                    }
                }
            }
            return output;
        }

        /// <summary>
        /// Génère le ON à partir d'une jointure SQL
        /// </summary>
        /// <param name="joining"></param>
        private string GetOnStatement(TableJoining joining)
        {
            string output = "";
            if (joining.To != null && joining.From != null)
            {
                Dimension prop = joining.From.Columns.FirstOrDefault(m => m.SqlJoins != null && m.SqlJoins.ContainsKey(joining.To.Alias));
                output += joining.From.Alias + "." + prop.SqlName + "=" + joining.To.Alias + "." + prop.SqlJoins[joining.To.Alias];
            }
            return output;
        }


        /// <summary>
        /// https://fr.wikipedia.org/wiki/Algorithme_de_Dijkstra
        /// Dijskstra:
        /// Search for the shortest pattern
        /// </summary>
        /// <param name="start">Start table</param>
        /// <param name="finish">End table</param>
        /// <param name="paths">Neural network => a dictionary of tables with their list of connexions (the int is the power of each connexion)</param>
        /// <returns></returns>
        public List<Table> Djiskra(Table start, Table finish, IDictionary<Table, Dictionary<Table, int>> paths)
        {
            var previous = new Dictionary<Table, Table>();
            var distances = new Dictionary<Table, int>();
            var nodes = new List<Table>();
            List<Table> path = new List<Table>();

            foreach (var vertex in paths)
            {
                if (vertex.Key == start)
                {
                    distances[vertex.Key] = 0;
                }
                else
                {
                    distances[vertex.Key] = int.MaxValue;
                }

                nodes.Add(vertex.Key);
            }

            while (nodes.Count != 0)
            {
                nodes.Sort((x, y) => distances[x] - distances[y]);

                var smallest = nodes[0];
                nodes.Remove(smallest);

                if (smallest == finish)
                {
                    path = new List<Table>();
                    while (previous.ContainsKey(smallest))
                    {
                        path.Add(smallest);
                        smallest = previous[smallest];
                    }

                    break;
                }

                if (distances[smallest] == int.MaxValue)
                {
                    break;
                }

                foreach (var neighbor in paths[smallest])
                {
                    var alt = distances[smallest] + neighbor.Value;
                    if (alt < distances[neighbor.Key])
                    {
                        distances[neighbor.Key] = alt;
                        previous[neighbor.Key] = smallest;
                    }
                }
            }
            if (path != null)
            {
                path.Add(start);
                path.Reverse();
            }
            return path;
        }

    }

    public class TableJoining
    {
        public Table From { get; set; }
        public Table To { get; set; }
        public int Number { get; set; }
    }

    public class Path
    {
        public List<TableJoining> Paths { get; set; }
        public int Nb { get { return Paths.Count == 1 ? 10000 : Paths.Count; } }
        public Table Start { get; internal set; }
        public Table To { get; internal set; }
    }
}

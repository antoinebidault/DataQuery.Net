using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;


namespace DataQuery.Net
{

    /// <summary>
    /// Les informations récupérées dans la requête GET ou POST (GET reco)
    /// </summary>
    public class DataQueryFilterParams
    {
        /// <summary>
        /// Wether aggregation is triggered or not (By default, the data are grouped by)
        /// </summary>
        public bool Aggregate { get; set; } = true;

        /// <summary>
        /// Nombre de lignes à récupérer par page (Max 10 000)
        /// </summary>
        public int? Size { get; set; }

        /// <summary>
        /// N° de la page en commençant par 1. (Par défaut : 1)
        /// </summary>
        public int? Page { get; set; }

        /// <summary>
        /// full text search
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// full text search restriction : default = * = all criteres
        /// </summary>
        public string QueryConstraints { get; set; }

        /// <summary>
        /// Date de début
        /// </summary>
        public DateTime? Start { get; set; }

        /// <summary>
        /// Date de fin de l'analyse (Par défaut)
        /// </summary>
        public DateTime? End { get; set; }

        /// <summary>
        /// Période : 1w = 1 semaine, 3m = 3mois
        /// </summary>
        [RegularExpression(@"^[0-9]{1,2}[dhmwy]$")]
        public string Period { get; set; }

        /// <summary>
        /// Nom du champ à trier (1 seul possible)
        /// </summary>
        public string Sort { get; set; }

        /// <summary>
        /// par défaut à false (Descendant), ordre du tri.
        /// </summary>
        public bool Asc { get; set; } = false;

        /// <summary>
        /// Liste des métriques (séparées par des ,)
        /// Case insensitive
        /// ex: Paps,clics
        /// </summary>
        public string Metrics { get; set; }

        /// <summary>
        /// Liste des dimensions (Séparées par des ,)
        /// ex: Date,Campaign
        /// </summary>
        public string Dimensions { get; set; }

        /// <summary>
        /// Filtre au format google (IdCookie==10310501,EventKey!=cat)
        /// </summary>
        public string Filters { get; set; }

        /// <summary>
        /// Si je veux un truc randomizé ou pas
        /// </summary>
        public bool Random { get; set; }

        /// <summary>
        /// Ex : -IdList:1,IdList:12
        /// </summary>
        public string Includes { get; set; }

        /// <summary>
        /// Champ sur lequel on applique les filtres start,end
        /// Laisser vide si vous préférez que le dataquery choisisse le champs de filtre de date pour vous.
        /// </summary>
        public string ForcedDateFilter { get; set; }

        /// <summary>
        /// Used for very large filters
        /// using a IN query
        /// </summary>
        public IDictionary<string, string[]> BatchFilters { get; set; }


        /// <summary>
        /// Bind datas to a DataQueryFilter
        /// </summary>
        /// <param name="dqFilter"></param>
        /// <param name="config"></param>
        public void BindTo(DataQueryFilter dqFilter, DataQueryCollections config)
        {
            //Propriétés simples
            dqFilter.Random = Random;
            dqFilter.DateDebut = Start;
            dqFilter.DateFin = End;
            dqFilter.FullTextQuery = Query;

            if (!string.IsNullOrEmpty(this.Period))
            {
                try
                {
                    dqFilter.DateFin = DateTime.Now;
                    string type = this.Period.Substring(this.Period.Length - 1, this.Period.Length);
                    int nb = Convert.ToInt32(this.Period.TrimEnd(type.ToCharArray()));

                    switch (type)
                    {
                        case "d":
                            dqFilter.DateDebut = DateTime.Now.AddDays(-nb);
                            break;
                        case "y":
                            dqFilter.DateDebut = DateTime.Now.AddYears(-nb);
                            break;
                        case "m":
                            dqFilter.DateDebut = DateTime.Now.AddMonths(-nb);
                            break;
                        case "w":
                            dqFilter.DateDebut = DateTime.Now.AddDays(-nb * 7);
                            break;
                        case "h":
                            dqFilter.DateDebut = DateTime.Now.AddHours(-nb);
                            break;
                    }
                }
                catch
                {
                    throw new Exception("Impossible de parser le paramètre pédiode");
                }
            }

            dqFilter.PageSize = Size;
            dqFilter.PageIndex = Page;
            dqFilter.Aggregate = Aggregate;

            // Données incluses
            if (!string.IsNullOrEmpty(Includes))
            {
                foreach (string dim in Includes.Split(',').Select(m => m.Trim()))
                {
                    dqFilter.Inclusions.Add(new Inclusion(dim, config));
                }
            }

            // filterClean.PaginationDimension = config.Dimensions[this.paginationDimension];
            // Dimensions
            if (!string.IsNullOrEmpty(Dimensions))
            {
                foreach (string dim in Dimensions.Split(',').Select(m => m.Trim()))
                {
                    if (config.Dimensions.ContainsKey(dim))
                    {
                        if (config.Dimensions[dim].IsGeography)
                            throw new Exception(string.Format("Vous sélectionnez un type géography, ce qui n'est pas autorisé : {0} ! ", dim));

                        if (!config.Dimensions[dim].AllowedToExport)
                            throw new Exception(string.Format("Vous n'avez pas les droits pour exporter cette dimension : {0} ! ", dim));

                        dqFilter.Dimensions.Add(config.Dimensions[dim]);
                    }
                }
            }

            // Add a little check to the batch filters
            if (BatchFilters != null && BatchFilters.Count() > 0)
            {
                foreach (var keyValuePair in BatchFilters)
                {
                    if (config.Dimensions.ContainsKey(keyValuePair.Key))
                    {
                        if (!config.Dimensions[keyValuePair.Key].AllowedToRequest)
                            throw new Exception(string.Format("Vous n'avez pas les droits pour requêter en batch cette dimension : {0} ! ", keyValuePair.Key));

                        var dt = new DataTable();
                        dt.Columns.Add(new DataColumn("data", typeof(string)));
                        foreach (var item in keyValuePair.Value)
                        {
                            dt.Rows.Add(item);
                        }

                        dqFilter.BatchFilters.Add(config.Dimensions[keyValuePair.Key], dt);
                    }
                }
            }

            // Full text searching
            if (!string.IsNullOrEmpty(Query))
            {
                if (!string.IsNullOrEmpty(QueryConstraints))
                {
                    foreach (string dim in QueryConstraints.Split(',').Select(m => m.Trim()))
                    {
                        if (config.Dimensions.ContainsKey(dim))
                        {
                            if (config.Dimensions[dim].IsGeography)
                                throw new Exception(string.Format("La contrainte de recherche suivante n'est pas autorisée : {0} ! ", dim));

                            if (!config.Dimensions[dim].AllowedToRequest && !config.Dimensions[dim].AllowedToExport)
                                throw new Exception(string.Format("La contrainte de recherche suivante n'est pas autorisée à l'export, contactez l'admin : {0} ! ", dim));

                            dqFilter.FullTextQueryConstraints.Add(config.Dimensions[dim]);
                        }
                    }
                }
            }


            //ForcedDateFilter
            if (!string.IsNullOrEmpty(this.ForcedDateFilter))
            {
                foreach (string dim in ForcedDateFilter.Split(',').Select(m => m.Trim()))
                {
                    if (config.Dimensions.ContainsKey(dim))
                    {
                        if (!config.Dimensions[dim].AllowedToRequest && config.Dimensions[dim].SqlType != SqlDbType.DateTime && config.Dimensions[dim].SqlType != SqlDbType.Date)
                            throw new Exception(string.Format("Le paramètre forcedDateFilter est incorrecte : {0} ! ", dim));

                        dqFilter.ForcedDateFilter.Add(config.Dimensions[dim]);
                    }
                }
            }

            //Métriques
            if (!string.IsNullOrEmpty(Metrics))
            {
                foreach (string met in Metrics.Split(',').Select(m => m.Trim()))
                {
                    if (config.Metrics.ContainsKey(met))
                    {

                        if (config.Metrics[met].IsGeography)
                            throw new Exception(string.Format("Vous sélectionnez un type géography, ce qui n'est pas autorisé : {0} ! ", met));

                        if (!config.Metrics[met].AllowedToExport)
                            throw new Exception(string.Format("Vous n'avez pas les droits pour exporter cette métrique : {0} ! ", met));

                        dqFilter.Metrics.Add(config.Metrics[met]);
                    }
                }
            }



            //Arrive sous la forme de (test==12,titi>=11);test==12
            //Or = ,
            //And = ;
            //Comme le queryexplorer de google
            string str = string.Empty;
            if (!string.IsNullOrEmpty(Filters))
            {
                for (int i = 0; i < Filters.Length; i++)
                {
                    str += Filters[i];
                    if (Filters[i] == ';' || Filters[i] == ',' || i == Filters.Length - 1)
                    {
                        dqFilter.Filters.Add(new Filter(str, config));
                        str = "";
                    }
                }
            }

            //Tri par dimension ou métrique
            if (!string.IsNullOrEmpty(this.Sort))
            {
                Sort sortClean = new Sort { Prop = config.Metrics.Values.First(), Asc = this.Asc };
                if (Dimensions != null && !string.IsNullOrEmpty(this.Dimensions) && Dimensions.Contains(this.Sort) && config.Dimensions.ContainsKey(this.Sort))
                {
                    sortClean.Prop = config.Dimensions[this.Sort];
                }
                else if (Metrics != null && Metrics.Contains(this.Sort) && config.Metrics.ContainsKey(this.Sort))
                {
                    sortClean.Prop = config.Metrics[this.Sort];
                }
                else
                {
                    throw new Exception("La variable de tri doit faire partie des métriques ou des dimensions");
                }
                dqFilter.Sorts.Add(sortClean);
            }


            // Dependent tables
            foreach (Table table in config.Tables.Values)
            {
                foreach (DatabaseProp prop in table.Props)
                {

                    foreach (DatabaseProp dim in dqFilter.ForcedDateFilter)
                    {
                        if (dim.Alias == prop.Alias)
                        {
                            dqFilter.Tables[table.Name] = table;
                        }
                    }

                    foreach (DatabaseProp metric in dqFilter.Metrics)
                    {
                        if (metric.Alias == prop.Alias)
                        {
                            dqFilter.Tables[table.Name] = table;
                        }
                    }

                    foreach (DatabaseProp dim in dqFilter.Dimensions)
                    {
                        if (dim.Alias == prop.Alias)
                        {
                            dqFilter.Tables[table.Name] = table;
                        }
                    }

                    foreach (Filter filter in dqFilter.Filters)
                    {
                        if (filter.Dimension.Alias == prop.Alias)
                        {
                            dqFilter.Tables[table.Name] = table;
                        }
                    }
                }
            }
        }
    }



}

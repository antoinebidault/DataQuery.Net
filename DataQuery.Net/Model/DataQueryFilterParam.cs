using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;


namespace DataQuery.Net
{

    /// <summary>
    /// The filter query param can be passed as argument of a controller action
    /// in  GET ou POST
    /// </summary>
    public class DataQueryFilterParams
    {
        /// <summary>
        /// Wether aggregation is triggered or not (By default, the data are grouped by)
        /// </summary>
        public bool Aggregate { get; set; } = true;

        /// <summary>
        /// Number of lines to get (the max is set in DataQuery config)
        /// </summary>
        public int? Size { get; set; }

        /// <summary>
        /// Page index
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
        /// database columns alias used to sort data (1 at a time)
        /// </summary>
        public string Sort { get; set; }

        /// <summary>
        /// False by default, set the sort direction
        /// </summary>
        public bool Asc { get; set; } = false;

        /// <summary>
        /// Comma separated database columns alias set as metric
        /// The prop name are case insensitive
        /// ex: Clics, Connexions
        /// </summary>
        public string Metrics { get; set; }

        /// <summary>
        /// Comma separated database columns alias
        /// e.g. Date,Campaign
        /// </summary>
        public string Dimensions { get; set; }

        /// <summary>
        /// Filters on Google format (IdCookie==10310501,EventKey!=cat)
        /// https://developers.google.com/analytics/devguides/reporting/core/v3/reference#filters
        /// </summary>
        public string Filters { get; set; }

        /// <summary>
        /// Set to true for randomizing the result
        /// </summary>
        public bool Random { get; set; }

        /// <summary>
        /// Ex : -IdList:1,IdList:12
        /// </summary>
        public string Includes { get; set; }

        /// <summary>
        /// Force the field used to filter date with start and end params
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
        public DataQueryFilter Parse(DataQuerySchema config)
        {
            var dqFilter = new DataQueryFilter();
            dqFilter.Random = Random;
            dqFilter.FullTextQuery = Query;


            /*
            if (!string.IsNullOrEmpty(this.Period))
            {
                try
                {
                    dqFilter.DateFin = DateTime.Now;
                    string type = this.Period.Substring(this.Period.Length - 1, 1);
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
            }*/

            dqFilter.PageSize = Size;
            dqFilter.PageIndex = Page;
            dqFilter.Aggregate = Aggregate;

            // Included data
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
                            throw new InvalidOperationException(string.Format("Vous sélectionnez un type géography, ce qui n'est pas autorisé : {0} ! ", dim));

                        if (!config.Dimensions[dim].AllowedToView)
                            throw new InvalidOperationException(string.Format("Vous n'avez pas les droits pour exporter cette dimension : {0} ! ", dim));

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
                        if (!config.Dimensions[keyValuePair.Key].AllowedToFilter)
                            throw new InvalidOperationException(string.Format("Vous n'avez pas les droits pour requêter en batch cette dimension : {0} ! ", keyValuePair.Key));

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
                                throw new InvalidOperationException(string.Format("La contrainte de recherche suivante n'est pas autorisée : {0} ! ", dim));

                            if (!config.Dimensions[dim].AllowedToFilter && !config.Dimensions[dim].AllowedToView)
                                throw new InvalidOperationException(string.Format("La contrainte de recherche suivante n'est pas autorisée à l'export, contactez l'admin : {0} ! ", dim));

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
                        if (!config.Dimensions[dim].AllowedToFilter && config.Dimensions[dim].SqlType != SqlDbType.DateTime && config.Dimensions[dim].SqlType != SqlDbType.Date)
                            throw new InvalidOperationException(string.Format("Le paramètre forcedDateFilter est incorrecte : {0} ! ", dim));

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
                            throw new InvalidOperationException(string.Format("Vous sélectionnez un type géography, ce qui n'est pas autorisé : {0} ! ", met));

                        if (!config.Metrics[met].AllowedToView)
                            throw new InvalidOperationException(string.Format("Vous n'avez pas les droits pour exporter cette métrique : {0} ! ", met));

                        dqFilter.Metrics.Add(config.Metrics[met]);
                    }
                }
            }



            // Converty (test==12,titi>=11);test==12
            //Or = ,
            //And = ;
            // Like Google query explorer
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

            // Order by a specific dimension
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
                    throw new InvalidOperationException("La variable de tri doit faire partie des métriques ou des dimensions");
                }
                dqFilter.Sorts.Add(sortClean);
            }


            // Dependent tables
            foreach (Table table in config.Tables.Values)
            {
                foreach (Column prop in table.Columns)
                {

                    foreach (Column dim in dqFilter.ForcedDateFilter)
                    {
                        if (dim.Name == prop.Name)
                        {
                            dqFilter.Tables[table.Name] = table;
                        }
                    }

                    foreach (Column metric in dqFilter.Metrics)
                    {
                        if (metric.Name == prop.Name)
                        {
                            dqFilter.Tables[table.Name] = table;
                        }
                    }

                    foreach (Column dim in dqFilter.Dimensions)
                    {
                        if (dim.Name == prop.Name)
                        {
                            dqFilter.Tables[table.Name] = table;
                        }
                    }

                    foreach (Filter filter in dqFilter.Filters)
                    {
                        if (filter.Dimension.Name == prop.Name)
                        {
                            dqFilter.Tables[table.Name] = table;
                        }
                    }
                }
            }


            if (!dqFilter.Tables.Any(m => m.Value.Root))
            {
                throw new InvalidOperationException("You must at least query a root property that is connected");
            }

            if (!dqFilter.Tables.Values.IsConnected(config.Tables.Values))
            {
                throw new InvalidOperationException("Tables must be connected ");

            }

            return dqFilter;
        }
    }
}

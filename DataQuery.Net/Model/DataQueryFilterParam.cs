﻿using System;
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
        /// False by default, set the sort direction
        /// </summary>
        public bool Count { get; set; } = false;

        /// <summary>
        /// Comma separated database columns alias (metrics or dimensions are allowed)
        /// e.g. Date,Campaign
        /// </summary>
        public IEnumerable<string> Dimensions { get; set; }

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
            dqFilter.PageSize = Size;
            dqFilter.PageIndex = Page;
            dqFilter.Aggregate = Aggregate;
            dqFilter.DisableCounting = Count;

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
            if (Dimensions != null && Dimensions.Any())
            {
                foreach (string dim in Dimensions.Select(m => m.Trim()))
                {
                    if (config.MetricsAndDimensions.ContainsKey(dim))
                    {
                        if (config.MetricsAndDimensions[dim].IsGeography)
                            throw new InvalidOperationException(string.Format("Vous sélectionnez un type géography, ce qui n'est pas autorisé : {0} ! ", dim));

                        if (!config.MetricsAndDimensions[dim].AllowedToView)
                            throw new InvalidOperationException(string.Format("Vous n'avez pas les droits pour exporter cette dimension : {0} ! ", dim));

                        dqFilter.Dimensions.Add(config.MetricsAndDimensions[dim]);
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


            /*
            //Métriques
            if (Metrics != null && Metrics.Any())
            {
                foreach (string met in Metrics.Select(m => m.Trim()))
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
            }*/



            // Converty (test==12,titi>=11);test==12
            //Or = ,
            //And = ;
            // Like Google query explorer
            string str = string.Empty;
            if (!string.IsNullOrEmpty(Filters))
            {
                var escaped = false;
                for (int i = 0; i < Filters.Length; i++)
                {
                    str += Filters[i];

                    if (Filters[i] == '\\')
                    {
                        escaped = true;
                    }
                    else if (!escaped)
                    {
                        if (Filters[i] == ';' || Filters[i] == ',' || i == Filters.Length - 1)
                        {
                            dqFilter.Filters.Add(new Filter(str, config));
                            str = "";
                        }
                    }
                    else if (escaped)
                    {
                        escaped = false;
                    }
                }
            }

            // Order by a specific dimension
            if (!string.IsNullOrEmpty(this.Sort))
            {
                Sort sortClean = new Sort { Prop = config.MetricsAndDimensions[this.Dimensions.FirstOrDefault()], Asc = this.Asc };
                if (Dimensions != null && this.Dimensions.Any() && Dimensions.Contains(this.Sort) && config.MetricsAndDimensions.ContainsKey(this.Sort))
                {
                    sortClean.Prop = config.MetricsAndDimensions[this.Sort];
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
                foreach (Dimension prop in table.Columns)
                {
                    foreach (Dimension dim in dqFilter.Dimensions)
                    {
                        if (dim.Alias == prop.Alias)
                        {
                            dqFilter.Tables[table.Alias] = table;
                        }
                    }

                    foreach (Filter filter in dqFilter.Filters)
                    {
                        if (filter.Dimension.Alias == prop.Alias)
                        {
                            dqFilter.Tables[table.Alias] = table;
                        }
                    }
                }
            }

            /*
            if (!dqFilter.Tables.Any(m => m.Value.Root))
            {
                throw new InvalidOperationException("You must at least query a root table that is connected");
            }*/

            if (!dqFilter.Tables.Values.IsConnected(config.Tables.Values))
            {
                throw new InvalidOperationException("Tables must be connected ");
            }

            return dqFilter;
        }
    }
}

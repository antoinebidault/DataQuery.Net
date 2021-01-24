using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DataQuery.Net
{

    /// <summary>
    /// Retrieved data from navigator
    /// </summary>
    public class DataQueryFilter
    {
        public DataQueryFilter()
        {
            DateDebut = null;
            DateFin = DateTime.Now;
            Sorts = new List<Sort>() { };
            Dimensions = new List<DatabaseProp>() { };
            FullTextQueryConstraints = new List<DatabaseProp>() { };
            Tables = new Dictionary<string, Table>();
            Metrics = new List<DatabaseProp>() { };
            Filters = new List<Filter>() { };
            Inclusions = new List<Inclusion>();
            ForcedDateFilter = new List<DatabaseProp>();
            BatchFilters = new Dictionary<DatabaseProp, DataTable>();
        }

        public int? PageSize { get; set; }
        public int? PageIndex { get; set; }
        public bool Aggregate { get; set; } = true;
        public bool Random { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public List<Sort> Sorts { get; set; }
        public List<DatabaseProp> Metrics { get; set; }
        public List<DatabaseProp> Dimensions { get; set; }
        public bool IsFullTextQuery { get { return !string.IsNullOrEmpty(FullTextQuery); } }
        public string FullTextQuery { get; internal set; }
        public List<DatabaseProp> FullTextQueryConstraints { get; set; }
        public IDictionary<string, Table> Tables { get; set; }
        public List<Filter> Filters { get; set; }
        public IDictionary<DatabaseProp, DataTable> BatchFilters { get; set; }
        public DatabaseProp PaginationDimension { get; set; }
        public List<Inclusion> Inclusions { get; set; }
        public List<DatabaseProp> ForcedDateFilter { get; set; }

        public List<string> GetListOfDimension()
        {
            //Renvoie une liste concaténée de dim, filtres et métriques
            return Metrics.Select(m => m.Alias).Union(Dimensions.Select(m => m.Alias)).Union(ForcedDateFilter.Select(m => m.Alias)).Union(Filters.Select(m => m.Dimension.Alias)).ToList();
        }

    }


    public class Sort
    {
        public DatabaseProp Prop { get; set; }
        public bool Asc { get; set; }
    }




}

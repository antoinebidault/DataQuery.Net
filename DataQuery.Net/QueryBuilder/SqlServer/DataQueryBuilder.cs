
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;

namespace DataQuery.Net
{
    public class DataQuerySqlServerBuilder
    {
        public DataQuerySqlServerBuilder(DataQueryOptions options)
        {
            this._options = options;
        }

        private DataQuerySchema _config { get; set; }
        private List<string> _sqlSelect;
        private List<string> _sqlGroupBy;
        private List<string> _sqlWhere;
        private List<string> _sqlOrderBy;
        private List<string> _sqlSelectCte;
        private List<SqlParameter> _sqlWhereParams;


        private StringBuilder _whereStatement;


        /// <summary>
        /// Generate an as hoc query using params + collections
        /// </summary>
        /// <param name="param"></param>
        /// <param name="conf"></param>
        /// <returns></returns>
        public DataQueryResult Query(DataQuerySchema conf, DataQueryFilter filters)
        {
            // Set size
            if (filters.PageSize > _options.MaxRecordsetSize)
            {
                filters.PageSize = _options.MaxRecordsetSize;
            }

            //Define config in global scope
            this._config = conf;

            //Init params
            _sqlSelect = new List<string>();
            _sqlSelectCte = new List<string>() { "RowNum" };
            _sqlGroupBy = new List<string>();
            _sqlWhere = new List<string>();
            _sqlOrderBy = new List<string>();
            _sqlWhereParams = new List<SqlParameter>();
            this._whereStatement = new StringBuilder();
            this.firstWhereStatement = true;

            SqlJoinBuilder builder = new SqlJoinBuilder(conf);
            string joinString = builder.JoinSqlString(filters.GetListOfDimension());

            // Query results
            var result = new DataQueryResult();

            StringBuilder sqlSelectQuery = new StringBuilder();
            StringBuilder sqlCountQuery = new StringBuilder();
            StringBuilder sqlQuery = new StringBuilder();

            // DIMENSIONS
            foreach (Column champ in filters.Dimensions)
            {
                _sqlSelect.Add(champ.SqlName + " AS " + champ.Alias);
                _sqlSelectCte.Add(champ.Alias);
                _sqlGroupBy.Add(champ.SqlName);
            }

            // Metrics handling
            foreach (Column champ in filters.Metrics)
            {
                _sqlSelect.Add(champ.SqlName + " AS " + champ.Alias);
                _sqlSelectCte.Add(champ.Alias);
            }

            // Sorting
            if (filters.Sorts.Any() && !filters.Random)
            {
                foreach (Sort sort in filters.Sorts)
                {
                    _sqlOrderBy.Add(sort.Prop.SqlName + " " + (sort.Asc ? "ASC" : "DESC"));
                }
            }
            else if (filters.Random)
            {
                // If Random
                _sqlOrderBy.Add("NEWID() ASC");
            }

            // Duplicate removing
            RemoveDuplicates();

            // Order by
            string sqlOrder = string.Join(",", _sqlOrderBy);

            // Select building
            sqlSelectQuery.Append(" SELECT ");
            sqlSelectQuery.Append(string.Join(",", _sqlSelect));
            sqlSelectQuery.AppendLine();

            // Select count
            sqlCountQuery.Append("SELECT COUNT(*) OVER () AS totalRows ");
            sqlCountQuery.AppendLine();

            // Build of joinings
            sqlQuery.Append(" FROM ");

            // Joining
            sqlQuery.Append(joinString);
            sqlQuery.AppendLine();


            string filterDefault = builder.GetDefaultFilters();
            if (!string.IsNullOrEmpty(filterDefault))
            {
                AppendWhere(" ( " + filterDefault + " ) ");
            }

            // Startdate and end date
            // AppendDateQuery(filters);

            // Filters on dimensions
            IEnumerable<Filter> filtersOnDimension = filters.Filters.Where(m => !m.Dimension.IsMetric);
            HandleWhereFilters(filtersOnDimension);

            // Batch filtering
            HandleBatchWhereFilters(filters.BatchFilters);

            // Full text queries
            HandleFullTextQueries(filters);

            // Inclusions handlinh
            AppendInclusion(filters.Inclusions);


            if (_whereStatement.Length > 0)
            {
                sqlQuery.Append(_whereStatement.ToString());
            }

            sqlQuery.AppendLine();
            if (_sqlGroupBy.Any() && filters.Aggregate)
            {
                sqlQuery.Append(" GROUP BY ");
                sqlQuery.Append(string.Join(",", _sqlGroupBy));
            }


            // Filters on metrics using HAVING statements
            AppendHavingStatements(filters, sqlQuery);

            // Order BY
            if (!string.IsNullOrEmpty(sqlOrder))
                sqlQuery.Append(" ORDER BY " + sqlOrder + " ");
            else
                sqlQuery.Append(" ORDER BY (SELECT NULL) ");


            string sql = "";
            string sqlCount = "";

            // OFFSET Pagination
            if (filters.PageSize.HasValue && filters.PageIndex.HasValue)
            {

                // Query formatting
                sqlCount = sqlCountQuery.ToString() + " " + sqlQuery.ToString();

                // Query selecting
                sql += sqlSelectQuery.ToString() + " " + sqlQuery.ToString();

                // Paginate
                sql += " OFFSET @pageS ROWS FETCH NEXT @pageE ROWS ONLY ";

                AddParameter("@pageS", SqlDbType.Int, (filters.PageSize * (filters.PageIndex - 1)));
                AddParameter("@pageE", SqlDbType.Int, (filters.PageSize));
            }
            else
            {
                sql = sqlSelectQuery.ToString() + " " + sqlQuery.ToString();
            }

            // Debug log
            Debug.WriteLine("------------------------ DataQuery Stored procedure  ----------------------------------------");
            _sqlWhereParams.ForEach(p => Debug.WriteLine("DECLARE " + p.ParameterName + " " + p.SqlDbType.ToString() + " = '" + p.Value + "'"));
            Debug.WriteLine(sql);
            Debug.WriteLine("------------------------------------------------------------------------------------");

            if (_options.IncludeQueryProfiling)
            {
                result.SqlQuery = sql;
            }
            var timestamp = DateTime.Now;

            using (var cnx = new SqlConnection(_options.ConnectionString))
            {
                cnx.Open();
                using (var manager = new SqlCommand(sql, cnx))
                {

                    //Ajout des params
                    foreach (var p in _sqlWhereParams)
                        manager.Parameters.Add(p);

                    try
                    {
                        //Requête
                        using (SqlDataReader reader = manager.ExecuteReader())
                        {
                            result.Data = new DataQueryTable();
                            bool columnDefined = false;

                            result.Data.PageIndex = filters.PageIndex;
                            result.Data.PageSize = filters.PageSize;
                            //Colonnes d'entête
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (!columnDefined)
                                {
                                    //Ajout d'une colonne
                                    var column = new DataQueryColumn();
                                    var metric = _config.MetricsAndDimensions[reader.GetName(i)];
                                    column.Name = metric.Alias;
                                    column.Label = metric.DisplayName;
                                    column.IsMetric = metric.IsMetric;
                                    column.Unite = metric.Unit;
                                    column.Color = metric.Color;
                                    column.Type = metric.SqlType.ToString();
                                    result.Data.Columns.Add(column);
                                }
                            }

                            object val = null;
                            List<object> row = null;
                            while (reader.Read())
                            {
                                //Ligne de données
                                row = new List<object>();

                                //Colonnes d'entête
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    val = reader.GetValue(i);

                                    // Cas spécifique des dates à formater en string
                                    if (val != null && result.Data.Columns[i].Type == SqlDbType.Date.ToString() && val.GetType() == typeof(DateTime))
                                        val = ((DateTime)val).ToString("dd/MM/yyyy");

                                    row.Add(val);
                                }

                                columnDefined = true;

                                //Ajoute la ligne;
                                result.Data.Rows.Add(row.ToArray());
                            }
                            result.Data.Count = result.Data.Rows.Count;
                        }
                    }
                    catch (SqlException e)
                    {
                        //Si erreur, on catch la requête SQL pour débugger...
                        throw new Exception(e.Message + "\r\n" + sql.Replace(Environment.NewLine, ""));
                    }
                }
            }

            //Pas de pagiantion
            if (string.IsNullOrEmpty(sqlCount) || (result.Data.Count < filters.PageSize && (filters.PageIndex == 1 || !filters.PageIndex.HasValue)))
            {
                result.Data.TotalRows = result.Data.Count;
            }
            else
            {
                using (var cnx = new SqlConnection(_options.ConnectionString))
                {
                    cnx.Open();
                    using (var manager = new SqlCommand(sqlCount, cnx))
                    {

                        //Ajout des params
                        foreach (SqlParameter p in _sqlWhereParams)
                        {
                            SqlParameter newParam = p;
                            manager.Parameters.Add(newParam);
                        }

                        //Requête
                        using (SqlDataReader reader = manager.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                result.Data.TotalRows = reader.GetInt32(reader.GetOrdinal("totalRows"));
                            }
                        }
                    }
                }
            }


            if (_options.IncludeQueryProfiling)
            {
                result.QueryDelay = (DateTime.Now - timestamp).TotalSeconds;
            }

            return result;
        }


        private void HandleBatchWhereFilters(IDictionary<Column, DataTable> filters)
        {

            if (filters == null)
                return;

            if (filters.Any())
            {
                StringBuilder where = new StringBuilder();
                int i = 0;
                foreach (var filter in filters)
                {
                    where.Append(" ( ");
                    where.Append(filter.Key.SqlName);
                    where.Append($" IN (SELECT Item FROM @{ filter.Key.Alias}{i}) ");
                    where.Append(" ) ");
                    //  filter.Value
                    AddParameter($"@{filter.Key.Alias}{i}", SqlDbType.Structured, filter.Value);
                    where.Append(" AND ");
                }
                where.Append(" 1=1 ");
                AppendWhere(where.ToString());
            }
        }


        private void HandleWhereFilters(IEnumerable<Filter> filtersOnDimension)
        {
            if (filtersOnDimension.Any())
            {
                StringBuilder where = new StringBuilder();
                where.Append(" ( ");
                int i = 0;
                foreach (Filter filter in filtersOnDimension)
                {
                    var dim = filter.Dimension;

                    where.Append(" ");
                    where.Append(filter.Prefix);
                    if (!filter.Disabled)
                    {

                        if (filter.Dimension.IsGeography)
                        {
                            where.Append(AppendGeographyQuery(filter));
                        }
                        else
                        {

                            if (filter.Type == OperatorType.Different && !string.IsNullOrEmpty(filter.Value))
                            {
                                where.Append(" ( ");
                                where.Append(dim.SqlName);
                                where.Append(" IS NULL OR ");
                            }


                            where.Append(" ");
                            where.Append(dim.SqlName);
                            where.Append(" ");
                            if (filter.Type == OperatorType.Equal && string.IsNullOrEmpty(filter.Value))
                            {
                                where.Append(" IS NULL ");
                            }
                            else if (filter.Type == OperatorType.Different && string.IsNullOrEmpty(filter.Value))
                            {
                                where.Append(" IS NOT NULL ");
                            }
                            else
                            {

                                where.Append(filter.Type.GetSqlLabel());
                                where.Append(" @" + dim.Alias + i);
                                AddParameter("@" + dim.Alias + i, dim.SqlTypeAuto, filter.Value);
                            }

                            if (filter.Type == OperatorType.Different && !string.IsNullOrEmpty(filter.Value))
                            {
                                where.Append(" ) ");
                            }
                        }

                    }
                    else
                    {
                        where.Append(" 1=1 ");
                    }


                    where.Append(" ");
                    where.Append(filter.Suffix);
                    where.Append(" ");

                    // Apply filter at last
                    if (filter != filtersOnDimension.Last())
                    {
                        where.Append(" ");
                        where.Append(filter.Separator.GetSqlLabel());
                        where.Append(" ");
                    }

                    where.AppendLine();
                    i++;
                }
                where.Append(" ) ");
                AppendWhere(where.ToString());
            }
        }

        private void HandleFullTextQueries(DataQueryFilter filters)
        {
            if (filters.IsFullTextQuery)
            {
                Table fullTextQueryTable = filters.Tables.FirstOrDefault(m => m.Value.SupportFreeText).Value;

                if (fullTextQueryTable == null)
                    throw new Exception("Full text query is not supported");

                //Test if some requested dimensions are in fulltext index 
                if (!fullTextQueryTable.Columns.Any(m => !m.IsMetric && filters.Dimensions.Select(s => s.Alias).Contains(m.Alias)))
                    throw new Exception(string.Format("This dimension does not belong to {0} which contains the fulltext index", fullTextQueryTable.Name));

                string fullTextSearchColumns = string.Format("{0}.*", fullTextQueryTable.Alias);

                if (filters.FullTextQueryConstraints.Any())
                    fullTextSearchColumns = string.Join(",", filters.FullTextQueryConstraints.Select(m => m.SqlName));

                AppendWhere(string.Format("CONTAINS({0},@fullTextSearch)", fullTextSearchColumns));
                AddParameter("@fullTextSearch", SqlDbType.NVarChar, FormatFullTextQuery(filters.FullTextQuery));
            }
        }

        private void AppendHavingStatements(DataQueryFilter filters, StringBuilder sqlQuery)
        {
            IEnumerable<Filter> filtersOnMetrics = filters.Filters.Where(m => m.Dimension.IsMetric);
            if (filtersOnMetrics.Any())
            {
                sqlQuery.Append(" HAVING ");
                int l = 0;
                int nbFilterOnMetrics = filtersOnMetrics.Count();
                foreach (Filter filter in filtersOnMetrics)
                {
                    var dim = filter.Dimension;
                    sqlQuery.Append(" ");
                    sqlQuery.Append(dim.SqlName);
                    sqlQuery.Append(" ");
                    sqlQuery.Append(filter.Type.GetSqlLabel());
                    sqlQuery.Append(" @" + dim.Alias + l);
                    AddParameter("@" + dim.Alias + l, dim.SqlTypeAuto, filter.Value);
                    if (nbFilterOnMetrics < (l + 1))
                    {
                        sqlQuery.Append(" ");
                        sqlQuery.Append(filter.Separator.GetSqlLabel());
                    }
                    sqlQuery.Append(" ");
                    sqlQuery.AppendLine();
                    l++;
                }
            }
        }

        private string FormatFullTextQuery(string fullTextQuery)
        {
            string criteres = string.Join("\" AND \"", fullTextQuery.Trim().Split(new char[] { ' ', '-' })); // on commence par splitter les critères de recherche, au cas où il y aurait plusieurs mots

            string textIntegralSearch = string.Format("(\"{0}*\") OR \"{1}\"", criteres, fullTextQuery);

            return textIntegralSearch;
        }

        /// <summary>
        /// Build a date query
        /// </summary>
        /*
        private void AppendDateQuery(DataQueryFilter filters)
        {
            if (!filters.DateDebut.HasValue && !filters.DateFin.HasValue)
                return;

            IList<Column> dateProps = null;

            //Cas ou on fait ça en automatique
            if (filters.ForcedDateFilter.Any())
                dateProps = filters.ForcedDateFilter;
            else
                dateProps = filters.Tables.Values.SelectMany(m => m.Columns).Where(m => m.UsedToFilterDate).ToList();

            int i = 1;
            if (dateProps.Any())
            {
                foreach (Column dim in dateProps)
                {
                    AppendWhere(dim.ColumnName + string.Format(" BETWEEN @dq_dateDeb{0} AND @dq_dateFin{0}  ", i));

                    if (filters.DateFin.Value.TimeOfDay.TotalSeconds == 0)
                        filters.DateFin = filters.DateFin.Value.AddDays(1).AddSeconds(-1);

                    AddParameter("@dq_dateDeb" + i, SqlDbType.DateTime, filters.DateDebut);
                    AddParameter("@dq_dateFin" + i, SqlDbType.DateTime, filters.DateFin);
                    i++;
                }
            }
        }*/


        private void AppendInclusion(List<Inclusion> inclusions)
        {
            if (!inclusions.Any())
                return;

            int i = 0;
            foreach (IGrouping<TypeInclusion, Inclusion> group in inclusions.GroupBy(m => m.Type))
            {
                string sqlLines = " ( ";
                foreach (var inclusion in group)
                {
                    string paramName = "@" + inclusion.KeyTable.Alias + i;
                    string inOrOutStr = (inclusion.Type == TypeInclusion.Out ? "NOT IN" : "IN");

                    sqlLines += string.Format("{0} {1} (SELECT {2} FROM {3} {4} WHERE {5}={6} ) ",
                      inclusion.LinkedPropertyColumnName,
                      inOrOutStr,
                      inclusion.KeyTable.SqlName,
                      inclusion.Table.Name,
                      inclusion.Table.Alias,
                      inclusion.Prop.SqlName,
                      paramName
                    );

                    if (inclusion != group.Last())
                        sqlLines += " OR ";

                    AddParameter(paramName, inclusion.Prop.SqlTypeAuto, inclusion.Value);
                    i++;
                };
                sqlLines += " ) ";

                if (group.Key == TypeInclusion.Add)
                {
                    AppendWhere(sqlLines, "OR");
                }
                else
                {
                    AppendWhere(sqlLines, "AND");
                }
            }
        }

        /// <summary>
        /// Add orthodromy filtering
        /// Ex : Societe_Position==2.065638 49.465552|6000
        /// </summary>
        private string AppendGeographyQuery(Filter filter)
        {

            string output = " ( ";
            string param = null;
            var arrVal = filter.Value.Split('|');

            if (arrVal.Count() != 2)
                throw new Exception("Error on dimension " + filter.Dimension.SqlName + ", the format must be {latInDeg} {lngInDeg}|{distanceInMeter}. Ex : 2.065638 49.465552|6000");

            try
            {
                var latLng = arrVal[0].Split(' ');
                double lat = double.Parse(latLng[0]);
                double lng = double.Parse(latLng[1]);
                double distance = double.Parse(arrVal[1]);
                param = $"geography::STGeomFromText('POINT({lat} {lng})', 4326)";
                output += (string.Format("{0}.STDistance({1}) IS NOT NULL", filter.Dimension.SqlName, param));
                output += " AND ";
                output += (string.Format("{0}.STDistance({1}) < {2} ", filter.Dimension.SqlName, param, distance));
            }
            catch
            {
                throw new Exception("Erreur sur la dimension " + filter.Dimension.SqlName + ", le format doit être {latInDeg} {lngInDeg}|{distanceInMeter}. Ex : 2.065638 49.465552|6000");
            }

            output += " ) ";

            return output;
        }


        /// <summary>
        /// Where statement handling
        /// </summary>
        bool firstWhereStatement = true;
        private DataQueryOptions _options;

        private void AppendWhere(string input, string sep = "AND")
        {
            if (firstWhereStatement)
            {
                _whereStatement.Append(" ");
                _whereStatement.Append(" WHERE ");
                _whereStatement.Append(" ");
            }
            else
            {
                _whereStatement.Append(" ");
                _whereStatement.Append(sep);
                _whereStatement.Append(" ");
            }
            _whereStatement.Append(" ");
            _whereStatement.Append(input);
            _whereStatement.Append(" ");
            firstWhereStatement = false;
        }

        /// <summary>
        /// Remove select, orderBy and groupBy alias
        /// </summary>
        private void RemoveDuplicates()
        {
            _sqlSelect = _sqlSelect.Distinct().ToList();
            _sqlGroupBy = _sqlGroupBy.Distinct().ToList();
            _sqlOrderBy = _sqlOrderBy.Distinct().ToList();
        }


        /// <summary>
        /// Add a single parameter to stored procedure
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="sqlDbType"></param>
        /// <param name="value"></param>
        private void AddParameter(string paramName, SqlDbType sqlDbType, object value)
        {
            SqlParameter p = new SqlParameter(paramName, sqlDbType);

            if (sqlDbType == SqlDbType.UniqueIdentifier)
                value = new Guid(value.ToString());

            if (value.GetType() == typeof(DateTime) && sqlDbType == SqlDbType.DateTime)
            {
                var date = new DateTime();
                if (!DateTime.TryParse(value.ToString(), out date))
                    date = DateTime.ParseExact(value.ToString(), _options.InputDateFormat, CultureInfo.CurrentUICulture);

                value = date;
            }

            p.Value = value;

            if (sqlDbType == SqlDbType.Structured)
                p.TypeName = "dbo.NVarcharList";

            _sqlWhereParams.Add(p);
        }
    }

}

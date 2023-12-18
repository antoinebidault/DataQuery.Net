using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataQuery.Net
{
  /// <summary>
  /// Filtre where en base de données
  /// </summary>
  public class Filter
  {
    public Filter(string filter, DataQuerySchema config)
    {
      IsLazy = false;

      //Séparateur de filtre
      this.Separator = ConditionSeparator.None;
      if (filter.EndsWith(";"))
        this.Separator = ConditionSeparator.And;
      else if (filter.EndsWith(","))
        this.Separator = ConditionSeparator.Or;

      //Regarde si l'operrateur est défini
      bool operatorDefined = true;
      foreach (OperatorType operationType in (Enum.GetValues(typeof(OperatorType))))
      {
        if (filter.IndexOf(operationType.GetQueryLabel()) > -1)
        {
          operatorDefined = true;
          this.Type = operationType;
        }
      }

      if (!operatorDefined)
        throw new Exception("Incorrectly formatted filter : " + filter);

      string cleanedFilter = filter.TrimEnd(';').TrimEnd(',');

      string pattern = @"(^\(*)[^\)]+(\)*$)";
      var res = Regex.Match(cleanedFilter, pattern);
      if (res.Success)
      {
        this.Prefix = res.Groups[1].Value;
        this.Suffix = res.Groups[2].Value;
        cleanedFilter = cleanedFilter.TrimStart('(').TrimEnd(')');
      }

      var filterArray = cleanedFilter.Split(new string[] { this.Type.GetQueryLabel() }, StringSplitOptions.None);

      //Définition de la dimension
      string dimOrMetricName = filterArray.First();

      //Définition de la valeur
      this.Value = filterArray.Last();


      //Gestion des métriques lazy avec la double crochets
      if (dimOrMetricName.StartsWith("[") && dimOrMetricName.EndsWith("]"))
      {
        IsLazy = true;
        dimOrMetricName = dimOrMetricName.TrimEnd(']').TrimStart('[');
      }

      if (config.MetricsAndDimensions.ContainsKey(dimOrMetricName))
      {
        if (!config.MetricsAndDimensions[dimOrMetricName].AllowedToFilter)
          throw new Exception(string.Format("You don't have the right to access this dimension : {0} ! ", dimOrMetricName));

        this.Dimension = config.MetricsAndDimensions[dimOrMetricName];
      }
      else
      {
        var message = string.Format("This dimension does not exist : '{0}'", filter);
        throw new Exception(message);
      }


    }

    /// <summary>
    /// For managing "("
    /// </summary>
    public string Suffix { get; set; }

    /// <summary>
    /// Permet de gérer les parenthèses
    /// </summary>
    public string Prefix { get; set; }

    /// <summary>
    /// Ex: eventId
    /// </summary>
    public Dimension Dimension { get; set; }

    /// <summary>
    /// Si lazy, on affiche le filtre que si la table dépendante est utilisée.
    /// </summary>
    public bool IsLazy { get; set; }

    /// <summary>
    /// Si lazy, on affiche le filtre que si la table dépendante est utilisée.
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Ex: ==, >=, ...
    /// </summary>
    public OperatorType Type { get; set; }

    /// <summary>
    /// Type Sql
    /// </summary>
    public SqlDbType SqlType { get; set; }

    /// <summary>
    /// Valeur de l'expression
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Or, And...
    /// </summary>
    public ConditionSeparator Separator { get; set; }
  }

}

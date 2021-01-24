using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQuery.Net
{
  public class DatabaseProp
  {
    public DatabaseProp()
    {
      this.SqlType = SqlDbType.NVarChar;
      this.Displayed = true;
      this.SqlJoin = new Dictionary<string, string>();
      this.AllowedToRequest = true;
      this.AllowedToExport = true;
      this.AllowedToReport = false;
    }


    /// <summary>
    /// Nom de la colonne en SQL
    /// </summary>
    public string Column { get; set; }

    /// <summary>
    /// Type SQL
    /// </summary>
    public SqlDbType SqlType { get; set; }

    /// <summary>
    /// Alias à donner à la colonne pour garantir l'unicité du dataquery (Doit être unique dans la config, sinon ça plante)
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    /// Label user friendly de la colonne pour affichages diverses...
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// Description de la colonne pour faciliter la compréhension
    /// </summary>
    public string Description { get; set; }


    /// <summary>
    /// Nom du groupe auquel appartient la métrique
    /// </summary>
    public string Group { get; set; }

    /// <summary>
    /// Vrai si c'est une métrique, sinon ce sera une dimension
    /// </summary>
    public bool IsMetric { get; set; }

    /// <summary>
    /// Si type géography (Un peu spécifique), le traitement des données sera un peu différent
    /// </summary>
    public bool IsGeography { get; set; }

   /// <summary>
   /// Unité de la valeur (Si c'est une métrique notamment)
   /// </summary>
    public string Unit { get; set; }

    /// <summary>
    /// Couleur de la donnée
    /// </summary>
    public string Color { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool Displayed { get; set; }

    /// <summary>
    /// Autorisé pour le requêtage (Faire des where)
    /// </summary>
    public bool AllowedToRequest { get; set; }

    /// <summary>
    /// Autorisé pour le reporting => Export dans un select
    /// </summary>
    public bool AllowedToExport { get; set; }

    /// <summary>
    /// Autorisé dans des croisements de rapports (Un peu la propriété fourre-tout)
    /// </summary>
    public bool AllowedToReport { get; set; }

    /// <summary>
    /// Si la propriété doit être utilisé pour filtrer sur le champs end et start passé en param.
    /// </summary>
    public bool UsedToFilterDate { get; set; }

    /// <summary>
    /// Jointures SQL à appliquer sur la propriété => "Nom de la table" > "Nom de la clé associée"
    /// </summary>
    public Dictionary<string, string> SqlJoin { get;  set; }
  }
}
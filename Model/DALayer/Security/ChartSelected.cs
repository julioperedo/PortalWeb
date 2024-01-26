using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using BE = BEntities;
using BEB = BEntities.Base;
using BEP = BEntities.Product;
using BEA = BEntities.SAP;
using BES = BEntities.Security;

using DALayer.Base;
using DALayer.Product;

using System.Data.Common;
using System.Text;
using System.Data.SqlClient;
using Dapper;
using BEntities.Filters;

namespace DALayer.Security {

    [Serializable()]
    public class ChartSelected : DALEntity<BES.ChartSelected> {

        #region Methods
       protected override void LoadRelations(ref BES.ChartSelected Item, params Enum[] Relations)
        {
            //throw new NotImplementedException();
        }

        protected override void LoadRelations(ref IEnumerable<BES.ChartSelected> Items, params Enum[] Relations)
        {
            //throw new NotImplementedException();
        }

        #endregion

        #region List Methods

        public List<BES.ChartSelected> List(long IdProfile) {
            StringBuilder sb = new();
            sb.AppendLine("SELECT   c.IdcChartType AS IdChartType, pc.IdChart, cl.Name AS ChartTypeName, c.Name AS ChartName ");
            sb.AppendLine("FROM     Security.ProfileChart pc ");
            sb.AppendLine("         INNER JOIN Base.Chart c ON pc.IdChart = c.Id ");
            sb.AppendLine("         INNER JOIN Base.Classifier cl ON c.IdcChartType = cl.Id ");
            sb.AppendLine("WHERE    pc.IdProfile = @IdProfile ");
            sb.AppendLine("ORDER BY 1, 2 ");

            List<BES.ChartSelected> Collection = SQLList(sb.ToString(), new { IdProfile }).AsList();
            return Collection;
        }

        public List<BES.ChartSelected> List(List<Field> FilterList, string Order) {
            var (filter, parameters) = GetFilter(FilterList.ToArray());            
            StringBuilder sb = new();
            sb.AppendLine("SELECT   c.IdcChartType AS IdChartType, pc.IdChart, cl.Name AS ChartTypeName, c.Name AS ChartName ");
            sb.AppendLine("FROM     Security.ProfileChart pc ");
            sb.AppendLine("         INNER JOIN Base.Chart c ON pc.IdChart = c.Id ");
            sb.AppendLine("         INNER JOIN Base.Classifier cl ON c.IdcChartType = cl.Id ");
            if(filter!="") sb.AppendLine($"WHERE    {filter} ");
            sb.AppendLine("ORDER BY " + Order);

            List<BES.ChartSelected> Collection = SQLList(sb.ToString(), parameters).AsList();
            return Collection;
        }

        #endregion

        #region Search Methods

        #endregion

        #region Constructors

        public ChartSelected() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public ChartSelected(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal ChartSelected(SqlConnection connection) : base(connection) { }

        #endregion

    }

}
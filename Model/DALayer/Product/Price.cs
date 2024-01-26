using DALayer.Base;

using DALayer.Security;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using BE = BEntities;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEP = BEntities.Product;
using BES = BEntities.Security;

namespace DALayer.Product
{
    public partial class Price
    {

        #region Save Methods 

        public void Save2(ref BEP.Price Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Product].[Price](IdProduct, IdSudsidiary, Regular, ClientSuggested, Observations, Commentaries, LogUser, LogDate) VALUES(@IdProduct, @IdSudsidiary, @Regular, @ClientSuggested, @Observations, @Commentaries, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Product].[Price] SET IdProduct = @IdProduct, IdSudsidiary = @IdSudsidiary, Regular = @Regular, ClientSuggested = @ClientSuggested, Observations = @Observations, Commentaries = @Commentaries, LogUser = @LogUser, LogDate = @LogDate WHERE Id = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = $"DECLARE @USERID BINARY(128);SET @USERID = CAST({Item.LogUser} AS BINARY(128));SET CONTEXT_INFO @USERID; DELETE FROM [Product].[Price] WHERE Id = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
                if (Item.StatusType == BE.StatusType.Insert)
                {
                    Item.Id = Convert.ToInt64(Connection.ExecuteScalar(strQuery, Item));
                }
                else
                {
                    Connection.Execute(strQuery, Item);
                }
                Item.StatusType = BE.StatusType.NoAction;
            }
        }

        public async Task SaveAsync(BEP.Price Item)
        {
            string query = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                query = "INSERT INTO [Product].[Price]([IdProduct], [IdSudsidiary], [Regular], [ClientSuggested], [Observations], [Commentaries], [LogUser], [LogDate]) VALUES(@IdProduct, @IdSudsidiary, @Regular, @ClientSuggested, @Observations, @Commentaries, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                query = "UPDATE [Product].[Price] SET [IdProduct] = @IdProduct, [IdSudsidiary] = @IdSudsidiary, [Regular] = @Regular, [ClientSuggested] = @ClientSuggested, [Observations] = @Observations, [Commentaries] = @Commentaries, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                query = "DELETE FROM [Product].[Price] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
                if (Item.StatusType == BE.StatusType.Insert)
                    Item.Id = Convert.ToInt64(await Connection.ExecuteScalarAsync(query, Item));
                else
                    await Connection.ExecuteAsync(query, Item);
            }
        }

        #endregion

        #region Methods 

        #endregion

        #region List Methods 

        //public List<BEP.Price> List(long IdLine, string Order, params Enum[] Relations)
        //{
        //    StringBuilder sb = new();
        //    sb.AppendLine("SELECT   p.* ");
        //    sb.AppendLine("FROM     Product.Price p ");
        //    sb.AppendLine("         INNER JOIN Product.Product r ON p.IdProduct = r.Id ");
        //    sb.AppendLine("WHERE    r.IdLine = @IdLine ");

        //    List<BEP.Price> Collection = Connection.Query<BEP.Price>(sb.ToString(), new { @IdLine = IdLine }).AsList();
        //    if (Collection.Count > 0) LoadRelations(ref Collection, Relations);
        //    return Collection;
        //}

        public List<BEP.Price> List2(long IdLine, string SortingBy, params Enum[] Relations)
        {
            string query = $@"SELECT	p.*
							  FROM		Product.Price p
										INNER JOIN Product.Product p1 ON p.IdProduct = p1.Id
							  WHERE		( p.Regular > 0 OR p1.ShowAlways = 1 OR ISNULL(p.Observations, '' ) <> '' ) 			
										AND EXISTS ( SELECT * FROM Product.LineDetail ld WHERE ld.IdLine = {IdLine} AND ISNULL(p1.Line, '') = ld.SAPLine ) 
							  ORDER BY {SortingBy} ";
            List<BEP.Price> items = SQLList(query, Relations).AsList();
            return items;
        }

        public IEnumerable<BEP.Price> ListByCode(string ItemCodes, string SortingBy, params Enum[] Relations)
        {
            string query = $@"SELECT	p.*
							  FROM		Product.Price p
										INNER JOIN Product.Product p1 ON p.IdProduct = p1.Id
							  WHERE		p.IdSudsidiary = 1 AND LOWER(p1.ItemCode) IN ( {ItemCodes.ToLower()} ) 
							  ORDER BY {SortingBy} ";
            IEnumerable<BEP.Price> items = SQLList(query, Relations);
            return items;
        }

        public IEnumerable<BEP.Price> ListBySubsidiary(long IdSubsidiary, params Enum[] Relations)
        {
            string query = $@"SELECT  *
							  FROM    Product.Price p
									  INNER JOIN Product.Product p1 ON p.IdProduct = p1.Id
							  WHERE   p1.Enabled = 1 AND p1.ShowTeamOnly = 0 AND p.Regular > 0 AND p.IdSudsidiary = {IdSubsidiary} ";
            IEnumerable<BEP.Price> items = SQLList(query, Relations);
            return items;
        }

        #endregion

        #region Search Methods 

        public BEP.Price Search(string ItemCode, long IdSubsidiary, params Enum[] Relations)
        {
            string query = $@"SELECT  p.*
							  FROM    Product.Price p
									  INNER JOIN Product.Product p1 ON p.IdProduct = p1.Id
							  WHERE   p1.ItemCode = '{ItemCode}' AND p.IdSudsidiary = {IdSubsidiary} ";
            BEP.Price item = SQLSearch(query, Relations);
            return item;
        }

        #endregion

    }
}
using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BE = BEntities;
using BEP = BEntities.Product;

namespace DALayer.Product
{
    public partial class Document
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public IEnumerable<BEP.Document> ListExtended(List<Field> FilterList, string SortingBy, params Enum[] Relations)
        {
            string filter = FilterList?.Count > 0 ? GetFilterString(FilterList.ToArray()) : "1 = 1", query;
            query = $@"SELECT   *
                       FROM	    ( SELECT  d.*, ld.IdLine, c.Name AS TypeName, p.ItemCode + ' - ' + p.Name AS ProductName
                                          , ( SELECT COUNT(*) FROM Product.DocumentFile df WHERE d.Id = df.IdDocument ) AS FilesCount
		                          FROM	  Product.Document d
					                      INNER JOIN Base.Classifier c ON d.TypeIdc = c.Id
					                      INNER JOIN Product.Product p ON d.IdProduct = p.Id
					                      INNER JOIN Product.LineDetail ld ON LOWER(p.Line) = LOWER(ld.SAPLine) ) a 
                       WHERE    {filter} 
                       ORDER BY {SortingBy} ";
            IEnumerable<BEP.Document> Items = SQLList(query, Relations);
            return Items;
        }
 
        #endregion

        #region Search Methods

        #endregion

    }
}
using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using BEA = BEntities.SAP;
using DALH = DALayer.SAP.Hana;

namespace BComponents.SAP
{
    [Serializable()]
    public class OrderFile : BCEntity
    {
        #region Search Methods

        public BEA.OrderFile Search(string Subsidiary, int DocEntry, string FileName, params Enum[] Relations)
        {
            BEA.OrderFile BEObject = null;
            try
            {
                using DALH.OrderFile DALObject = new DALH.OrderFile();
                BEObject = DALObject.Search(Subsidiary, DocEntry, FileName, Relations);
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
            return BEObject;
        }

        #endregion

        #region List Methods

        public IEnumerable<BEA.OrderFile> List(IEnumerable<string> DocEntries, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.OrderFile> BECollection = default;
                using (DALH.OrderFile DALObject = new DALH.OrderFile())
                {
                    BECollection = DALObject.List(DocEntries, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        public IEnumerable<BEA.OrderFile> ListByDeliveries(IEnumerable<string> DocEntries, params Enum[] Relations)
        {
            try
            {
                IEnumerable<BEA.OrderFile> BECollection = default;
                using (DALH.OrderFile DALObject = new DALH.OrderFile())
                {
                    BECollection = DALObject.ListByDeliveries(DocEntries, Relations);
                }
                return BECollection;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
                return null;
            }
        }

        #endregion

        #region Constructors

        public OrderFile() : base() { }

        #endregion
    }
}

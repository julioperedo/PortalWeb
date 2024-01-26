using BEntities.Filters;
using BEntities.PostSale;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BE = BEntities;
using BEA = BEntities.SAP;
using BET = BEntities.PostSale;

namespace DALayer.SAP.Hana
{
    [Serializable()]
    public class RMA : DALEntity<BET.ServiceCall>
    {
        #region Methods

        protected override void LoadRelations(ref ServiceCall Item, params Enum[] Relations) { }

        protected override void LoadRelations(ref IEnumerable<ServiceCall> Items, params Enum[] Relations) { }

        #endregion

        #region List Methods

        public IEnumerable<BET.ServiceCall> List(List<Field> Filters, string Order)
        {
            string filter = Filters?.Count > 0 ? $"WHERE    {GetFilter(Filters?.ToArray())}" : "";
            string query = $@"SELECT  *
                              FROM    ( SELECT  DISTINCT CAST(T0.""callID"" AS VARCHAR(10)) AS ""Code"", T0.""technician"" AS ""TechnicianCode"", (CASE WHEN T0.""technician"" IS NULL THEN '' ELSE T3.""firstName"" || ' ' || IFNULL(T3.""middleName"",'') || ' ' || T3.""lastName"" END) AS ""Technician""
                                                , t0.""assignee"" AS ""AssigneeCode"", t12.U_NAME AS ""Assignee""
                                                , CAST(TO_VARCHAR(t0.""createDate"", 'yyyy-MM-dd') || ' ' || SUBSTRING(LPAD(TO_VARCHAR(t0.""createTime""), 4, '0'), 1, 2) || ':' || SUBSTRING(LPAD(TO_VARCHAR(t0.""createTime""), 4, '0'), 3, 2) AS TIMESTAMP) AS ""CreateDate""
                                                , CAST(TO_VARCHAR(t0.""StartDate"", 'yyyy-MM-dd') || ' ' || SUBSTRING(LPAD(TO_VARCHAR(t0.""StartTime""), 4, '0'), 1, 2) || ':' || SUBSTRING(LPAD(TO_VARCHAR(t0.""StartTime""), 4, '0'), 3, 2) AS TIMESTAMP) AS ""StartDate""
                                                , CAST(TO_VARCHAR(t0.""U_FECHING"", 'yyyy-MM-dd') || ' ' || SUBSTRING(LPAD(TO_VARCHAR(t0.""U_HORA""), 4, '0'), 1, 2) || ':' || SUBSTRING(LPAD(TO_VARCHAR(t0.""U_HORA""), 4, '0'), 3, 2) AS TIMESTAMP) AS ""AdmissionDate""
                                                , CAST(TO_VARCHAR(t0.""closeDate"", 'yyyy-MM-dd') || ' ' || SUBSTRING(LPAD(TO_VARCHAR(t0.""closeTime""), 4, '0'), 1, 2) || ':' || SUBSTRING(LPAD(TO_VARCHAR(t0.""closeTime""), 4, '0'), 3, 2) AS TIMESTAMP) AS ""CloseDate""
                                                , DAYS_BETWEEN(IFNULL(T0.U_FECHING, T0.""createDate""), IFNULL(T0.""closeDate"", NOW())) AS ""OpenDays"", T0.""customer"" AS ""ClientCode"", T0.""custmrName"" AS ""ClientName"", T0.U_CIUDAD AS ""CityCode"", T6.""Name"" AS ""City"", T0.U_NOMUSFIN AS ""FinalUser""
                                                , CAST(TO_VARCHAR(t0.U_FECHEN, 'yyyy-MM-dd') || ' ' || SUBSTRING(LPAD(TO_VARCHAR(t0.U_HORAENT), 4, '0'), 1, 2) || ':' || SUBSTRING(LPAD(TO_VARCHAR(t0.U_HORAENT), 4, '0'), 3, 2) AS TIMESTAMP) AS ""DeliveredDate""
                                                , t0.U_ENTPOR AS ""DeliveredBy"", t0.U_SERV_EXTERNO AS ""ExternalService"", t0.U_TECNICO_SERV_EXTERNO AS ""ExternalServiceTechnician"", t0.U_NRO_BOLETA_EXTERNA AS ""ExternalServiceNumber""
                                                , t0.U_DIRECCION_SERV_EXTERNO AS ""ExternalServiceAddress"", t0.U_NOGUIA AS ""GuideNumber"", t0.U_TRANSP AS ""Transport""
                                                , T1.U_MARCA AS ""Brand"", t8.U_LINEA AS ""Line"", T0.""itemCode"" AS ""ItemCode"", T0.""itemName"" AS ""ItemName"", T0.U_ENGRTA AS ""Warranty"", T0.""manufSN"" AS ""SerialNumber"", T0.""status"" AS ""StatusCode"", T2.""Name"" AS ""Status"", T0.""subject"" AS ""Subject""
                                                , CAST(T0.U_PIEZASCON AS INT) AS ""CountedPieces"", CAST(T0.U_PIEZASCON_ANTERIOR AS INT) AS ""PriorCountedPieces"", CAST(T0.U_PIEZASCONT_DIF AS INT) AS ""DiffCountedPieces"", CAST(T0.U_FECHCOM AS DATE) AS ""PurchaseDate"", T0.U_REPFALL AS ""ReportedBy""
                                                , CAST(IFNULL(( SELECT FIRST_VALUE(TA.""DocDate"" ORDER BY TA.""DocNum"" DESC)
                                                                FROM   {DBSA}.OINV TA 
                                                                        LEFT OUTER JOIN {DBSA}.OINS TB ON TA.""DocNum"" = TB.""invoiceNum""
                                                                WHERE  TB.""manufSN"" = T0.""manufSN"" AND TB.""itemCode"" = T0.""itemCode"" ),                   
                                                                (SELECT FIRST_VALUE(TA.""DocDate"" ORDER BY TA.""DocNum"" DESC) 
                                                                FROM   {DBSA}.ODLN TA 
                                                                        LEFT OUTER JOIN {DBSA}.OINS TB ON TA.""DocNum"" = TB.""deliveryNo"" 
                                                                WHERE  TB.""manufSN"" = T0.""manufSN"" AND TB.""itemCode"" = T0.""itemCode"" )) AS DATE) AS ""ClientPurchaseDate"", T0.U_AUX1 AS ""ReceivedBy"", t0.U_MAILUSFIN AS ""FinalUserEMail"", T0.U_TELUSFIN AS ""FinalUserPhone"", T0.U_PRUEBCO AS ""RefNV"", T0.U_COMENT AS ""Comments""
                                                , T5.U_GPRODUCT AS ""ProductManager"", t7.""FilePath"", t7.""FileName"", t0.U_LUGAR AS ""ServiceTypeCode"", ( CASE t0.U_LUGAR WHEN 1 THEN 'On Site' WHEN 2 THEN 'Bench' ELSE '' END ) AS ""ServiceType""
                                                , CAST(t0.""origin"" AS INT) AS ""OriginCode"", t13.""Name"" AS ""Origin"", t0.""Street"", t0.""City"" AS ""City2"", t0.""Room"", CAST(t0.""Location"" AS INT) AS ""LocationCode"", t9.""Name"" AS ""Location"", t0.""Country"" AS ""CountryCode"", t10.""Name"" AS ""Country""
                                                , t0.""State"" AS ""StateCode"", t11.""Name"" AS ""State"", CAST(t0.""resolution"" AS VARCHAR(5000)) AS ""Resolution"", t0.""priority"" AS ""Prioriry"", t0.""callType"" AS ""CallType"", CAST(t0.""problemTyp"" AS INT) AS ""ProblemTypeCode"", t14.""Name"" AS ""ProblemType""
                                        FROM    {DBSA}.OSCL T0
                                                LEFT OUTER JOIN {DBSA}.OINS T1 ON T0.""insID"" = T1.""insID""
                                                LEFT OUTER JOIN {DBSA}.OSCS T2 ON T0.""status"" = T2.""statusID""
                                                LEFT OUTER JOIN {DBSA}.OHEM T3 ON T0.""technician"" = T3.""empID""
                                                LEFT OUTER JOIN {DBSA}.OCRD T4 ON T0.""customer"" = T4.""CardCode""
                                                LEFT OUTER JOIN {DBSA}.OITM T5 ON T0.""itemCode"" = T5.""ItemCode""
                                                LEFT OUTER JOIN {DBSA}.""@CIUDAD"" T6 ON T6.""Code"" = T0.""U_CIUDAD""
                                                LEFT OUTER JOIN ( SELECT ""AbsEntry"", FIRST_VALUE(CAST(""trgtPath"" AS VARCHAR(5000)) ORDER BY ""Line"") AS ""FilePath"", STRING_AGG(CAST(""FileName"" AS VARCHAR(5000)) || '.' || CAST(""FileExt"" AS VARCHAR(5000)), '; ') AS ""FileName""
                                                                FROM   {DBSA}.ATC1 s0
                                                                GROUP BY ""AbsEntry"" ) t7 ON t0.""AtcEntry"" = t7.""AbsEntry""
                                                LEFT OUTER JOIN {DBSA}.OITM t8 ON t1.""itemCode"" = t8.""ItemCode""
                                                LEFT OUTER JOIN {DBSA}.OCLO t9 ON t0.""Location"" = t9.""Code""
                                                LEFT OUTER JOIN {DBSA}.OCRY t10 ON t0.""Country"" = t10.""Code""
                                                LEFT OUTER JOIN {DBSA}.OCST t11 ON t0.""State"" = t11.""Code"" AND t0.""Country"" = t11.""Country""
                                                LEFT OUTER JOIN {DBSA}.OUSR t12 ON t0.""assignee"" = t12.USERID
                                                LEFT OUTER JOIN {DBSA}.OSCO t13 ON t0.""origin"" = t13.""originID"" 
                                                LEFT OUTER JOIN {DBSA}.OSCP t14 ON t0.""problemTyp"" = t14.""prblmTypID"" ) a 
                              { filter} 
                              ORDER BY {GetOrder(Order)} ";
            IEnumerable<BET.ServiceCall> items = SQLList(query);
            return items;
        }

        public IEnumerable<BEA.Item> ListStatuses(List<Field> Filters, string Order)
        {
            string filter = Filters?.Count > 0 ? $@"{GetFilter(Filters?.ToArray())}" : "1 = 1";
            string query = $@"SELECT  t0.""statusID"" AS ""Id"", t0.""Name""
                              FROM    {DBSA}.OSCS t0                                      
                              WHERE   {filter}
                              ORDER By {GetOrder(Order)} ";
            IEnumerable<BEA.Item> items = SQLList<BEA.Item>(query);
            return items;
        }

        public IEnumerable<BEA.Item> ListTechnicians(List<Field> Filters, string Order)
        {
            string filter = Filters?.Count > 0 ? $"AND {GetFilter(Filters?.ToArray())}" : "";
            string query = $@"SELECT  t0.""empID"" AS ""Id"", ""firstName"" || IFNULL(' ' || ""middleName"" || ' ', ' ') || ""lastName"" AS ""Name""
                              FROM    {DBSA}.OHEM t0
                                      INNER JOIN {DBSA}.HEM6 t1 ON t0.""empID"" = t1.""empID""
                              WHERE   EXISTS ( SELECT * FROM {DBSA}.OSCL WHERE ""technician"" = T0.""empID"" ) AND t1.""roleID"" = -2 AND t0.""Active"" = 'Y' {filter} 
                              ORDER BY {GetOrder(Order)} ";

            IEnumerable<BEA.Item> items = SQLList<BEA.Item>(query);
            return items;
        }

        public IEnumerable<BEA.Item> ListSubjects()
        {
            string query = $@"SELECT ""Code"" AS ""Id"", ""Name"" FROM {DBSA}.OCLS ";
            IEnumerable<BEA.Item> items = SQLList<BEA.Item>(query);
            return items;
        }

        public IEnumerable<BEA.RMAHistory> ListHistory(int Id, string Order)
        {
            string query = $@"SELECT  T1.""logInstanc"" AS ""Order"", CAST(TO_VARCHAR(T1.""createDate"", 'yyyy-MM-dd') || ' ' || SUBSTRING(LPAD(TO_VARCHAR(T1.""createTime""), 4, '0'), 1, 2) || ':' || SUBSTRING(LPAD(TO_VARCHAR(T1.""createTime""), 4, '0'), 3, 2) AS TIMESTAMP) AS ""CreateDate""
                                      , CAST(TO_VARCHAR(T1.""updateDate"", 'yyyy-MM-dd') || ' ' || SUBSTRING(LPAD(TO_VARCHAR(T1.""UpdateTime""), 4, '0'), 1, 2) || ':' || SUBSTRING(LPAD(TO_VARCHAR(T1.""UpdateTime""), 4, '0'), 3, 2) AS TIMESTAMP) AS ""UpdateDate""
                                      , T2.""Name"" AS ""Status"", T1.""subject"" AS ""Subject""
                              FROM    {DBSA}.OSCL T0 
                                      INNER JOIN {DBSA}.ASCL T1 ON T0.""callID"" = T1.""callID""
                                      INNER JOIN {DBSA}.OSCS T2 ON T2.""statusID"" = T1.""status"" 
                              WHERE   T0.""callID"" = {Id}
                              ORDER BY {GetOrder(Order)} ";
            IEnumerable<BEA.RMAHistory> items = SQLList<BEA.RMAHistory>(query);
            return items;
        }

        public IEnumerable<BEA.RMAActivity> ListActivities(int Id, string Order)
        {
            string query = $@"SELECT  t1.""ClgID"" AS ""Id"", t2.""CardCode""
                                      , CAST(TO_VARCHAR(IFNULL(t2.""Recontact"", t2.""CntctDate""), 'yyyy-MM-dd') || ' ' || SUBSTRING(LPAD(TO_VARCHAR(IFNULL(t2.""BeginTime"", t2.""CntctTime"")), 4, '0'), 1, 2) || ':' || SUBSTRING(LPAD(TO_VARCHAR(IFNULL(t2.""BeginTime"", t2.""CntctTime"")), 4, '0'), 3, 2) AS TIMESTAMP) AS ""ActivityDate""
                                      , t2.""AttendUser"" AS ""TreatedByCode"", t3.U_NAME AS ""TreatedBy"", t2.""Closed"", t2.""Notes"", t2.""Details"", t2.""AtcEntry"" AS ""AttachmentCode"", t2.""Attachment""
                                      , ( CASE t2.""Action"" WHEN 'C' THEN 'Llamada telefónica' WHEN 'E' THEN 'Nota' WHEN 'M' THEN 'Reunión' WHEN 'N' THEN 'Otra' WHEN 'P' THEN 'Campaña' WHEN 'T' THEN 'Tarea' ELSE '' END ) AS ""ActivityType""
                                      , t2.""Action"" AS ""ActivityTypeCode"", t4.""Name"" AS ""Subject"", CAST(t2.""CntctSbjct"" AS VARCHAR(6)) AS ""SubjectCode"", t2.""AssignedBy"" AS ""AssignedByCode"", t5.U_NAME AS ""AssignedBy"", t2.""CntctCode"" AS ""ContactCode"", t6.""Name"" AS ""Contact"", t2.""Tel"" AS ""Telephone""  
                              FROM    {DBSA}.OSCL t0 
                                      INNER JOIN {DBSA}.SCL5 t1 ON t0.""callID"" = t1.""SrvcCallId""         
                                      INNER JOIN {DBSA}.OCLG t2 ON t2.""ClgCode"" = t1.""ClgID""
                                      INNER JOIN {DBSA}.OUSR t3 ON t2.""AttendUser"" = t3.USERID
                                      LEFT OUTER JOIN {DBSA}.OCLS t4 ON t2.""CntctSbjct"" = t4.""Code""
                                      INNER JOIN {DBSA}.OUSR t5 ON t2.""AssignedBy"" = t5.USERID
                                      INNER JOIN {DBSA}.OCPR t6 ON t2.""CntctCode"" = t6.""CntctCode""
                              WHERE   t0.""callID"" = {Id} 
                              ORDER BY {GetOrder(Order)} ";
            IEnumerable<BEA.RMAActivity> items = SQLList<BEA.RMAActivity>(query);
            return items;
        }

        public IEnumerable<BEA.RMARepair> ListRepairs(int Id)
        {
            string query = $@"SELECT  t2.""SltCode"" AS ""Id"", t2.""ItemCode"", t2.""StatusNum"" AS ""StatusCode"", t3.""Name"" AS ""Status"", t2.""Owner"" AS ""OwnerCode"", t4.U_NAME AS ""Owner"", CAST(t2.""DateCreate"" AS DATE) AS ""CreateDate""
                                      , t2.""UpdateBy"" AS ""UpdatedByCode"", t5.U_NAME AS ""UpdatedBy"", CAST(t2.""DateUpdate"" AS DATE) AS ""UpdateDate"", t2.""Subject"", t2.""Symptom"", t2.""Cause"", t2.""Descriptio"" AS ""Description"", t2.""Attachment""
                              FROM    {DBSA}.OSCL T0
                                      INNER JOIN {DBSA}.SCL1 T1 ON T0.""callID"" = T1.""srvcCallID""
                                      INNER JOIN {DBSA}.OSLT T2 ON T1.""solutionID"" = T2.""SltCode"" 
                                      INNER JOIN {DBSA}.OSST t3 ON t2.""StatusNum"" = t3.""Number""
                                      INNER JOIN {DBSA}.OUSR t4 ON t2.""Owner"" = t4.USERID
                                      INNER JOIN {DBSA}.OUSR t5 ON t2.""UpdateBy"" = t5.USERID
                              WHERE   T0.""callID"" = {Id}
                              ORDER BY t1.""line"" ";
            IEnumerable<BEA.RMARepair> items = SQLList<BEA.RMARepair>(query);
            return items;
        }

        public IEnumerable<BEA.RMACost> ListCosts(int Id)
        {
            string query = $@"SELECT  t0.""callID"", t1.""Line"", CAST(t1.""Object"" AS INT) AS ""DocType"", (CASE t1.""Object"" WHEN 15 THEN 'Entrega' WHEN 17 THEN 'Orden de Venta' WHEN 67 THEN 'Transf. de Inventario' ELSE '' END) AS ""DocTypeDesc""
                                      , t1.""DocNumber"", CAST(t1.""DocPstDate"" AS DATE) AS ""PostingDate"", t1.""Transfered"", t2.""ItemCode"", t2.""ItemName"", CAST(t2.""TransToTec"" AS INT) AS ""TransToTec"", CAST(t2.""Delivered"" AS INT) AS ""Delivered""
                                      , CAST(t2.""RetFromTec"" AS INT) AS ""RetFromTec"", CAST(t2.""Returned"" AS INT) AS ""Returned"", t2.""Bill"", CAST(t2.""QtyToBill"" AS INT) AS ""QtyToBill"", CAST(t2.""QtyToInv"" AS INT) AS ""QtyToInv""
                                      , ( CASE t1.""Object"" WHEN 15 THEN t4.CANCELED WHEN 17 THEN t3.CANCELED ELSE 'N' END ) AS ""Canceled""
                              FROM    {DBSA}.OSCL t0
                                      INNER JOIN {DBSA}.SCL4 t1 ON t0.""callID"" = t1.""SrcvCallID""
                                      INNER JOIN {DBSA}.SCL2 t2 ON t0.""callID"" = t2.""SrcvCallID"" AND t1.""Line"" = t2.""Line""
                                      LEFT OUTER JOIN {DBSA}.ORDR t3 ON t1.""DocNumber"" = t3.""DocNum""
                                      LEFT OUTER JOIN {DBSA}.ODLN t4 ON t1.""DocNumber"" = t4.""DocNum""
                              WHERE   t0.""callID"" = {Id} 
                              ORDER BY t1.""Line"" ";
            IEnumerable<BEA.RMACost> items = SQLList<BEA.RMACost>(query);
            return items;
        }

        public IEnumerable<BEA.Item> ListBrands()
        {
            string query = $@"SELECT  DISTINCT U_MARCA AS ""Name""
                              FROM    {DBSA}.OINS t0
                              WHERE   U_MARCA IS NOT NULL ";
            IEnumerable<BEA.Item> items = SQLList<BEA.Item>(query);
            return items;
        }

        public IEnumerable<BEA.SAPUser> ListUsers()
        {
            string query = $@"SELECT USERID AS ""Id"", U_NAME AS ""Name"", USER_CODE AS ""Code"" FROM {DBSA}.OUSR ORDER BY U_NAME";
            IEnumerable<BEA.SAPUser> items = SQLList<BEA.SAPUser>(query);
            return items;
        }

        public IEnumerable<BEA.SAPContacts> ListContacts(string CardCode)
        {
            string query = $@"SELECT ""CntctCode"" AS ""Id"", ""CardCode"", ""Name"", ""Position"" FROM {DBSA}.OCPR WHERE LOWER(""CardCode"") = '{CardCode.ToLower()}' ";
            IEnumerable<BEA.SAPContacts> items = SQLList<BEA.SAPContacts>(query);
            return items;
        }

        public IEnumerable<BEA.Item> ListCities()
        {
            string query = $@"SELECT CAST(""Code""  AS INT) AS ""Id"", ""Name"" FROM {DBSA}.""@CIUDAD"" ";
            IEnumerable<BEA.Item> items = SQLList<BEA.Item>(query);
            return items;
        }

        public IEnumerable<BEA.Item> ListCountries()
        {
            string query = $@"SELECT ""Code"", ""Name"" FROM {DBSA}.OCRY ";
            IEnumerable<BEA.Item> items = SQLList<BEA.Item>(query);
            return items;
        }

        public IEnumerable<BEA.Item> ListStates(string CountryCode)
        {
            string query = $@"SELECT ""Code"", ""Name"" FROM {DBSA}.OCST WHERE ""Country"" = '{CountryCode}' ";
            IEnumerable<BEA.Item> items = SQLList<BEA.Item>(query);
            return items;
        }

        public IEnumerable<BEA.Item> ListLocations()
        {
            string query = $@"SELECT ""Code"" AS ""Id"", ""Name"" FROM {DBSA}.OCLO ";
            IEnumerable<BEA.Item> items = SQLList<BEA.Item>(query);
            return items;
        }

        public IEnumerable<BEA.Item> ListCallTypes()
        {
            string query = $@"SELECT ""callTypeID"" AS ""Id"", ""Name"" FROM {DBSA}.OSCT ";
            IEnumerable<BEA.Item> items = SQLList<BEA.Item>(query);
            return items;
        }

        public IEnumerable<BEA.Item> ListOrigins()
        {
            string query = $@"SELECT ""originID"" AS ""Id"", ""Name"" FROM {DBSA}.OSCO ";
            IEnumerable<BEA.Item> items = SQLList<BEA.Item>(query);
            return items;
        }

        public IEnumerable<BEA.Item> ListProblemTypes()
        {
            string query = $@"SELECT ""prblmTypID"" AS ""Id"", ""Name"" FROM {DBSA}.OSCP ";
            IEnumerable<BEA.Item> items = SQLList<BEA.Item>(query);
            return items;
        }

        public IEnumerable<BEA.Item> ListSolutionStatuses()
        {
            string query = $@"SELECT ""Number"" AS ""Id"", ""Name"" FROM {DBSA}.OSST ";
            IEnumerable<BEA.Item> items = SQLList<BEA.Item>(query);
            return items;
        }

        #endregion

        #region Search Methods

        public BEA.Item SearchState(int Id)
        {
            string query = $@"SELECT  ""statusID"" AS ""Id"", ""Name""
                              FROM    {DBSA}.OSCS 
                              WHERE   ""statusID"" = {Id} ";

            BEA.Item item = SQLSearch<BEA.Item>(query);
            return item;
        }

        public BEA.RMAActivity SearchActivity(int Id)
        {
            string query = $@"SELECT  t1.""ClgID"" AS ""Id""
                                      , CAST(TO_VARCHAR(IFNULL(t2.""Recontact"", t2.""CntctDate""), 'yyyy-MM-dd') || ' ' || SUBSTRING(LPAD(TO_VARCHAR(IFNULL(t2.""BeginTime"", t2.""CntctTime"")), 4, '0'), 1, 2) || ':' || SUBSTRING(LPAD(TO_VARCHAR(IFNULL(t2.""BeginTime"", t2.""CntctTime"")), 4, '0'), 3, 2) AS TIMESTAMP) AS ""ActivityDate""
                                      , t3.U_NAME AS ""TreatedBy"", t2.""Closed"", t2.""Notes"", t2.""Details"", t2.""Attachment"", ( CASE t2.""Action"" WHEN 'C' THEN 'Llamada telefónica' WHEN 'E' THEN 'Nota' WHEN 'M' THEN 'Reunión' WHEN 'N' THEN 'Otra' WHEN 'P' THEN 'Campaña' WHEN 'T' THEN 'Tarea' ELSE '' END ) AS ""ActivityType""
                                      , t4.""Name"" AS ""Subject"", CAST(t2.""CntctSbjct"" AS VARCHAR(6)) AS ""SubjectCode"", t5.U_NAME AS ""AssignedBy"", t6.""Name"" AS ""Contact"", t2.""Tel"" AS ""Telephone""
                              FROM    {DBSA}.OSCL t0 
                                      INNER JOIN {DBSA}.SCL5 t1 ON t0.""callID"" = t1.""SrvcCallId""         
                                      INNER JOIN {DBSA}.OCLG t2 ON t2.""ClgCode"" = t1.""ClgID""
                                      INNER JOIN {DBSA}.OUSR t3 ON t2.""AttendUser"" = t3.USERID 
                                      LEFT OUTER JOIN {DBSA}.OCLS t4 ON t2.""CntctSbjct"" = t4.""Code""
                                      INNER JOIN {DBSA}.OUSR t5 ON t2.""AssignedBy"" = t5.USERID
                                      INNER JOIN {DBSA}.OCPR t6 ON t2.""CntctCode"" = t6.""CntctCode""
                              WHERE   t1.""ClgID"" = {Id} ";

            BEA.RMAActivity item = SQLSearch<BEA.RMAActivity>(query);
            return item;
        }

        public BEA.ProductCard SearchProductCard(string SerialNumber)
        {
            string query = $@"SELECT  ""itemCode"" AS ""ItemCode"", ""itemName"" AS ""ItemName"", t0.U_MARCA AS ""Brand"", t1.U_LINEA AS ""Line"", t1.U_GPRODUCT AS ""ProductManager"", ""invoiceNum"" AS ""RefNV"", CAST(""dlvryDate"" AS DATE) AS ""PurchaseDate"", ""warranty"" AS ""Warranty"", ""customer"" AS ""ClientCode"", ""custmrName"" AS ""ClientName"", ""state"" AS ""CityCode""
                              FROM    {DBSA}.OINS t0
                                      INNER JOIN {DBSA}.OITM t1 ON t0.""itemCode"" = t1.""ItemCode""
                              WHERE   ""manufSN"" = '{SerialNumber}' ";

            BEA.ProductCard item = SQLSearch<BEA.ProductCard>(query);
            return item;
        }

        #endregion

        #region Save to SAP Methods

        //public string SaveServiceCall()
        //{
        //    string newKey = "";
        //    SAPSettings settings = GetConfigData();
        //    SAPbobsCOM.Company oCompany; // The company object
        //    oCompany = new SAPbobsCOM.Company
        //    {
        //        DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB,
        //        Server = $"{settings.Server}:{settings.Port}",
        //        language = SAPbobsCOM.BoSuppLangs.ln_Spanish_La,
        //        CompanyDB = settings.DBSA,
        //        UseTrusted = false,
        //        UserName = settings.User,
        //        Password = settings.Password
        //    };

        //    int result = oCompany.Connect();
        //    if (result == 0)
        //    {
        //        SAPbobsCOM.ServiceCalls sc = (SAPbobsCOM.ServiceCalls)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oServiceCalls);
        //        sc.AssigneeCode = 1;
        //        sc.CallType = 1; //OSCT
        //        sc.City = "SANTA CRUZ";
        //        sc.Country = "BO";
        //        sc.CreationDate = DateTime.Now;
        //        sc.CreationTime = DateTime.Now;
        //        sc.CustomerCode = "CCOA-001";
        //        sc.CustomerName = "COACOM S.R.L.";
        //        //sc.Description = "A16";
        //        sc.ItemCode = "DECD7XK5";
        //        sc.ItemDescription = "SERVIDOR DELL POWER EDGE R640 Rack/SILVER 4114(2.2GHz-10C)/16GB/300GB SAS 15K HD/H730P/iDRAC 9 EXP";
        //        sc.Location = -2;
        //        sc.ManufacturerSerialNum = "7C6J9Z2";
        //        sc.Origin = 1; //OSCO
        //        sc.Priority = SAPbobsCOM.BoSvcCallPriorities.scp_High;
        //        sc.Resolution = "A18";
        //        sc.ResolutionDate = DateTime.Now;
        //        sc.ResolutionTime = DateTime.Now;
        //        sc.Room = "A19";
        //        sc.StartDate = DateTime.Now;
        //        sc.StartTime = DateTime.Now;
        //        //sc.ClosingDate = DateTime.Now;
        //        //sc.ClosingTime = 1511;
        //        sc.State = "1";
        //        sc.Status = -3;
        //        sc.Street = "A20";
        //        sc.Subject = "A21";
        //        sc.TechnicianCode = 116;
        //        sc.UserFields.Fields.Item("U_FECHING").Value = DateTime.Now;
        //        sc.UserFields.Fields.Item("U_HORA").Value = DateTime.Now;
        //        sc.UserFields.Fields.Item("U_CIUDAD").Value = "1";
        //        sc.UserFields.Fields.Item("U_NOMUSFIN").Value = "Julio Peredo";
        //        //SI hay entrega
        //        //sc.UserFields.Fields.Item("U_FECHEN").Value = DateTime.Now;
        //        //sc.UserFields.Fields.Item("U_HORAENT").Value = DateTime.Now;
        //        //entregado por
        //        sc.UserFields.Fields.Item("U_ENTPOR").Value = "J. Peredo";
        //        sc.UserFields.Fields.Item("U_SERV_EXTERNO").Value = "A23";
        //        sc.UserFields.Fields.Item("U_TECNICO_SERV_EXTERNO").Value = "A24";
        //        sc.UserFields.Fields.Item("U_NRO_BOLETA_EXTERNA").Value = "A25";
        //        sc.UserFields.Fields.Item("U_DIRECCION_SERV_EXTERNO").Value = "A26";
        //        sc.UserFields.Fields.Item("U_NOGUIA").Value = "A27";
        //        sc.UserFields.Fields.Item("U_TRANSP").Value = "A28";
        //        sc.UserFields.Fields.Item("U_PIEZASCON").Value = 10000;
        //        sc.UserFields.Fields.Item("U_PIEZASCON_ANTERIOR").Value = 10010;
        //        sc.UserFields.Fields.Item("U_PIEZASCONT_DIF").Value = 10;
        //        //fecha de compra
        //        sc.UserFields.Fields.Item("U_FECHCOM").Value = DateTime.Now;
        //        sc.UserFields.Fields.Item("U_REPFALL").Value = "Alguien";
        //        //recibido por
        //        sc.UserFields.Fields.Item("U_AUX1").Value = "A29";
        //        // # NV
        //        sc.UserFields.Fields.Item("U_PRUEBCO").Value = "NV-XXXXXX";
        //        sc.UserFields.Fields.Item("U_COMENT").Value = "Mis comentarios";
        //        sc.UserFields.Fields.Item("U_LUGAR").Value = "1"; // 1 (On Site)  o 2 (Bench)
        //        // Warranty SI o NO
        //        sc.UserFields.Fields.Item("U_ENGRTA").Value = "SI";

        //        if (sc.Add() == 0)
        //        {
        //            newKey = oCompany.GetNewObjectKey();
        //        }
        //        else
        //        {
        //            throw new Exception(oCompany.GetLastErrorDescription());
        //        }
        //    }
        //    else
        //    {
        //        throw new Exception(oCompany.GetLastErrorDescription());
        //    }
        //    if (oCompany != null && oCompany.Connected) oCompany.Disconnect();
        //    return newKey;
        //}

        //public void SaveActivityToSAP(ref BEA.RMAActivity Item, int IdRMA)
        //{
        //    SAPSettings settings = GetConfigData();
        //    SAPbobsCOM.Company oCompany; // The company object
        //    oCompany = new SAPbobsCOM.Company
        //    {
        //        DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB,
        //        Server = $"{settings.Server}:{settings.Port}",
        //        language = SAPbobsCOM.BoSuppLangs.ln_Spanish_La,
        //        CompanyDB = settings.DBSA,
        //        UseTrusted = false,
        //        UserName = settings.User,
        //        Password = settings.Password
        //    };

        //    int result = oCompany.Connect();
        //    if (result == 0)
        //    {
        //        SAPbobsCOM.ServiceCalls sc = (SAPbobsCOM.ServiceCalls)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oServiceCalls);
        //        sc.GetByKey(IdRMA);

        //        SAPbobsCOM.Contacts act = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oContacts);
        //        if (Item.Id > 0) act.GetByKey(Item.Id);

        //        //act.DocType = (int)SAPbobsCOM.BoObjectTypes.oServiceCalls;
        //        act.Activity = Item.ActivityTypeCode switch
        //        {
        //            "C" => SAPbobsCOM.BoActivities.cn_Conversation,
        //            "E" => SAPbobsCOM.BoActivities.cn_Note,
        //            "M" => SAPbobsCOM.BoActivities.cn_Meeting,
        //            "N" => SAPbobsCOM.BoActivities.cn_Other,
        //            "P" => SAPbobsCOM.BoActivities.cn_Campaign,
        //            "T" => SAPbobsCOM.BoActivities.cn_Task,
        //            _ => SAPbobsCOM.BoActivities.cn_Other
        //        };
        //        act.ContactDate = Item.ActivityDate;
        //        act.ContactTime = Item.ActivityDate;
        //        act.CardCode = Item.CardCode;
        //        act.Closed = Item.Closed == "Y" ? SAPbobsCOM.BoYesNoEnum.tYES : SAPbobsCOM.BoYesNoEnum.tNO;
        //        act.ContactPersonCode = Item.ContactCode;
        //        act.Subject = Item.Subject;
        //        act.Details = Item.Details;
        //        act.Notes = Item.Notes;
        //        act.Phone = Item.Telephone;
        //        act.HandledBy = Item.TreatedByCode;
        //        //pendiente el asignado (se autoasigna con el usuario que se usa)

        //        var files = Item.Files.Where(x => x.Action == "I");
        //        int count = act.Attachments.Count;
        //        foreach (var f in files)
        //        {
        //            act.Attachments.Add();
        //            act.Attachments.Item(count++).FileName = f.Name;
        //        }
        //        files = Item.Files.Where(x => x.Action == "D" & x.Existing == "Y");
        //        foreach (var f in files)
        //        {
        //            //SAPbobsCOM.Attachments2 oAttachments2 = (SAPbobsCOM.Attachments2)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oAttachments2);
        //            //SAPbobsCOM.Attachments2_Lines oAttachments2_Lines;
        //            //oAttachments2.GetByKey(Item.AttachmentCode);
        //            //oAttachments2_Lines = oAttachments2.Lines;
        //            //oAttachments2_Lines.SetCurrentLine(f.Line);
        //            //oAttachments2_Lines.FileName = null;
        //            //if (oAttachments2.Update() != 0) throw new Exception(oCompany.GetLastErrorDescription());

        //            act.Attachments.Item(f.Line - 1).FileName = string.Empty;
        //            act.Attachments.Refresh();
        //        }

        //        if (Item.Id == 0)
        //        {
        //            if (act.Add() == 0)
        //            {
        //                string newKey = oCompany.GetNewObjectKey();
        //                sc.Activities.Add();
        //                sc.Activities.ActivityCode = int.Parse(newKey);
        //                if (sc.Update() != 0) throw new Exception(oCompany.GetLastErrorDescription());
        //            }
        //            else
        //            {
        //                throw new Exception(oCompany.GetLastErrorDescription());
        //            }
        //        }
        //        else
        //        {
        //            if (act.Update() != 0) throw new Exception(oCompany.GetLastErrorDescription());
        //        }
        //    }
        //    if (oCompany != null && oCompany.Connected) oCompany.Disconnect();
        //}

        //public void DeleteActivityInSAP(int Id, int ActivityId)
        //{
        //    SAPSettings settings = GetConfigData();
        //    SAPbobsCOM.Company oCompany; // The company object

        //    oCompany = new SAPbobsCOM.Company
        //    {
        //        DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB,
        //        Server = $"{settings.Server}:{settings.Port}",
        //        language = SAPbobsCOM.BoSuppLangs.ln_Spanish_La,
        //        CompanyDB = settings.DBSA,
        //        UseTrusted = false,
        //        UserName = settings.User,
        //        Password = settings.Password
        //    };

        //    int result = oCompany.Connect();
        //    if (result == 0)
        //    {
        //        SAPbobsCOM.ServiceCalls sc = (SAPbobsCOM.ServiceCalls)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oServiceCalls);
        //        sc.GetByKey(Id);
        //        sc.Activities.ActivityCode = ActivityId;
        //        sc.Activities.Delete();
        //        sc.Update();
        //    }
        //}

        #endregion

        #region Private Methods

        //private SAPSettings GetConfigData()
        //{
        //    var currentDir = IISHelper.GetContentRoot(); //Directory.GetCurrentDirectory() Antes de 2.2 funcionaba
        //    var builder = new ConfigurationBuilder()
        //         .SetBasePath(currentDir) // requires Microsoft.Extensions.Configuration.Json
        //         .AddJsonFile("appsettings.json") // requires Microsoft.Extensions.Configuration.Json
        //         .AddEnvironmentVariables(); // requires Microsoft.Extensions.Configuration.EnvironmentVariables
        //    IConfigurationRoot config = builder.Build();

        //    var settingsSection = config.GetSection("SAPSettings");
        //    var settings = settingsSection.Get<SAPSettings>();
        //    return settings;
        //}

        #endregion

        #region Constructors

        public RMA() : base() { }

        #endregion
    }
}

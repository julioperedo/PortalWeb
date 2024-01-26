using System;
using System.Collections.Generic;
using System.Text;

namespace BEntities.SAP
{
    //public class RMA : BEntity
    //{
    //    public int Id { get; set; }
    //    public int TechnicianCode { get; set; }
    //    public string TechnicianName { get; set; }
    //    public DateTime CreateDate { get; set; }
    //    public DateTime? AdmissionDate { get; set; }
    //    public DateTime? CloseDate { get; set; }
    //    public int OpenDays { get; set; }
    //    public string ClientCode { get; set; }
    //    public string ClientName { get; set; }
    //    public string City { get; set; }
    //    public string FinalUser { get; set; }
    //    public string ReportedBy { get; set; }
    //    public string ReceivedBy { get; set; }
    //    public string Brand { get; set; }
    //    public string Line { get; set; }
    //    public string ItemCode { get; set; }
    //    public string ItemName { get; set; }
    //    public string ProductManager { get; set; }
    //    public string Warranty { get; set; }
    //    public string Serial { get; set; }
    //    public int StateCode { get; set; }
    //    public string StateName { get; set; }
    //    public string Subject { get; set; }
    //    public int CountedPieces { get; set; }
    //    public int PriorCountedPieces { get; set; }
    //    public int DiffCountedPieces { get; set; }
    //    public DateTime? PurchaseDate { get; set; }
    //    public DateTime? ClientPurchaseDate { get; set; }
    //    public string Telephone { get; set; }
    //    public string RefNV { get; set; }
    //    public string Comments { get; set; }
    //    public string FileName { get; set; }
    //    public string FilePath { get; set; }
    //    public DateTime? DeliveredDate { get; set; }
    //    public string DeliveredBy { get; set; }
    //    public string ExternalService { get; set; }
    //    public string ExternalServiceTechnician { get; set; }
    //    public string ExternalServiceNumber { get; set; }
    //    public string ExternalServiceAddress { get; set; }
    //    public string GuideNumber { get; set; }
    //    public string Transport { get; set; }
    //    public string ServiceType { get; set; }
    //    public string Street { get; set; }
    //    public string City2 { get; set; }
    //    public string Location { get; set; }
    //    public string Room { get; set; }
    //    public string State { get; set; }
    //    public string Country { get; set; }
    //    public string Resolution { get; set; }
    //}

    public class RMAHistory : BEntity
    {
        public int Order { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Status { get; set; }
        public string Subject { get; set; }
    }

    public class RMAActivity : BEntity
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string CardCode { get; set; }
        public DateTime ActivityDate { get; set; }
        public string TreatedBy { get; set; }
        public int TreatedByCode { get; set; }
        public int AssignedByCode { get; set; }
        public string AssignedBy { get; set; }
        public string Closed { get; set; }
        public string Notes { get; set; }
        public string Details { get; set; }
        public string Attachment { get; set; }
        public string ActivityType { get; set; }
        public string ActivityTypeCode { get; set; }
        public string Subject { get; set; }
        public string SubjectCode { get; set; }
        public string Contact { get; set; }
        public int ContactCode { get; set; }
        public string Telephone { get; set; }
        public int AttachmentCode { get; set; }
        public List<RMAFile> Files { get; set; }
    }

    public class RMAFile : BEntity
    {
        public string Name { get; set; }
        public string Action { get; set; }
        public string Existing { get; set; }
        public int Line { get; set; }
    }

    public class RMARepair : BEntity
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string ItemCode { get; set; }
        public string Status { get; set; }
        public int StatusCode { get; set; }
        public string Owner { get; set; }
        public int OwnerCode { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdatedBy { get; set; }
        public int UpdatedByCode { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Subject { get; set; }
        public string Symptom { get; set; }
        public string Cause { get; set; }
        public string Description { get; set; }
        public string Attachment { get; set; }
    }

    public class RMACost : BEntity
    {
        public int callID { get; set; }
        public int Line { get; set; }
        public int DocType { get; set; }
        public string DocTypeDesc { get; set; }
        public int DocNumber { get; set; }
        public DateTime PostingDate { get; set; }
        public string Transfered { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int TransToTec { get; set; }
        public int Delivered { get; set; }
        public int RetFromTec { get; set; }
        public int Returned { get; set; }
        public string Bill { get; set; }
        public int QtyToBill { get; set; }
        public int QtyToInv { get; set; }
        public string Canceled { get; set; }
    }

    public class ProductCard : BEntity
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Brand { get; set; }
        public string Line { get; set; }
        public string ProductManager { get; set; }
        public int RefNV { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Warranty { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string CityCode { get; set; }
        public int LastCountedPieces { get; set; }
        public string ReportedBy { get; set; }
        public string Phone { get; set; }
        public string EMail { get; set; }
        public string FinalUser { get; set; }
        public string ExternalService { get; set; }
        public string ExServTechnician { get; set; }
        public string Address { get; set; }
    }
}

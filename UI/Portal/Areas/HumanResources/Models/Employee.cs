namespace Portal.Areas.HumanResources.Models
{
    public class EmployeeVacation : BEntities.SAP.Employee
    {
        public int Years { get; set; }
        public int Months { get; set; }
        public decimal Days { get; set; }
        public decimal TotalDays => Days + ExtraDays;
        public decimal ExtraDays { get; set; }
        public decimal Taken { get; set; }
        public decimal Remaining => TotalDays - Taken;
        public EmployeeVacation() { }
        public EmployeeVacation(BEntities.SAP.Employee Item)
        {
            Id = Item.Id;
            FirstName = Item.FirstName;
            LastName = Item.LastName;
            StartDate = Item.StartDate;
            TermDate = Item.TermDate;
            Picture = Item.Picture;
        }

    }
}

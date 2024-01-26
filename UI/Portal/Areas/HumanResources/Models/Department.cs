namespace Portal.Areas.HumanResources.Models
{
	public class Department
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public long IdManager { get; set; }
		public string Manager { get; set; }
	}
}

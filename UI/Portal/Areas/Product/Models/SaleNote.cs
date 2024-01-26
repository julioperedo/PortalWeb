using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Areas.Product.Models
{
	public class SaleNote
	{
		public string Subsidiary { get; set; }
		public string ClientCode { get; set; }
		public string ClientName { get; set; }
		public int DocType { get; set; }
		public string DocTypeDesc { get; set; }
		public long Number { get; set; }
		public DateTime Date { get; set; }
		public bool IsOwn { get; set; }
		public List<SaleNoteDetail> Items { get; set; }
	}
	public class SaleNoteDetail
	{
		public string ItemCode { get; set; }
		public string Name { get; set; }
		public string SerialsResume { get; set; }
		public List<string> Serials { get; set; }
		public int Count { get { return Serials?.Count ?? 0; } }
	}
}

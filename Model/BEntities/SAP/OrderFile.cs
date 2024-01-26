using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class OrderFile : BEntity
    {
        public string Subsidiary { get; set; }
        public int DocEntry { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public string FileExt { get; set; }
    }
}

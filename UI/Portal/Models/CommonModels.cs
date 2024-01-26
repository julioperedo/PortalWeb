using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Models {
    public class ChartData
    {
        public string Title { get; set; }
        public long Universe { get; set; }
        public string Type { get; set; }
        public List<string> Labels { get; set; }
        public List<ChartSerie> Series { get; set; }
        public List<ChartSerie> DrillDown { get; set; }
    }

    public class ChartSerie
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public double Total { get; set; }
        public List<DataPair> Data { get; set; }
        public string Stack { get; set; }
    }

    public class DataPair
    {
        public string Label { get; set; }
        public decimal? Value { get; set; }
        public int Count { get; set; }
        public string Color { get; set; }
        public string DrillDown { get; set; }
        public decimal? Percentage { get; set; }
        public decimal? Total { get; set; }
    }

    public class EmailAccount
    {
        public string Name { get; set; }
        public string EMail { get; set; }
    }
}

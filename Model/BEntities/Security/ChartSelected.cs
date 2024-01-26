namespace BEntities.Security {
    public class ChartSelected : BEntity {
        public long IdChartType { get; set; }
        public long IdChart { get; set; }
        public string ChartTypeName { get; set; }
        public string ChartName { get; set; }
    }
}

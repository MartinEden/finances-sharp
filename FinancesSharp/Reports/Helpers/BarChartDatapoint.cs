namespace FinancesSharp.Reports.Helpers
{
    public class BarChartDatapoint
    {
        public readonly string MainLabel;
        public readonly string SubLabel;
        public readonly object Value;
        public readonly string HumanReadableValue;
        public readonly int? DrilldownId;

        public BarChartDatapoint(string mainLabel, string subLabel, object value, string humanReadableValue, int? drilldownId)
        {
            MainLabel = mainLabel;
            SubLabel = subLabel;
            Value = value;
            HumanReadableValue = humanReadableValue;
            DrilldownId = drilldownId;
        }
    }
}
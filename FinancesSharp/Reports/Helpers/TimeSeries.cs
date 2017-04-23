using System.Collections;
using System.Collections.Generic;

namespace FinancesSharp.Reports.Helpers
{
    public class TimeSeries<TKey>
    {
        public readonly string SeriesName;
        public readonly IEnumerable<TimeSeriesDatapoint> Datapoints;
        public readonly int? DrilldownId;

        public TimeSeries(string seriesName, IEnumerable<TimeSeriesDatapoint> datapoints, int? drilldownId)
        {
            SeriesName = seriesName;
            Datapoints = datapoints;
            DrilldownId = drilldownId;
        }
    }

    public class TimeSeriesDatapoint
    {
        public object Date { get; set; }
        public object Value { get; set; }
        public string HumanReadableValue { get; set; }
    }
}
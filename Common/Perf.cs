using System.Text;
using HdrHistogram;

namespace Common;

public class Perf
{
    private readonly Dictionary<string, LongHistogram> histograms = new();

    public void RecordValue(long elapsed, string from, string to)
    {
        var direction = $"{from}>>{to}";
        if (!histograms.TryGetValue(direction, out var histogram))
        {
            // A Histogram covering the range from ~466 nanoseconds to 1 hour
            // (3,600,000,000,000 ns) with a resolution of 3 significant figures:
            histogram = new LongHistogram(TimeStamp.Hours(1), 3);
            histograms.Add(direction, histogram);
        }
        histogram.RecordValue(elapsed);
    }
    
    public string Info()
    {
        var sb = new StringBuilder();
        foreach (var kvp in histograms)
        {
            sb.AppendLine(kvp.Key);
            
            var writer = new StringWriter();
            var scalingRatio = OutputScalingFactor.TimeStampToMicroseconds;
            var histogram = kvp.Value;
            histogram.OutputPercentileDistribution(writer, outputValueUnitScalingRatio: scalingRatio);
            sb.AppendLine(writer.ToString());
        }
        return sb.ToString();
    }
}

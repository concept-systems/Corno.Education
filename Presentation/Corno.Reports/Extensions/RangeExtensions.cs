using System.Collections.Generic;
using System.Linq;

namespace Corno.Reports.Extensions;

public static class RangeExtensions
{
    public static IEnumerable<(long begin, long end)> Ranges(this IEnumerable<long> numbers)
    {
        // ReSharper disable once GenericEnumeratorNotDisposed
        var e = numbers.GetEnumerator();
        if (!e.MoveNext()) yield break;

        var begin = e.Current;
        var end = begin + 1;
        while (e.MoveNext())
        {
            if (e.Current != end)
            {
                yield return (begin, end);
                begin = end = e.Current;
            }
            end++;
        }
        yield return (begin, end);
    }

    public static string ToRangeString(this IEnumerable<(long begin, long end)> ranges)
    {
        return "[" + string.Join(", ", ranges.Select(r => r.end - r.begin == 1 ? $"{r.begin}" : $"{r.begin}-{r.end - 1}")) + "]";
    }
}
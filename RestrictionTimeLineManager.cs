using System;
using System.Collections.Generic;

namespace Antiban;

public class RestrictionTimeLineManager
{
    private readonly Dictionary<int, RestrictionTimeLine> _restrictionTimeLines;

    private readonly Dictionary<int, TimeSpan> _mapTimings = new Dictionary<int, TimeSpan>()
    {
        {-1, TimeSpan.FromSeconds(10)},
        {0, TimeSpan.FromSeconds(60)}, 
        {1, TimeSpan.FromHours(24)},
    };

    public RestrictionTimeLineManager(RestrictionTimeLine restrictionAllTimeLine)
    {
        _restrictionTimeLines = new Dictionary<int, RestrictionTimeLine>()
        {
            { -1, restrictionAllTimeLine }
        };
    }

    public RestrictionTimeLine Get(int key)
    {
        var restrictionTimeLine = _restrictionTimeLines.GetValueOrDefault(key, new RestrictionTimeLine());
        if (!_restrictionTimeLines.ContainsKey(key))
            _restrictionTimeLines.Add(key, restrictionTimeLine);
        return restrictionTimeLine;
    }

    public DateTime FindFreeSpace(int priority, DateTime startTime)
    {
        bool isShifted;
        do
        {
            isShifted = false;
            for (var p = -1; p <= priority; p++)
            {
                var restrictionTimeLine = Get(p);
                var restrictionTime = new RestrictionTime(startTime, _mapTimings[p]);
                
                var freeSpace = restrictionTimeLine.FindFreeSpace(restrictionTime);
                if (freeSpace.Start <= restrictionTime.Start) continue;

                startTime = restrictionTime.ShiftTo(freeSpace.Start)
                    .Start;

                isShifted = true;
            }

        } while (isShifted);

        return startTime;
    }

    public DateTime FindFreeSpaceAndReserve(int priority, DateTime startTime)
    {
        var sentDateTime = FindFreeSpace(priority, startTime);

        for (var p = -1; p <= priority; p++)
        {
            var restrictionTimeLine = Get(p);
            var restrictionTime = new RestrictionTime(sentDateTime, _mapTimings[p]);
            restrictionTimeLine.Add(restrictionTime);
        }

        return sentDateTime;
    }
}
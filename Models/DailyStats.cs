namespace Diploma_cs.Models;

public class DailyStats
{
    public DateTime Date { get; set; }
    public int SessionsCount { get; set; }
    public int PacksCount { get; set; }
    public int DailyTarget { get; set; }
    public DailyStats()
    {
    }

    public DailyStats(DateTime date, int sessionsCount, int packsCount, int dailyTarget)
    {
        Date = date.Date;
        SessionsCount = sessionsCount;
        PacksCount = packsCount;
        DailyTarget = dailyTarget;
    }

    public DayStatus GetDayStatus()
    {
        if (SessionsCount < DailyTarget)
            return DayStatus.Ok;
        else if (SessionsCount == DailyTarget)
            return DayStatus.Target;
        else
            return DayStatus.Exceeded;
    }

    public enum DayStatus
    {
        Ok,
        Target,
        Exceeded
    }
}
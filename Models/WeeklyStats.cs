namespace Diploma_cs.Models;

public class WeeklyStats
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public float AvgConsumedWeek { get; set; }
    public float AvgBoughtPacks { get; set; }
    public float BoughtSum { get; set; }

    public WeeklyStats()
    {
    }

    public WeeklyStats(DateTime startDate, DateTime endDate, float avgConsumedWeek, float avgBoughtPacks, float boughtSum)
    {
        StartDate = startDate.Date;
        EndDate = endDate.Date;
        AvgConsumedWeek = avgConsumedWeek;
        AvgBoughtPacks = avgBoughtPacks;
        BoughtSum = boughtSum;
    }
}
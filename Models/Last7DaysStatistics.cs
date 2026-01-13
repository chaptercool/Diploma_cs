namespace Diploma_cs.Models;

public sealed class Last7DaysStatistics
{
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }

    public required IReadOnlyList<string> Labels { get; init; }

    public required IReadOnlyList<float> DailyConsumption { get; init; }
    public required IReadOnlyList<float> MoneySpent { get; init; }
    public required IReadOnlyList<float> TargetExceededBy { get; init; }

    public int DaysWithAnyStats { get; init; }
    public int ExceededDaysCount { get; init; }

    public bool HasEnoughData(int minDays = 3) => DaysWithAnyStats >= minDays;
}

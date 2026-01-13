using Diploma_cs.Models;

namespace Diploma_cs.Data.Services;

public sealed class UiStatisticsService
{
    private readonly AppDataService _appDataService;

    public UiStatisticsService(AppDataService appDataService)
    {
        _appDataService = appDataService ?? throw new ArgumentNullException(nameof(appDataService));
    }

    public async Task<Last7DaysStatistics> GetLast7DaysAsync(DateTime? endDate = null)
    {
        var end = (endDate ?? DateTime.Today).Date;
        var start = end.AddDays(-6);

        var labels = Enumerable.Range(0, 7)
            .Select(i => start.AddDays(i).ToString("dd.MM"))
            .ToArray();

        var dailyStats = await _appDataService.GetDailyStatsRangeAsync(start, end);
        var profile = await _appDataService.GetUserProfileAsync();
        var packPrice = profile?.PackPrice ?? 0f;

        var byDate = dailyStats
            .GroupBy(s => s.Date.Date)
            .ToDictionary(g => g.Key, g => g.First());

        var consumption = new float[7];
        var spent = new float[7];
        var exceededBy = new float[7];

        int daysWithAnyStats = 0;
        int exceededDays = 0;

        for (int i = 0; i < 7; i++)
        {
            var date = start.AddDays(i).Date;

            if (!byDate.TryGetValue(date, out var ds))
                continue;

            int sessions = GetIntProperty(ds, "SessionsCount")
                           ?? GetIntProperty(ds, "SmokingSessions")
                           ?? GetIntProperty(ds, "CigarettesSmoked")
                           ?? 0;

            int packs = GetIntProperty(ds, "PacksCount")
                        ?? GetIntProperty(ds, "PackPurchases")
                        ?? GetIntProperty(ds, "PacksBought")
                        ?? 0;

            int target = GetIntProperty(ds, "DailyTarget") ?? 0;

            consumption[i] = sessions;
            spent[i] = packs * packPrice;

            var diff = sessions - target;
            if (diff > 0)
            {
                exceededBy[i] = diff;
                exceededDays++;
            }

            if (sessions > 0 || packs > 0 || target > 0)
                daysWithAnyStats++;
        }

        return new Last7DaysStatistics
        {
            StartDate = start,
            EndDate = end,
            Labels = labels,
            DailyConsumption = consumption,
            MoneySpent = spent,
            TargetExceededBy = exceededBy,
            DaysWithAnyStats = daysWithAnyStats,
            ExceededDaysCount = exceededDays
        };
    }

    private static int? GetIntProperty(object obj, string name)
    {
        var prop = obj.GetType().GetProperty(name);
        if (prop == null)
            return null;

        var value = prop.GetValue(obj);
        if (value == null)
            return null;

        if (value is int i)
            return i;

        if (value is long l)
            return (int)l;

        if (value is float f)
            return (int)f;

        if (value is double d)
            return (int)d;

        return null;
    }
}

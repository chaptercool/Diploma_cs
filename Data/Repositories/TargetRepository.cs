using Diploma_cs.Data.Services;

namespace Diploma_cs.Data.Repositories;

public class TargetRepository
{
    private readonly CsvTargetService _targetService;
    private readonly WeeklyStatsRepository _weeklyStatsRepository;
    private const int SuccessThreshold = 4;

    public TargetRepository(CsvTargetService targetService, 
                          WeeklyStatsRepository? weeklyStatsRepository = null)
    {
        _targetService = targetService ?? throw new ArgumentNullException(nameof(targetService));
        _weeklyStatsRepository = weeklyStatsRepository;
    }

    public async Task SaveInitialTargetAsync(int dailyConsumption)
    {
        int initialTarget = dailyConsumption + 2;
        var record = new TargetRecord
        {
            Date = DateTime.Now,
            Target = initialTarget,
            CalculatedBy = "Initial",
            MlScore = 0f
        };

        await _targetService.AddTargetAsync(record);
    }

    public async Task<int?> CalculateAndSaveMLTargetAsync()
    {
        if (_weeklyStatsRepository == null)
            throw new InvalidOperationException("WeeklyStatsRepository is required for ML target calculation");

        var weeklyStats = await _weeklyStatsRepository.GetLatestWeeklyStatsAsync();
        if (weeklyStats == null)
            return null;

        try
        {
            var modelInput = new MainComputingModule.ModelInput
            {
                AvgConsumedWeek = weeklyStats.AvgConsumedWeek,
                AvgBoughtPacks = weeklyStats.AvgBoughtPacks,
                BoughtSum = weeklyStats.BoughtSum
            };

            var prediction = MainComputingModule.Predict(modelInput);
            float mlScore = prediction.Score;

            int newTarget = Math.Max(1, (int)Math.Round(mlScore));

            var record = new TargetRecord
            {
                Date = DateTime.Now,
                Target = newTarget,
                CalculatedBy = "ML",
                MlScore = mlScore
            };

            await _targetService.AddTargetAsync(record);
            return newTarget;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error calculating ML target", ex);
        }
    }

    public async Task<int?> GetCurrentTargetAsync()
    {
        var record = await _targetService.GetCurrentTargetAsync();
        return record?.Target;
    }

    public async Task<int?> GetTargetForDateAsync(DateTime date)
    {
        var record = await _targetService.GetTargetForDateAsync(date);
        return record?.Target;
    }

    public async Task<bool> IsSuccessfullyQuitAsync()
    {
        var currentTarget = await GetCurrentTargetAsync();
        return currentTarget.HasValue && currentTarget.Value <= SuccessThreshold;
    }

    public async Task<List<TargetRecord>> GetAllTargetsAsync()
    {
        return await _targetService.GetAllTargetsAsync();
    }

    public async Task<TargetRecord?> GetTargetRecordForDateAsync(DateTime date)
    {
        return await _targetService.GetTargetForDateAsync(date);
    }

    public async Task<TargetRecord?> GetCurrentTargetRecordAsync()
    {
        return await _targetService.GetCurrentTargetAsync();
    }

    public async Task ClearAllTargetsAsync()
    {
        await _targetService.ClearAllTargetsAsync();
    }

    public string GetTargetFilePath() => _targetService.GetTargetFilePath();
}
using Diploma_cs.Data.Services;
using Diploma_cs.Models;

namespace Diploma_cs.Data.Repositories;

public class UserProfileRepository
{
    private readonly CsvUserProfileService _profileService;
    private readonly TargetRepository _targetRepository;

    public UserProfileRepository(CsvUserProfileService profileService, 
                               TargetRepository? targetRepository = null)
    {
        _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
        _targetRepository = targetRepository;
    }

    public async Task SaveUserProfileAsync(string name, int age, string gender, 
                                          int dailyConsumption, float packPrice)
    {
        int initialTarget = dailyConsumption + 2;
        var profile = new UserProfile
        {
            Name = name,
            Age = age,
            Gender = gender,
            DailyConsumption = dailyConsumption,
            PackPrice = packPrice,
            InitialTarget = initialTarget,
            SetupCompletedDate = DateTime.Now
        };

        await _profileService.SaveUserProfileAsync(profile);

        if (_targetRepository != null)
        {
            await _targetRepository.SaveInitialTargetAsync(dailyConsumption);
        }
    }

    public async Task<UserProfile?> GetUserProfileAsync()
    {
        return await _profileService.GetUserProfileAsync();
    }

    public bool IsSetupComplete()
    {
        return _profileService.ProfileExists();
    }

    public async Task<int?> GetInitialTargetAsync()
    {
        var profile = await GetUserProfileAsync();
        return profile?.InitialTarget;
    }

    public async Task ResetAllUserDataAsync()
    {
        await _profileService.DeleteProfileAsync();
    }

    public string GetProfileFilePath() => _profileService.GetProfileFilePath();
}
namespace Diploma_cs.Models;

public class Achievement
{
    public int AchievementID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Statement { get; set; } = string.Empty;
    public string DescriptionUnlocked { get; set; } = string.Empty;
    public string DescriptionLocked { get; set; } = string.Empty;
    public bool IsUnlocked { get; set; }
    public DateTime? UnlockedDate { get; set; }

    public string GetDescription()
    {
        return IsUnlocked ? DescriptionUnlocked : DescriptionLocked;
    }

    public string GetIconFileName()
    {
        return GetAchievementIconName(AchievementID);
    }

    public string IconFileName => GetIconFileName();

    private static string GetAchievementIconName(int achievementID)
    {
        return achievementID switch
        {
            1 => "a_1_one.svg",
            2 => "a_2_two.svg",
            3 => "a_3_three.svg",
            4 => "a_4_four.svg",
            5 => "a_5_five.svg",
            6 => "a_6_six.svg",
            7 => "a_7_seven.svg",
            8 => "a_8_eight.svg",
            9 => "a_9_nine.svg",
            10 => "a_10_ten.svg",
            11 => "a_11_eleven.svg",
            12 => "a_12_twelve.svg",
            _ => "a_1_one.svg"
        };
    }
}

public class UserAchievement
{
    public int AchievementID { get; set; }
    public DateTime UnlockedDate { get; set; }
}
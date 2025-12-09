namespace Diploma_cs.Models;

public class UserProfile
{
    public required string Name { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public int DailyConsumption { get; set; }
    public float PackPrice { get; set; }
    public DateTime SetupCompletedDate { get; set; }
    public int InitialTarget { get; set; }

    public UserProfile()
    {
    }

    public UserProfile(string name, int age, string gender, int dailyConsumption, float packPrice, int initialTarget)
    {
        Name = name;
        Age = age;
        Gender = gender;
        DailyConsumption = dailyConsumption;
        PackPrice = packPrice;
        InitialTarget = initialTarget;
        SetupCompletedDate = DateTime.Now;
    }
}
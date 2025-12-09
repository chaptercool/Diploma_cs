namespace Diploma_cs.Models;

public class SmokingSession
{
    public int Id { get; set; }
    public DateTime SessionTime { get; set; }
    public string SessionType { get; set; } = "Smoking";

    public SmokingSession()
    {
    }

    public SmokingSession(DateTime sessionTime, string sessionType = "Smoking")
    {
        SessionTime = sessionTime;
        SessionType = sessionType;
    }
}
using SQLite;

namespace workout_planner_maui.Models.IronLog;

public class WorkoutSession
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int? TemplateId { get; set; } // Nullable if ad-hoc
    public DateTime Date { get; set; }
    public DateTime? EndTime { get; set; }
    public long TotalVolume { get; set; }
}

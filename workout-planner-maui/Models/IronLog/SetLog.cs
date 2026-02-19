using SQLite;

namespace workout_planner_maui.Models.IronLog;

public class SetLog
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int SessionId { get; set; }

    [Indexed]
    public int ExerciseId { get; set; }

    public double Weight { get; set; }
    public int Reps { get; set; }
    public decimal RPE { get; set; }
    public bool IsWarmup { get; set; }
    public DateTime Timestamp { get; set; }
}

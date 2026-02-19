using SQLite;

namespace workout_planner_maui.Models.IronLog;

public class ExerciseLog
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string ExerciseName { get; set; } = string.Empty;
    public double Weight { get; set; }
    public int Reps { get; set; }
    public decimal RPE { get; set; }
    public bool IsPersonalRecord { get; set; }
    public DateTime Timestamp { get; set; }
}

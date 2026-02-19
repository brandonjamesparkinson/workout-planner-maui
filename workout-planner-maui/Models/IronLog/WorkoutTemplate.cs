using SQLite;

namespace workout_planner_maui.Models.IronLog;

public class WorkoutTemplate
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}

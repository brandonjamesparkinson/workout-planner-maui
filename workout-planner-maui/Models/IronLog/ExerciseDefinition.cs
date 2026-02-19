using SQLite;

namespace workout_planner_maui.Models.IronLog;

public class ExerciseDefinition
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string MuscleGroup { get; set; } = string.Empty;
}

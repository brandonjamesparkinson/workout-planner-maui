using SQLite;

namespace workout_planner_maui.Models.IronLog;

public class Settings
{
    [PrimaryKey]
    public string Key { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;
}

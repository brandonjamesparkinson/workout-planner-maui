using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace workout_planner_maui.Models;

public partial class Workout : ObservableObject
{
    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private bool isWarmup;

    public ObservableCollection<Exercise> Exercises { get; } = new();
}

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace workout_planner_maui.Models;

public partial class Exercise : ObservableObject
{
    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private int targetSets;

    [ObservableProperty]
    private int targetReps;

    [ObservableProperty]
    private double weight;

    public ObservableCollection<ExerciseSet> Sets { get; } = new();

    public Exercise()
    {
        
    }
}

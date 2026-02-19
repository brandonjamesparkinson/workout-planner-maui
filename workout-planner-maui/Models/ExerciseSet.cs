using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace workout_planner_maui.Models;

public partial class ExerciseSet : ObservableObject
{
    [ObservableProperty]
    private int? completedReps;

    [ObservableProperty]
    private int targetReps;

    [ObservableProperty]
    private bool isCompleted;

    // Command to cycle through states: null -> target -> target-1 ... -> 0 -> null
    [RelayCommand]
    private void CycleReps()
    {
        if (CompletedReps == null)
        {
            // Empty -> Target Reps
            CompletedReps = TargetReps;
            IsCompleted = true;
        }
        else if (CompletedReps == 0)
        {
            // Fail/0 -> Empty
            CompletedReps = null;
            IsCompleted = false;
        }
        else
        {
            // Decrementing Reps
            CompletedReps--;
            IsCompleted = true; // Even 0 is a "result", so technically completed the attempt, but let's stick to true.
        }
    }
}

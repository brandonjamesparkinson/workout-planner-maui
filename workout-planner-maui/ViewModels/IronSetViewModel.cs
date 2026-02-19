using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using workout_planner_maui.Models.IronLog;
using workout_planner_maui.Services;

namespace workout_planner_maui.ViewModels;

public partial class IronSetViewModel : ObservableObject
{
    private readonly int _targetReps;
    private readonly double _weight;
    private readonly string _exerciseName;
    private readonly DatabaseService _dbService;

    [ObservableProperty]
    private string displayValue;

    [ObservableProperty]
    private Color backgroundColor;

    [ObservableProperty]
    private Color borderColor;

    [ObservableProperty]
    private Color textColor;

    // 0: Unchecked (Target)
    // 1: Completed (Target)
    // 2: Failed (-1)
    // 3: Failed (0)
    private int _state = 0;

    public IronSetViewModel(int targetReps, double weight, string exerciseName, DatabaseService dbService)
    {
        _targetReps = targetReps;
        _weight = weight;
        _exerciseName = exerciseName;
        _dbService = dbService;
        ResetVisuals();
    }

    private void ResetVisuals()
    {
        DisplayValue = _targetReps.ToString();
        BackgroundColor = Colors.Black;
        BorderColor = Colors.White;
        TextColor = Colors.White;
    }

    [RelayCommand]
    private async Task Tap()
    {
        _state++;
        if (_state > 3) _state = 0;

        switch (_state)
        {
            case 0: // Reset
                ResetVisuals();
                break;

            case 1: // Completed (Target)
                DisplayValue = _targetReps.ToString();
                BackgroundColor = Colors.White; // "Completed: White background..."
                BorderColor = Colors.White;
                TextColor = Colors.Black;       // "...black text."

                // Haptics
                try { HapticFeedback.Default.Perform(HapticFeedbackType.LongPress); } catch { }

                // Log
                await LogSet(_targetReps);
                break;

            case 2: // Failed (-1)
                DisplayValue = "-1";
                BackgroundColor = Colors.Black;     // "Failed: Black background..."
                BorderColor = Color.FromArgb("#FF4F00"); // "...Orange border..."
                TextColor = Color.FromArgb("#FF4F00");   // "...Orange text."
                break;

            case 3: // Failed (0)
                DisplayValue = "0";
                BackgroundColor = Colors.Black;
                BorderColor = Color.FromArgb("#FF4F00");
                TextColor = Color.FromArgb("#FF4F00");
                break;
        }
    }

    private async Task LogSet(int reps)
    {
        await _dbService.SaveLogAsync(new ExerciseLog
        {
            ExerciseName = _exerciseName,
            Weight = _weight,
            Reps = reps,
            Timestamp = DateTime.UtcNow,
            // Defaults for now
            RPE = 0,
            IsPersonalRecord = false
        });
    }
}

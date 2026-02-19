using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using workout_planner_maui.Services;

namespace workout_planner_maui.ViewModels;

public partial class WorkoutViewModel : ObservableObject
{
    private readonly DatabaseService _dbService;
    private IDispatcherTimer _timer;
    private TimeSpan _remainingTime;

    public ObservableCollection<IronExerciseViewModel> Exercises { get; } = new();

    [ObservableProperty]
    private string restTimerDisplay = "00:00";

    [ObservableProperty]
    private Color restTimerColor = Colors.White;

    public WorkoutViewModel(DatabaseService dbService)
    {
        _dbService = dbService;
        LoadDummyData();
    }

    private void LoadDummyData()
    {
        // Prototype data
        Exercises.Add(new IronExerciseViewModel("SQUAT", 4, 5, 315, _dbService));
        Exercises.Add(new IronExerciseViewModel("BENCH_PRESS", 3, 8, 225, _dbService));
        Exercises.Add(new IronExerciseViewModel("DEADLIFT", 1, 5, 405, _dbService));
    }

    [RelayCommand]
    private void StartRestTimer()
    {
        // Reset to 2 mins for demo
        _remainingTime = TimeSpan.FromMinutes(2);
        UpdateTimerDisplay();

        if (_timer == null)
        {
            _timer = Application.Current.Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) =>
            {
                _remainingTime = _remainingTime.Subtract(TimeSpan.FromSeconds(1));
                if (_remainingTime.TotalSeconds <= 0)
                {
                    _remainingTime = TimeSpan.Zero;
                    _timer.Stop();
                    // Flash Orange
                    RestTimerColor = Color.FromArgb("#FF4F00");
                }
                UpdateTimerDisplay();
            };
        }
        else
        {
            _timer.Stop(); // Restart if running
        }
        
        RestTimerColor = Colors.White;
        _timer.Start();
    }

    private void UpdateTimerDisplay()
    {
        RestTimerDisplay = _remainingTime.ToString(@"mm\:ss");
    }
}

public class IronExerciseViewModel : ObservableObject
{
    public string Name { get; }
    public ObservableCollection<IronSetViewModel> Sets { get; } = new();

    public IronExerciseViewModel(string name, int setCheckboxes, int targetReps, double weight, DatabaseService dbService)
    {
        Name = name;
        for (int i = 0; i < setCheckboxes; i++)
        {
            Sets.Add(new IronSetViewModel(targetReps, weight, name, dbService));
        }
    }
}

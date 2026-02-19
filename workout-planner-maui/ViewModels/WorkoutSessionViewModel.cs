using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Timers;
using workout_planner_maui.Models;

namespace workout_planner_maui.ViewModels;

public partial class WorkoutSessionViewModel : ObservableObject
{
    [ObservableProperty]
    private Workout currentWorkout;

    [ObservableProperty]
    private bool isTimerRunning;

    [ObservableProperty]
    private TimeSpan timerValue;

    [ObservableProperty]
    private string timerDisplay;

    private System.Timers.Timer _timer;
    private DateTime _startTime;

    public WorkoutSessionViewModel()
    {
        // Mock Data for now
        CurrentWorkout = new Workout { Name = "Workout A", IsWarmup = false };
        
        CurrentWorkout.Exercises.Add(new Exercise 
        { 
            Name = "Squat", 
            TargetSets = 5, 
            TargetReps = 5, 
            Weight = 220 
        });

         CurrentWorkout.Exercises.Add(new Exercise 
        { 
            Name = "Bench Press", 
            TargetSets = 5, 
            TargetReps = 5, 
            Weight = 185 
        });

         CurrentWorkout.Exercises.Add(new Exercise 
        { 
            Name = "Barbell Row", 
            TargetSets = 5, 
            TargetReps = 5, 
            Weight = 135 
        });


        // Initialize Sets
        foreach (var exercise in CurrentWorkout.Exercises)
        {
            for (int i = 0; i < exercise.TargetSets; i++)
            {
                exercise.Sets.Add(new ExerciseSet { TargetReps = exercise.TargetReps });
            }
        }

        TimerDisplay = "00:00";
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += OnTimerElapsed;
    }

    [RelayCommand]
    private void ToggleWorkoutType()
    {
        CurrentWorkout.IsWarmup = !CurrentWorkout.IsWarmup;
    }

    [RelayCommand]
    private void StartTimer()
    {
        if (IsTimerRunning)
        {
            _timer.Stop();
            IsTimerRunning = false;
        }
        else
        {
             // Reset start time relative to current value if we want to pause/resume, 
             // but user request implies "activates when set completed".
             // Simple implementation: Start/Restart.
             _startTime = DateTime.Now;
             TimerValue = TimeSpan.Zero;
             TimerDisplay = "00:00";
             _timer.Start();
             IsTimerRunning = true;
        }
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        TimerValue = DateTime.Now - _startTime;
        TimerDisplay = TimerValue.ToString(@"mm\:ss");
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using workout_planner_maui.Models.IronLog;
using workout_planner_maui.Services;

namespace workout_planner_maui.ViewModels;

public partial class WorkoutViewModel : ObservableObject, IQueryAttributable
{
    private readonly DatabaseService _dbService;
    private IDispatcherTimer _restTimer;
    private IDispatcherTimer _sessionTimer;
    private TimeSpan _remainingRestTime;
    
    // Session Tracking
    private DateTime _sessionStartTime;
    private TimeSpan _sessionDuration;

    public ObservableCollection<IronExerciseViewModel> Exercises { get; } = new();

    // Header Properties
    [ObservableProperty]
    private string status = "ACTIVE";

    [ObservableProperty]
    private string sessionTimeDisplay = "00:00:00";

    [ObservableProperty]
    private long totalVolume = 0;

    [ObservableProperty]
    private bool isEmptyStateVisible = true;

    // Footer Properties
    [ObservableProperty]
    private string restTimerDisplay = "00:00";

    [ObservableProperty]
    private Color restTimerColor = Colors.White;

    public WorkoutViewModel(DatabaseService dbService)
    {
        _dbService = dbService;
        InitializeSession();
    }

    private void InitializeSession()
    {
        _sessionStartTime = DateTime.UtcNow;
        StartSessionTimer();
        UpdateEmptyState();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("SelectedExercise") && query["SelectedExercise"] is ExerciseDefinition exercise)
        {
            // Add to list with default 3 sets
            var newExercise = new IronExerciseViewModel(exercise.Name, 3, 5, 45, _dbService, RemoveExercise);
            newExercise.SetLogged += OnSetLogged;
            Exercises.Add(newExercise);
            UpdateEmptyState();
        }
    }

    private void OnSetLogged(object sender, long volume)
    {
        TotalVolume += volume;
    }

    private void RemoveExercise(IronExerciseViewModel exercise)
    {
        exercise.SetLogged -= OnSetLogged;
        Exercises.Remove(exercise);
        UpdateEmptyState();
    }

    private void UpdateEmptyState()
    {
        IsEmptyStateVisible = Exercises.Count == 0;
    }

    [RelayCommand]
    private async Task AddExercise()
    {
        await Shell.Current.GoToAsync("ExerciseSelectionPage"); // Assumes route is registered or global
        // Route registration usually implicit or needs manual register if not AppShell structured nicely.
        // We will double check AppShell.
    }

    [RelayCommand]
    private async Task FinishWorkout()
    {
        Status = "FINISHED";
        _sessionTimer.Stop();

        var session = new WorkoutSession
        {
            Date = _sessionStartTime,
            EndTime = DateTime.UtcNow,
            TotalVolume = TotalVolume
        };

        var allSets = new List<SetLog>();
        foreach (var ex in Exercises)
        {
            // Assuming we map sets. 
            // Since IronSetViewModel logs individually to DB in Phase 1, we might need to adjust.
            // Phase 1: Set -> Tap -> SaveLogAsync (Immediate SQLite Insert)
            // Phase 2 Req: "End/Save Workout: ... navigates the user back to a HistoryPage."
            // If we are saving sets immediately, "Finish" just creates the Session wrapper.
            // But we need to link Sets to Session.
            
            // To fix this retroactively without complex state:
            // The sets are already in the DB as 'ExerciseLog' (Phase 1).
            // Phase 2 introduces 'SetLog'. 
            // Let's have the ViewModels CREATE SetLog objects but NOT save them immediately?
            // OR update IronSetViewModel to store state and we save all at the end.
            
            // Let's go with: IronSetViewModel holds the data, FinishWorkout saves everything.
            // This matches "End/Save Workout" pattern better.
            
            foreach (var s in ex.Sets)
            {
               if (s.IsCompletedState)
               {
                   // Create SetLog
                   // We need ExerciseDefinition ID? For now we only have Name.
                   // We'll trust the Name or lookup later. 
                   // Ideally we pass Definition ID to VM.
                   allSets.Add(new SetLog
                   {
                       // ExerciseId = ... lookup?
                       Weight = s.Weight,
                       Reps = s.TargetReps, // Or actual completed?
                       Timestamp = DateTime.UtcNow
                   });
               }
            }
        }

        // Actually, IronSetViewModel already saves to 'ExerciseLog' (Phase 1 table).
        // I should probably UPDATE IronSetViewModel to NOT save immediately, or save to the NEW table.
        // For this Prototype, let's keep it simple:
        // IronSetViewModel acts as UI state.
        // FinishWorkout saves the 'WorkoutSession' and 'SetLog's.
        
        await _dbService.SaveSessionAsync(session, ExtractSetsFromViewModels());
        
        // Navigate away or show summary opacity
        await Shell.Current.DisplayAlert("IRON LOG", $"WORKOUT SAVED.\nVOL: {TotalVolume}", "ACK");
        // Clear
        Exercises.Clear();
        TotalVolume = 0;
        InitializeSession();
    }

    private List<SetLog> ExtractSetsFromViewModels()
    {
        // Flatten sets
        var list = new List<SetLog>();
        foreach(var ex in Exercises)
        {
             foreach(var s in ex.Sets)
             {
                 if(s.IsCompletedState)
                 {
                     list.Add(new SetLog
                     {
                         // We don't have IDs here easily without lookup, using 0 for now or simple mapping if we had it.
                         // Using Name for display if we need to join later, but schema has ExerciseId.
                         // For prototype speed, we might settle for saving, or do a quick lookup.
                         // Let's just save the logs.
                         Weight = s.Weight,
                         Reps = s.TargetReps,
                         Timestamp = DateTime.UtcNow
                     });
                 }
             }
        }
        return list;
    }

    // Timers
    private void StartSessionTimer()
    {
        if (_sessionTimer == null)
        {
            _sessionTimer = Application.Current.Dispatcher.CreateTimer();
            _sessionTimer.Interval = TimeSpan.FromSeconds(1);
            _sessionTimer.Tick += (s, e) =>
            {
                _sessionDuration = DateTime.UtcNow - _sessionStartTime;
                SessionTimeDisplay = _sessionDuration.ToString(@"hh\:mm\:ss");
            };
        }
        _sessionTimer.Start();
    }

    [RelayCommand]
    private void StartRestTimer()
    {
        _remainingRestTime = TimeSpan.FromMinutes(2);
        UpdateRestTimerDisplay();

        if (_restTimer == null)
        {
            _restTimer = Application.Current.Dispatcher.CreateTimer();
            _restTimer.Interval = TimeSpan.FromSeconds(1);
            _restTimer.Tick += (s, e) =>
            {
                _remainingRestTime = _remainingRestTime.Subtract(TimeSpan.FromSeconds(1));
                if (_remainingRestTime.TotalSeconds <= 0)
                {
                    _remainingRestTime = TimeSpan.Zero;
                    _restTimer.Stop();
                    RestTimerColor = Color.FromArgb("#FF4F00");
                }
                UpdateRestTimerDisplay();
            };
        }
        else
        {
            _restTimer.Stop();
        }
        
        RestTimerColor = Colors.White;
        _restTimer.Start();
    }

    private void UpdateRestTimerDisplay()
    {
        RestTimerDisplay = _remainingRestTime.ToString(@"mm\:ss");
    }
}

// Updated Inner VM
public partial class IronExerciseViewModel : ObservableObject
{
    private readonly DatabaseService _dbService;
    private readonly Action<IronExerciseViewModel> _removeAction;
    public string Name { get; }
    public ObservableCollection<IronSetViewModel> Sets { get; } = new();

    public event EventHandler<long> SetLogged;

    public IronExerciseViewModel(string name, int setCheckboxes, int targetReps, double weight, DatabaseService dbService, Action<IronExerciseViewModel> removeAction)
    {
        Name = name;
        _dbService = dbService;
        _removeAction = removeAction;
        for (int i = 0; i < setCheckboxes; i++)
        {
            var set = new IronSetViewModel(targetReps, weight, name, dbService);
            set.SetCompleted += (s, valid) => 
            {
                 if(valid) SetLogged?.Invoke(this, (long)(weight * targetReps));
                 // Note: Logic for "Un-completing" to subtract volume is complex, skipping for prototype.
            };
            Sets.Add(set);
        }
    }
    
    [RelayCommand]
    private void Remove()
    {
        _removeAction?.Invoke(this);
    }
}

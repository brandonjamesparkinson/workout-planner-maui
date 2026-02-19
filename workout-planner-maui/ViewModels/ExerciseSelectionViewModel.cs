using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using workout_planner_maui.Models.IronLog;
using workout_planner_maui.Services;

namespace workout_planner_maui.ViewModels;

public partial class ExerciseSelectionViewModel : ObservableObject
{
    private readonly DatabaseService _dbService;

    public ObservableCollection<ExerciseDefinition> Exercises { get; } = new();

    public ExerciseSelectionViewModel(DatabaseService dbService)
    {
        _dbService = dbService;
        LoadExercisesCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadExercises()
    {
        Exercises.Clear();
        var list = await _dbService.GetExercisesAsync();
        foreach (var item in list)
        {
            Exercises.Add(item);
        }
    }

    [RelayCommand]
    private async Task SelectExercise(ExerciseDefinition exercise)
    {
        // Return result to previous page via Navigation parameters or MessagingCenter?
        // Simpler for modal: Dictionary parameter in GoBack, or use a weakness of Maui Shell Modals.
        // Let's use the standard Shell navigation with parameters.
        
        await Shell.Current.GoToAsync("..", new Dictionary<string, object>
        {
            { "SelectedExercise", exercise }
        });
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}

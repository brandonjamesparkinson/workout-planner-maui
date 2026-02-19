using workout_planner_maui.Models;
using workout_planner_maui.ViewModels;
using Microsoft.Maui.Devices;

namespace workout_planner_maui.Views;

public partial class WorkoutSessionPage : ContentPage
{
	public WorkoutSessionPage()
	{
		InitializeComponent();
		BindingContext = new WorkoutSessionViewModel();
	}

    private async void OnRepTapped(object sender, EventArgs e)
    {
        if (sender is VisualElement element && element.BindingContext is ExerciseSet set)
        {
            // Micro-Interaction: Scale Animation
            await element.ScaleTo(0.95, 100, Easing.CubicOut);
            await element.ScaleTo(1.0, 100, Easing.CubicIn);

            // Haptic Feedback
            try 
            {
                HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            }
            catch (Exception)
            {
                // Ignore if not supported
            }

            // Execute Logic
            if (set.CycleRepsCommand.CanExecute(null))
            {
                set.CycleRepsCommand.Execute(null);
            }
        }
    }
}

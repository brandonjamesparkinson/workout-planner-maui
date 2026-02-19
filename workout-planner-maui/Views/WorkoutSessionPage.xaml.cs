using workout_planner_maui.ViewModels;

namespace workout_planner_maui.Views;

public partial class WorkoutSessionPage : ContentPage
{
	public WorkoutSessionPage()
	{
		InitializeComponent();
		BindingContext = new WorkoutSessionViewModel();
	}
}

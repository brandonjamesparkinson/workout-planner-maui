using workout_planner_maui.ViewModels;

namespace workout_planner_maui.Views;

public partial class WorkoutPage : ContentPage
{
	public WorkoutPage(WorkoutViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}

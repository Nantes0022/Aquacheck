namespace aquacheckProject;

public partial class WaterControl : ContentPage
{
    public WaterControl()
    {
        InitializeComponent();
    }


    private void returnHomeTemp(object sender, TappedEventArgs e)
    {

        Shell.Current.GoToAsync("..");
    }

    private async void DrainWater(object sender, TappedEventArgs e)
    {
        await Navigation.PushModalAsync(new DrainWater(), false);
    }
    private async void FillWater(object sender, TappedEventArgs e)
    {
        await Navigation.PushModalAsync(new FillWater(), false);
    }
    private async void Ventilation(object sender, TappedEventArgs e)
    {
        await Navigation.PushModalAsync(new Ventilation(), false);
    }
}
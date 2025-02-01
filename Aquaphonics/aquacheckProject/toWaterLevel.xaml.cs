namespace aquacheckProject;

public partial class toWaterLevel : ContentPage
{
	public toWaterLevel()
	{
		InitializeComponent();
	}

    private void toHome(object sender, TappedEventArgs e)
    {
        App.Current.MainPage = new HomePage();
    }
}
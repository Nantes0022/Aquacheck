namespace aquacheckProject;

public partial class Instructions : ContentPage
{
	public Instructions()
	{
		InitializeComponent();
	}

    private void returnTapped(object sender, TappedEventArgs e)
    {
        App.Current.MainPage = new HomePage();
    }
}
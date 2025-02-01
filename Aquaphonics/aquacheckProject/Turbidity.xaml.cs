using aquacheckProject.GatherData;
using System.Timers;
using Timer = System.Timers.Timer;

namespace aquacheckProject;

public partial class Turbidity : ContentPage
{
    private CancellationTokenSource _cancellationTokenSource;
    private Image turbidImage;
    public sensorData sensorData = new sensorData();
    private Timer _timer;
    public Turbidity()
	{
		InitializeComponent();
        _timer = new Timer(1000);
        _timer.Elapsed += feedEvent;
        _timer.AutoReset = true;
        _timer.Enabled = true;
    }

    private void feedEvent(object source, ElapsedEventArgs e)
    {
        // Ensure UI updates are done on the main thread
        MainThread.BeginInvokeOnMainThread(() =>
        {
            sensorData.data();
            turbidityCard();
        });
    }


    private void returnHome(object sender, TappedEventArgs e)
    {
        Shell.Current.GoToAsync("..");
    }


    
    public void turbidityCard()
    {
        if (sensorData.Turbid > 50)
        {
            turbidImageObj.Children.Remove(turbidImage);
            turbidImage = new Image
            {
                Source = "water_pollution3.png", // Replace with your image file name or URI
                WidthRequest = 150, // Set the desired width
                Margin = new Thickness(0, 60, 0, 0) // Set the desired margin
            };

            // Add the Image to the StackLayout
            turbidImageObj.Children.Add(turbidImage);
            Turbid.Text = sensorData.Turbid.ToString() + " NTU";
            Turbid.TextColor = Color.FromArgb("#FF0000");
            TurbidWarningBold.Text = "The Aquarium's Turbidity is High";
            TurbidWarningBold.TextColor = Color.FromArgb("#FF0000");
            TurbidWarning.Text = "It Requires immediate Filtration.";
            TurbidWarning.TextColor = Color.FromArgb("#FF0000");
        }
        else if (sensorData.Turbid > 40)
        {
            turbidImageObj.Children.Remove(turbidImage);
            turbidImage = new Image
            {
                Source = "water_pollution2.png", // Replace with your image file name or URI
                WidthRequest = 150, // Set the desired width
                Margin = new Thickness(0, 60, 0, 0) // Set the desired margin
            };

            // Add the Image to the StackLayout
            turbidImageObj.Children.Add(turbidImage);

            Turbid.Text = sensorData.Turbid.ToString() + " NTU";
            Turbid.TextColor = Color.FromArgb("#ee8d1d");
            TurbidWarningBold.Text = "Warning.";
            TurbidWarningBold.TextColor = Color.FromArgb("#ee8d1d");
            TurbidWarning.Text = "The Turbidity will exceed the normal range..";
            TurbidWarning.TextColor = Color.FromArgb("#ee8d1d");
        }
        else
        {
            turbidImageObj.Children.Remove(turbidImage);
            turbidImage = new Image
            {
                Source = "water_pollution1.png", // Replace with your image file name or URI
                WidthRequest = 150, // Set the desired width
                Margin = new Thickness(0, 60, 0, 0) // Set the desired margin
            };

            // Add the Image to the StackLayout
            turbidImageObj.Children.Add(turbidImage);
            Turbid.Text = sensorData.Turbid.ToString() + " NTU";
            Turbid.TextColor = Color.FromArgb("#008000");
            TurbidWarningBold.Text = "The Aquarium's temperature is Normal";
            TurbidWarningBold.TextColor = Color.FromArgb("#008000");
            TurbidWarning.Text = "The water is ideal to the fish.";
            TurbidWarning.TextColor = Color.FromArgb("#008000");
        }
    }
}
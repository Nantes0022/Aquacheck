using aquacheckProject.GatherData;
using System.Timers;
using Timer = System.Timers.Timer;

namespace aquacheckProject;

public partial class Temperature : ContentPage
{
    public sensorData sensorData = new sensorData();
    private Timer _timer;
    public Temperature()
	{
		InitializeComponent();
        _timer = new Timer(1000);
        _timer.Elapsed += feedEvent;
        _timer.AutoReset = true;
        _timer.Enabled = true;
    }

    private void returnHomeTemp(object sender, TappedEventArgs e)
    {

        Shell.Current.GoToAsync("..");
    }

    private void feedEvent(object source, ElapsedEventArgs e)
    {
        // Ensure UI updates are done on the main thread
        MainThread.BeginInvokeOnMainThread(() =>
        {
            sensorData.data();
            tempCard();
        });
    }

    private Image tempImage;
    public void tempCard()
    {
        if (sensorData.Temperature <= 24)
        {
            tempImageObj.Children.Remove(tempImage);
            tempImage = new Image
            {
                Source = "temperatures_cold.png", // Replace with your image file name or URI
                WidthRequest = 150, // Set the desired width
                Margin = new Thickness(0, 60, 0, 0) // Set the desired margin
            };

            // Add the Image to the StackLayout
            tempImageObj.Children.Add(tempImage);
            tempRes.Text = sensorData.Temperature.ToString() + "°C";
            tempRes.TextColor = Color.FromArgb("#FF0000");
            tempWarningBold.Text = "The Aquarium's temperature is Low";
            tempWarningBold.TextColor = Color.FromArgb("#FF0000");
            tempWarning.Text = "The water is Cold it requires immediate action.";
            tempWarning.TextColor = Color.FromArgb("#FF0000");
        }
        else if (sensorData.Temperature >= 28)
        {
            tempImageObj.Children.Remove(tempImage);
            tempImage = new Image
            {
                Source = "temperatures_hot.png", // Replace with your image file name or URI
                WidthRequest = 150,
                Margin = new Thickness(0, 60, 0, 0) // Set the desired margin
            };

            // Add the Image to the StackLayout
            tempImageObj.Children.Add(tempImage);
            tempRes.Text = sensorData.Temperature.ToString() + "°C";
            tempRes.TextColor = Color.FromArgb("#FF0000");
            tempWarningBold.Text = "The Aquarium's temperature is High";
            tempWarningBold.TextColor = Color.FromArgb("#FF0000");
            tempWarning.Text = "The water is Hot it requires immediate action.";
            tempWarning.TextColor = Color.FromArgb("#FF0000");
        }
        else
        {
            tempImageObj.Children.Remove(tempImage);
            tempImage = new Image
            {
                Source = "temperatures_normal.png", // Replace with your image file name or URI
                WidthRequest = 150,
                Margin = new Thickness(0, 60, 0, 0) // Set the desired margin
            };

            // Add the Image to the StackLayout
            tempImageObj.Children.Add(tempImage);
            tempRes.Text = sensorData.Temperature.ToString() + "°C";
            tempRes.TextColor = Color.FromArgb("c");
            tempWarningBold.Text = "The Aquarium's temperature is Normal";
            tempWarningBold.TextColor = Color.FromArgb("#008000");
            tempWarning.Text = "The water is ideal to the fish.";
            tempWarning.TextColor = Color.FromArgb("#008000");
        }
    }
}
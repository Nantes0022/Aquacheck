using aquacheckProject.GatherData;
using System.Timers;
using Timer = System.Timers.Timer;

namespace aquacheckProject;

public partial class phLevel : ContentPage
{
    private CancellationTokenSource _cancellationTokenSource;
    private Image pHImage;
    public sensorData sensorData = new sensorData();
    private Timer _timer;
    public phLevel()
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
            pHCard();
        });
    }
    private void returnTapped(object sender, TappedEventArgs e)
    {
        Shell.Current.GoToAsync("..");
    }


    public void pHCard()
    {
        if (sensorData.pH < 7.0 || sensorData.pH > 8.4)
        {
            pHImageObj.Children.Remove(pHImage);
            pHImage = new Image
            {
                Source = "water_drop3.png", // Replace with your image file name or URI
                WidthRequest = 150, // Set the desired width
                Margin = new Thickness(0, 60, 0, 0) // Set the desired margin
            };

            // Add the Image to the StackLayout
            pHImageObj.Children.Add(pHImage);
            pH.Text = sensorData.pH.ToString();
            pH.TextColor = Color.FromArgb("#FF0000");
            pHWarningBold.Text = "The Aquarium's pH is High";
            pHWarningBold.TextColor = Color.FromArgb("#FF0000");
            pHWarning.Text = "The water is acidic it requires immediate action.";
            pHWarning.TextColor = Color.FromArgb("#FF0000");
        }
        else if (sensorData.pH >= 8.0 && sensorData.pH <= 8.4)
        {
            pHImageObj.Children.Remove(pHImage);
            pHImage = new Image
            {
                Source = "water_drop2.png", // Replace with your image file name or URI
                WidthRequest = 150, // Set the desired width
                Margin = new Thickness(0, 60, 0, 0) // Set the desired margin
            };

            // Add the Image to the StackLayout
            pHImageObj.Children.Add(pHImage);
            pH.Text = sensorData.pH.ToString();
            pH.TextColor = Color.FromArgb("#ee8d1d");
            pHWarningBold.Text = "Warning.";
            pHWarningBold.TextColor = Color.FromArgb("#ee8d1d");
            pHWarning.Text = "The pH level will exceed the normal range.";
            pHWarning.TextColor = Color.FromArgb("#ee8d1d");
        }
        else if (sensorData.pH >= 7.0 && sensorData.pH <= 7.4)
        {
            pHImageObj.Children.Remove(pHImage);
            pHImage = new Image
            {
                Source = "water_drop2.png", // Replace with your image file name or URI
                WidthRequest = 150, // Set the desired width
                Margin = new Thickness(0, 60, 0, 0) // Set the desired margin
            };

            // Add the Image to the StackLayout
            pHImageObj.Children.Add(pHImage);
            pH.Text = sensorData.pH.ToString();
            pH.TextColor = Color.FromArgb("#ee8d1d");
            pHWarningBold.Text = "Warning.";
            pHWarningBold.TextColor = Color.FromArgb("#ee8d1d");
            pHWarning.Text = "The pH level will exceed the normal range.";
            pHWarning.TextColor = Color.FromArgb("#ee8d1d");
        }
        else
        {
            pHImageObj.Children.Remove(pHImage);
            pHImage = new Image
            {
                Source = "water_drop1.png", // Replace with your image file name or URI
                WidthRequest = 150, // Set the desired width
                Margin = new Thickness(0, 60, 0, 0) // Set the desired margin
            };

            // Add the Image to the StackLayout
            pHImageObj.Children.Add(pHImage);
            pH.Text = sensorData.pH.ToString();
            pH.TextColor = Color.FromArgb("#008000");
            pHWarningBold.Text = "The Aquarium's pH is Normal";
            pHWarningBold.TextColor = Color.FromArgb("#008000");
            pHWarning.Text = "Ideal for the Fishes.";
            pHWarning.TextColor = Color.FromArgb("#008000");
        }
    }
}
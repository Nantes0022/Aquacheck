using aquacheckProject.GatherData;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace aquacheckProject;

public partial class tdsLevel : ContentPage
{

    private static readonly HttpClient client = new HttpClient();
    private CancellationTokenSource _cancellationTokenSource;
    public sensorData sensorData = new sensorData();
    private Timer _timer;
    public tdsLevel()
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
            displayData();
        });
    }
    private void toReturn(object sender, TappedEventArgs e)
    {
        Shell.Current.GoToAsync("..");
    }
    private Image tdsImage;



    private void displayData() {
        if (sensorData.TDS <= 150)
        {
            tdsImageObj.Children.Remove(tdsImage);
            tdsImage = new Image
            {
                Source = "dissolving1.png", // Replace with your image file name or URI
                WidthRequest = 150, // Set the desired width
                Margin = new Thickness(0, 60, 0, 0) // Set the desired margin
            };
            TDS.Text = sensorData.TDS.ToString() + " ppm";
            TDS.TextColor = Color.FromArgb("#008000");
            tdsWarningBold.Text = "The Aquarium's TDS level is Normal";
            tdsWarningBold.TextColor = Color.FromArgb("#008000");
            tdsWarning.Text = "Ideal for the Fishes.";
            tdsWarning.TextColor = Color.FromArgb("#008000");

            // Add the Image to the StackLayout
            tdsImageObj.Children.Add(tdsImage);
        }
        else if (sensorData.TDS <= 300)
        {
            tdsImageObj.Children.Remove(tdsImage);
            tdsImage = new Image
            {
                Source = "dissolving2.png", // Replace with your image file name or URI
                WidthRequest = 150, // Set the desired width
                Margin = new Thickness(0, 60, 0, 0) // Set the desired margin
            };

            TDS.Text = sensorData.TDS.ToString() + " ppm";
            TDS.TextColor = Color.FromArgb("#ee8d1d");
            tdsWarningBold.Text = "Warning.";
            tdsWarningBold.TextColor = Color.FromArgb("#ee8d1d");
            tdsWarning.Text = "The TDS level will exceed the normal range.";
            tdsWarning.TextColor = Color.FromArgb("#ee8d1d");
            // Add the Image to the StackLayout
            tdsImageObj.Children.Add(tdsImage);
        }
        else
        {
            tdsImageObj.Children.Remove(tdsImage);
            tdsImage = new Image
            {
                Source = "dissolving3.png", // Replace with your image file name or URI
                WidthRequest = 150, // Set the desired width
                Margin = new Thickness(0, 60, 0, 0) // Set the desired margin
            };

            TDS.Text = sensorData.TDS.ToString() + " ppm";
            TDS.TextColor = Color.FromArgb("#FF0000");
            tdsWarningBold.Text = "The Aquarium's TDS level is High, ";
            tdsWarningBold.TextColor = Color.FromArgb("#FF0000");
            tdsWarning.Text = "Potentially Stressing the Fish. Immediate Monitoring and Water Adjustment are Needed.";
            tdsWarning.TextColor = Color.FromArgb("#FF0000");
            // Add the Image to the StackLayout
            tdsImageObj.Children.Add(tdsImage);
        }
    }
}
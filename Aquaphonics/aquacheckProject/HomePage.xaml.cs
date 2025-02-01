using System.Net.Http;
using System.Threading.Tasks;
using aquacheckProject.GatherData;
using Microsoft.Maui.Controls;
using System.Text.Json;
using Timer = System.Timers.Timer;
using System.Diagnostics.Metrics;
using System.Timers;
using System.Text.RegularExpressions;
using Plugin.Maui.Calendar;
using Plugin.LocalNotification;
using System;
using System.ComponentModel;

namespace aquacheckProject;

public partial class HomePage : ContentPage
{
    public sensorData sensorData = new sensorData();
    private Timer _timer;
    public feedData feedData = new feedData();
    Dictionary<string, feedData> schedules;
    string currentMonth = DateTime.Now.ToString("MMMM");
    int currentYear = DateTime.Now.Year;
    public string[] timeFeed = { };
    public HomePage()
    {
        InitializeComponent();
        _timer = new Timer(500);
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
            displayValue();
            tdsCard();
        });
    }



    private async void toWaterControl(object sender, TappedEventArgs e)
    {
        await Navigation.PushModalAsync(new WaterControl(), false);
    }
    private async void Temp_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushModalAsync(new Temperature(), false);
    }
    private async void Water_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushModalAsync(new toWaterLevel(), false);
    }
    private async void tds_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushModalAsync(new tdsLevel(), false);
    }
    private async void pH_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushModalAsync(new phLevel(), false);
    }
    private async void Turbidity_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushModalAsync(new Turbidity(), false);
    }

    private async void feederTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushModalAsync(new toFeed(), false);
    }
    private async void Instruction_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushModalAsync(new Instructions(), false);
    }

    private void displayValue()
    {
        CardsResponsiveUI();
    }

    private Image tdsImage;
    private Image tempImage;
    private Image pHImage;
    private Image turbidImage;

    private void CardsResponsiveUI()
    {
        tempCard();
        tdsCard();
        pHCard();
        turbidityCard();
    }

    public async Task getNearestTime()
    {
        using HttpClient client = new HttpClient();
        string url = "https://aquacheck.site/feeding/feedAndroidAPI.php?api_key=iu311DXlOK42kGrBNHzPhQ8X8ZrmXXvbrRR8sbUz"; // Replace with your desired URL

        // Send a GET request to the specified URL
        HttpResponseMessage response = await client.GetAsync(url);

        // Ensure the request was successful
        response.EnsureSuccessStatusCode();

        string feedData = await response.Content.ReadAsStringAsync();

        schedules = JsonSerializer.Deserialize<Dictionary<string, feedData>>(feedData);

        DateTime now = DateTime.Now;
        string nowFormatted = now.ToString("HH:mm"); // Format now as HH:mm

        // List to hold all times in DateTime format
        List<DateTime> allTimes = new List<DateTime>();

        // Iterate through the schedules and collect all times for today
        foreach (var schedule in schedules)
        {
            if (schedule.Value.date == now.ToString("dd-MM-yyyy"))
            {
                foreach (var time in schedule.Value.times)
                {
                    // Parse the time string in HH:mm format
                    if (DateTime.TryParseExact(time, "HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime parsedTime))
                    {
                        // Combine today's date with the parsed time
                        DateTime fullTime = new DateTime(now.Year, now.Month, now.Day, parsedTime.Hour, parsedTime.Minute, 0);
                        allTimes.Add(fullTime);
                    }
                }
            }
        }

        // Get the nearest upcoming time
        DateTime? nearestTime = allTimes
            .Where(t => t > now) // Filter times that are in the future
            .OrderBy(t => t)     // Order by the closest time
            .FirstOrDefault();   // Get the nearest time

        // Display the result
        if (nearestTime.HasValue)
        {
            interval.Text = $"Today: {nearestTime.Value.ToString("hh:mm tt")}";
        }
        else
        {
            interval.Text = "Today: No upcoming feed.";
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await getNearestTime();
    }


    private double _lastTdsValue = -1;  // Initialize to an invalid value
    private int _notificationId = 1000;  // Starting notification ID
    private bool _isInitialized = false;


    public void tdsCard()
    {
        // Update the TDS image and text regardless of whether a notification is sent
        UpdateTdsImage(
            sensorData.TDS <= 150 ? "dissolving1.png" :
            sensorData.TDS <= 300 ? "dissolving2.png" :
            "dissolving3.png",
            sensorData.TDS <= 150 ? "Normal" :
            sensorData.TDS <= 300 ? "Moderate" :
            "High",
            null // No notification initially
        );

        // Skip further processing if the TDS value hasn't changed significantly
        if (Math.Abs(sensorData.TDS - _lastTdsValue) < 0.01)
        {
            return;
        }

        if (!_isInitialized)
        {
            if (sensorData.TDS > 0) // Assuming valid TDS values are > 0
            {
                _isInitialized = true; // Mark as initialized
                _lastTdsValue = sensorData.TDS; // Set the baseline TDS value
            }
            return; // Skip notifications during initialization
        }

        _lastTdsValue = sensorData.TDS; // Update the last TDS value
        _notificationId++; // Increment the notification ID for unique notifications

        NotificationRequest? request = null; // Initialize request as null

        if (sensorData.TDS > 150)
        {
            request = new NotificationRequest
            {
                NotificationId = _notificationId,
                Title = "AquaCheck",
                BadgeNumber = 1,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddMilliseconds(100)
                }
            };

            request.Description = $"The Aquarium's TDS level is {(sensorData.TDS <= 300 ? "Moderate" : "High")}";
            LocalNotificationCenter.Current.Show(request);
        }
    }

    private void UpdateTdsImage(string imageSource, string description, NotificationRequest? request)
    {
        tdsImageObj.Children.Remove(tdsImage);
        tdsImage = new Image
        {
            Source = imageSource,
            WidthRequest = 40,
            Margin = new Thickness(-55, 5, 0, 0)
        };
        tdsImageObj.Children.Add(tdsImage);
        TDS.Text = sensorData.TDS.ToString() + " ppm";
    }

    private double _lastTempValue = -1; // Initialize to an invalid value
    private bool _isTempInitialized = false; // Track initialization state

    public void tempCard()
    {
        // Update the temperature image and text regardless of whether a notification is sent
        UpdateTempImage(
            sensorData.Temperature <= 24 ? "temperatures_cold.png" :
            sensorData.Temperature >= 28 ? "temperatures_hot.png" :
            "temperatures_normal.png",
            null // No notification initially
        );

        // Skip further processing if the temperature value hasn't changed significantly
        if (Math.Abs(sensorData.Temperature - _lastTempValue) < 0.01)
        {
            return;
        }

        if (!_isTempInitialized)
        {
            if (sensorData.Temperature > 0) // Assuming valid temperature values are > 0
            {
                _isTempInitialized = true; // Mark as initialized
                _lastTempValue = sensorData.Temperature; // Set the baseline temperature value
            }
            return; // Skip notifications during initialization
        }

        _lastTempValue = sensorData.Temperature; // Update the last temperature value
        _notificationId++; // Increment the notification ID for unique notifications

        NotificationRequest? request = null; // Initialize request as null

        if (sensorData.Temperature <= 24)
        {
            request = new NotificationRequest
            {
                NotificationId = _notificationId,
                Title = "AquaCheck",
                Description = "The Aquarium's temperature is Low",
                BadgeNumber = 1,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddMilliseconds(100)
                }
            };
            LocalNotificationCenter.Current.Show(request);
        }
        else if (sensorData.Temperature >= 28)
        {
            request = new NotificationRequest
            {
                NotificationId = _notificationId,
                Title = "AquaCheck",
                Description = "The Aquarium's temperature is High",
                BadgeNumber = 1,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddMilliseconds(100)
                }
            };
            LocalNotificationCenter.Current.Show(request);
        }
    }

    private void UpdateTempImage(string imageSource, NotificationRequest? request)
    {
        tempImageObj.Children.Remove(tempImage);
        tempImage = new Image
        {
            Source = imageSource,
            WidthRequest = 40,
            Margin = new Thickness(-53, 5, 0, 0)
        };
        tempImageObj.Children.Add(tempImage);
        tempRes.Text = sensorData.Temperature.ToString() + " C";
    }


    private double _lastPhValue = -1; // Initialize to an invalid value
    private bool _isPhInitialized = false; // Track initialization state

    public void pHCard()
    {
        // Update the pH image and text regardless of whether a notification is sent
        UpdatePhImage(
            sensorData.pH < 7.0 || sensorData.pH > 8.4 ? "water_drop3.png" :
            sensorData.pH >= 8.0 && sensorData.pH <= 8.4 ? "water_drop2.png" :
            sensorData.pH >= 7.0 && sensorData.pH <= 7.4 ? "water_drop2.png" :
            "water_drop1.png",
            null // No notification initially
        );

        // Skip further processing if the pH value hasn't changed significantly
        if (Math.Abs(sensorData.pH - _lastPhValue) < 0.01)
        {
            return;
        }

        if (!_isPhInitialized)
        {
            if (sensorData.pH > 0) // Assuming valid pH values are > 0
            {
                _isPhInitialized = true; // Mark as initialized
                _lastPhValue = sensorData.pH; // Set the baseline pH value
            }
            return; // Skip notifications during initialization
        }

        _lastPhValue = sensorData.pH; // Update the last pH value
        _notificationId++; // Increment the notification ID for unique notifications

        NotificationRequest? request = null; // Initialize request as null

        if (sensorData.pH < 7.0 || sensorData.pH > 8.4)
        {
            request = new NotificationRequest
            {
                NotificationId = _notificationId,
                Title = "AquaCheck",
                Description = "The Aquarium's pH is High",
                BadgeNumber = 1,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddMilliseconds(100)
                }
            };
            LocalNotificationCenter.Current.Show(request);
        }
        else if (sensorData.pH >= 8.0 && sensorData.pH <= 8.4)
        {
            request = new NotificationRequest
            {
                NotificationId = _notificationId,
                Title = "AquaCheck",
                Description = "The Aquarium's pH is High",
                BadgeNumber = 1,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddMilliseconds(100)
                }
            };
            LocalNotificationCenter.Current.Show(request);
        }
        else if (sensorData.pH >= 7.0 && sensorData.pH <= 7.4)
        {
            request = new NotificationRequest
            {
                NotificationId = _notificationId,
                Title = "AquaCheck",
                Description = "The Aquarium's pH is Moderate",
                BadgeNumber = 1,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddMilliseconds(100)
                }
            };
            LocalNotificationCenter.Current.Show(request);
        }
    }

    private void UpdatePhImage(string imageSource, NotificationRequest? request)
    {
        pHImageObj.Children.Remove(pHImage);
        pHImage = new Image
        {
            Source = imageSource,
            WidthRequest = 40,
            Margin = new Thickness(-54, 5, 0, 0)
        };
        pHImageObj.Children.Add(pHImage);
        pH.Text = sensorData.pH.ToString();
    }

    private double _lastTurbidValue = -1; // Initialize to an invalid value
    private bool _isTurbidInitialized = false; // Track initialization state

    public void turbidityCard()
    {
        // Update the turbidity image and text
        UpdateTurbidityImage(
            sensorData.Turbid > 50 ? "water_pollution3.png" :
            sensorData.Turbid > 40 ? "water_pollution2.png" :
            "water_pollution1.png"
        );

        // Skip further processing if the turbidity value hasn't changed significantly
        if (Math.Abs(sensorData.Turbid - _lastTurbidValue) < 0.01)
        {
            return;
        }

        if (!_isTurbidInitialized)
        {
            if (sensorData.Turbid > 0) // Assuming valid turbidity values are > 0
            {
                _isTurbidInitialized = true; // Mark as initialized
                _lastTurbidValue = sensorData.Turbid; // Set the baseline turbidity value
            }
            return; // Skip notifications during initialization
        }

        _lastTurbidValue = sensorData.Turbid; // Update the last turbidity value
        _notificationId++; // Increment the notification ID for unique notifications

        NotificationRequest? request = null; // Initialize request as null

        if (sensorData.Turbid > 50)
        {
            request = new NotificationRequest
            {
                NotificationId = _notificationId,
                Title = "AquaCheck",
                Description = "The Aquarium's Turbidity is High",
                BadgeNumber = 1,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddMilliseconds(100)
                }
            };
            LocalNotificationCenter.Current.Show(request);
        }
        else if (sensorData.Turbid > 40)
        {
            request = new NotificationRequest
            {
                NotificationId = _notificationId,
                Title = "AquaCheck",
                Description = "The Aquarium's Turbidity is Moderate",
                BadgeNumber = 1,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddMilliseconds(100)
                }
            };
            LocalNotificationCenter.Current.Show(request);
        }
    }

    private void UpdateTurbidityImage(string imageSource)
    {
        turbidImageObj.Children.Remove(turbidImage);
        turbidImage = new Image
        {
            Source = imageSource,
            WidthRequest = 40,
            Margin = new Thickness(-55, 5, 0, 0)
        };
        turbidImageObj.Children.Add(turbidImage);
        Turbid.Text = sensorData.Turbid.ToString() + " NTU";
    }


    private async void toRec(object sender, TappedEventArgs e)
    {
        await Navigation.PushModalAsync(new reportsParam(), false);
    }

    private async void toDex(object sender, TappedEventArgs e)
    {
        await Navigation.PushModalAsync(new aquaDex(), false);
    }
}

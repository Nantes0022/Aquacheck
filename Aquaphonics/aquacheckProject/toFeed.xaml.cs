using aquacheckProject.GatherData;
using System.Text.Json;
using System.Text.RegularExpressions;
using Plugin.Maui.Calendar;

namespace aquacheckProject;

public partial class toFeed : ContentPage
{
    public feedData feedData = new feedData();
    Dictionary<string,feedData> schedules;
    string currentMonth = DateTime.Now.ToString("MMMM");
    int currentYear = DateTime.Now.Year;
    public string[] timeFeed = { };
    public toFeed()
	{
		InitializeComponent();
        getData();
        //dateControl();
    }

    private async void setTime_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushModalAsync(new setTimeFish(), false);
    }
    private void back_Tapped(object sender, TappedEventArgs e)
    {
        App.Current.MainPage = new HomePage();
    }



    public async Task getData()
    {
        using HttpClient client = new HttpClient();
        string url = "https://aquacheck.site/feeding/feedAndroidAPI.php?api_key=iu311DXlOK42kGrBNHzPhQ8X8ZrmXXvbrRR8sbUz"; // Replace with your desired URL

        // Send a GET request to the specified URL
        HttpResponseMessage response = await client.GetAsync(url);

        // Ensure the request was successful
        response.EnsureSuccessStatusCode();

        string feedData = await response.Content.ReadAsStringAsync();


        schedules = JsonSerializer.Deserialize<Dictionary<string, feedData>>(feedData);

        // Display all schedules
        foreach (var schedule in schedules)
        {
            if (schedule.Value.date == DateTime.Now.ToString("dd-MM-yyyy"))
            {

                Label lbl1 = new Label
                {
                    FontFamily = "Poppins-Bold",
                    TextColor = Color.FromArgb("#120810"),
                    FontSize = 16,
                    Text = schedule.Value.fish,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                Data.Children.Add(lbl1);

                foreach (var time in schedule.Value.times)
                {
                    Label lbl2 = new Label
                    {
                        FontFamily = "Poppins-Semibold",
                        TextColor = Color.FromArgb("#120810"),
                        FontSize = 14,
                        Text = DateTime.Parse(time).ToString("hh:mm tt"),
                    };
                    Data.Children.Add(lbl2);
                }

                // Add the Image to the StackLayout
            }
        }
    }

    private bool isTimerRunning = true;

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

        if (nearestTime.HasValue)
        {
            StartCountdown(nearestTime.Value);
        }
        else
        {
            interval.Text = "No upcoming feeds for today.";
            hrs.Text = "";
        }
    }

    private async void StartCountdown(DateTime nearestTime)
    {
        while (isTimerRunning)
        {
            DateTime now = DateTime.Now;

            if (nearestTime <= now)
            {
                interval.Text = "It's feeding time!";
                hrs.Text = "";
                isTimerRunning = false; // Stop the timer
                break;
            }

            TimeSpan timeDifference = nearestTime - now;

            // Extract hours, minutes, and seconds from the time difference
            int hours = timeDifference.Hours;
            int minutes = timeDifference.Minutes;
            int seconds = timeDifference.Seconds;

            // Format the interval text based on hours
            if (hours > 0)
            {
                interval.Text = $"Upcoming Feed in {hours:D2}:{minutes:D2}:{seconds:D2}";
            }
            else
            {
                interval.Text = $"Upcoming Feed in {minutes:D2}:{seconds:D2}";
            }

            hrs.Text = "Upcoming Feed";

            // Wait for 1 second before updating
            await Task.Delay(1000);
        }
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        isTimerRunning = true; // Ensure the timer starts
        await getNearestTime();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        isTimerRunning = false; // Stop the timer when the page is not active
    }

    /*public void dateControl()
    {
        pickersChanged(currentYear.ToString(), currentMonth);
        int year = DateTime.Now.Year;
        for (int i = 1; i < 6; i++) {
            YearPicker.Items.Add(year.ToString());
            year++;
        }

        MonthPicker.Title = currentMonth;
        YearPicker.Title = currentYear.ToString();


    }

    private void YearPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        currentYear =Convert.ToInt16(YearPicker.SelectedItem as string);
        pickersChanged(currentYear.ToString(),currentMonth);
    }

    private void MonthPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        currentMonth = MonthPicker.SelectedItem as string;
        pickersChanged(currentYear.ToString(), currentMonth);
    }

    private void pickersChanged(string year, string month) {
        DateTime dt = DateTime.Parse(month + "-01-" + year);
        CalendarView.ShownDate = dt;
    }*/
}
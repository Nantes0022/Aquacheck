using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System;
using System.Text.RegularExpressions;

namespace aquacheckProject;

public partial class setTime : ContentPage
{
    string apiKey = "iu311DXlOK42kGrBNHzPhQ8X8ZrmXXvbrRR8sbUz";
    public string title="Schedule"; 
    public int titlenumber;
    string formattedTime;
    string formattedDate;
    public int ticks;
    public string feedType;
	public setTime()
	{
		InitializeComponent();
        getFeedKey();
	}
    private async void back_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushModalAsync(new toFeed(), false);
    }

    private void setTimeNow_Tapped(object sender, TappedEventArgs e)
    {
        getDandT();
        feedHttpPOSTReq();
    }

    private void feedRadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            var selectedRadioButton = sender as RadioButton;
            if (selectedRadioButton != null)
            {
                string content = selectedRadioButton.Content?.ToString();

                if (content != null)
                {
                    switch (content)
                    {
                        case "1x":
                            ticks = 1;
                            break;
                        case "2x":
                            ticks = 2;
                            break;
                        case "3x":
                            ticks = 3;
                            break;
                    }
                }
            }
        }
    }

    private void feedTypeRadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            var selectedRadioButton = sender as RadioButton;
            string content = selectedRadioButton.Content?.ToString();
            if (content != null)
            {
                switch (content)
                {
                    case "Once":
                        feedType = "onetime";
                        break;
                    case "Everyday":
                        feedType = "everyday";
                        break;
                }
            }
        }
    }

    private async void getFeedKey() 
    {
         using HttpClient client = new HttpClient();
         string url = "https://aquacheck.site/feeding/feedAndroidAPI.php?api_key=iu311DXlOK42kGrBNHzPhQ8X8ZrmXXvbrRR8sbUz"; // Replace with your desired URL
         HttpResponseMessage response = await client.GetAsync(url);

         response.EnsureSuccessStatusCode(); // Throws if the HTTP status is an error code
         string responseBody = await response.Content.ReadAsStringAsync();

         var scheduleData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(responseBody);

            // Accessing individual schedules dynamically
        foreach (var schedule in scheduleData)
        {
            string scheduleName = schedule.Key;
            var scheduleInfo = schedule.Value;
            Match match = Regex.Match(scheduleName, @"\d+");
            if (match.Success) {
                titlenumber = int.Parse(match.Value);
            }
        }
    }

    public async void feedHttpPOSTReq() {
        title += Convert.ToString(titlenumber+1);
        string baseUrl = "https://aquacheck.site/feeding/feedAndroidUpdate.php";
        string url = $"{baseUrl}?api_key={apiKey}&SchedKey={title}&date={formattedDate}&ticks={ticks}&time={formattedTime}&type={feedType}";
        try
        {
            // Create an HttpClient instance
            using HttpClient client = new HttpClient();

            // Make the POST request
            HttpResponseMessage response = await client.PostAsync(url, null);

            // Ensure the response is successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response content
                string responseContent = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Success",$"Response: {responseContent}","OK");
            }
            else
            {
                await DisplayAlert("Failed", $"Error: {response.StatusCode}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Failed", $"Exception: {ex.Message}", "OK");
        }
    }

    public void getDandT() {
        DateTime t = DateTime.Parse(timepick.Time.ToString());
        formattedTime = t.ToString("HH:mm");
        DateTime t2 = DateTime.Now;
        string nowformattedTime = t2.ToString("HH:mm");
        DateTime d()
        {
            if (TimeSpan.Parse(nowformattedTime) > TimeSpan.Parse(formattedTime))
            {
                return DateTime.Now.AddDays(1);
            }
            else
            {
                return DateTime.Now;
            }
        };
        formattedDate = d().ToString("dd-MM-yyyy");
    }
}
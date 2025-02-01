using MySqlConnector;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;
using aquacheckProject.GatherData;
using Microsoft.Maui.Animations;
using Microsoft.VisualBasic.FileIO;

namespace aquacheckProject;

public partial class setTimeFish : ContentPage
{
    feedData feedData = new feedData();
    List<string> timetags = new List<string>();
    int dispensetag;
    public setTimeFish()
	{
		InitializeComponent();

    }

    private async void back_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushModalAsync(new toFeed(), false);
    }

    
    private void pickFeed_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (pickFeed.SelectedItem.ToString() == "Feed based on fish")
        {
            AutoFeed.IsVisible = true;
            feedbtn.IsVisible = false;
        }
        else if (pickFeed.SelectedItem.ToString() == "Manual Feeding") 
        {
            AutoFeed.IsVisible = false;
            fishControl.IsVisible = false;
            pickFish.SelectedItem = "Select Fish";
            timetags.Clear();
            dispensetag=0;
            feedbtn.IsVisible = true;
        }
    }

    private void pickFish_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (pickFish.SelectedItem.ToString() == "Goldfish")
        {
            fishControl.IsVisible = true;
            timetags.Clear();
            timetags.Add("08:00");
            timetags.Add("20:00");
            dispensetag = 5;
            timeDisplay.Text = "";
            dispenseDisplay.Text = "";
            for (int i = 0; i < timetags.Count; i++)
            {
                string timeString = DateTime.Parse(timetags[i].ToString()).ToString("hh:mm tt");
                if (i < timetags.Count - 1)
                    timeDisplay.Text += timeString + ", ";
                else
                    timeDisplay.Text += timeString;
            }
            dispenseDisplay.Text = dispensetag.ToString() + " Seconds";
        }
        else if (pickFish.SelectedItem.ToString() == "Guppies")
        {
            fishControl.IsVisible = true;
            timetags.Clear();
            timetags.Add("08:00");
            timetags.Add("12:00");
            timetags.Add("18:00");
            dispensetag = 3;
            timeDisplay.Text = "";
            dispenseDisplay.Text = "";
            for (int i = 0; i < timetags.Count; i++)
            {
                string timeString = DateTime.Parse(timetags[i].ToString()).ToString("hh:mm tt");
                if (i < timetags.Count - 1)
                    timeDisplay.Text += timeString + ", ";
                else
                    timeDisplay.Text += timeString;
            }
            dispenseDisplay.Text = dispensetag.ToString() + " Seconds";
        }
        else if (pickFish.SelectedItem.ToString() == "Neon Tetras")
        {
            fishControl.IsVisible = true;
            timetags.Clear();
            timetags.Add("08:00");
            timetags.Add("20:00");
            dispensetag = 3;
            timeDisplay.Text = "";
            dispenseDisplay.Text = "";
            for (int i = 0; i < timetags.Count; i++)
            {
                string timeString = DateTime.Parse(timetags[i].ToString()).ToString("hh:mm tt");
                if (i < timetags.Count - 1)
                    timeDisplay.Text += timeString + ", ";
                else
                    timeDisplay.Text += timeString;
            }
            dispenseDisplay.Text = dispensetag.ToString() + " Seconds";
        }
        else if (pickFish.SelectedItem.ToString() == "Mollies")
        {

            fishControl.IsVisible = true;
            timetags.Clear();
            timetags.Add("08:00");
            timetags.Add("12:00");
            timetags.Add("20:00");
            dispensetag = 5;
            timeDisplay.Text = "";
            dispenseDisplay.Text = "";
            for (int i = 0; i < timetags.Count; i++)
            {
                string timeString = DateTime.Parse(timetags[i].ToString()).ToString("hh:mm tt");
                if (i < timetags.Count - 1)
                    timeDisplay.Text += timeString + ", ";
                else
                    timeDisplay.Text += timeString;
            }
            dispenseDisplay.Text = dispensetag.ToString() + " Seconds";
        }
        else if (pickFish.SelectedItem.ToString() == "Platies")
        {
            fishControl.IsVisible = true;
            timetags.Clear();
            timetags.Add("08:00");
            timetags.Add("12:00");
            timetags.Add("18:00");
            dispensetag = 3;
            timeDisplay.Text = "";
            dispenseDisplay.Text = "";
            for (int i = 0; i < timetags.Count; i++)
            {
                string timeString = DateTime.Parse(timetags[i].ToString()).ToString("hh:mm tt");
                if (i < timetags.Count - 1)
                    timeDisplay.Text += timeString + ", ";
                else
                    timeDisplay.Text += timeString;
            }
            dispenseDisplay.Text = dispensetag.ToString() + " Seconds";
        }
        else if (pickFish.SelectedItem.ToString() == "Angelfish")
        {

            fishControl.IsVisible = true;
            timetags.Clear();
            timetags.Add("08:00");
            timetags.Add("12:00");
            timetags.Add("20:00");
            dispensetag = 5;
            timeDisplay.Text = "";
            dispenseDisplay.Text = "";
            for (int i = 0; i < timetags.Count; i++)
            {
                string timeString = DateTime.Parse(timetags[i].ToString()).ToString("hh:mm tt");
                if (i < timetags.Count - 1)
                    timeDisplay.Text += timeString + ", ";
                else
                    timeDisplay.Text += timeString;
            }
            dispenseDisplay.Text = dispensetag.ToString() + " Seconds";
        }
    }

    public async void getData()
    {
        using HttpClient client = new HttpClient();
        string url = "https://aquacheck.site/feeding/feedAndroidAPI.php?api_key=iu311DXlOK42kGrBNHzPhQ8X8ZrmXXvbrRR8sbUz"; // Replace with your desired URL

        // Send a GET request to the specified URL
        HttpResponseMessage response = await client.GetAsync(url);

        // Ensure the request was successful
        response.EnsureSuccessStatusCode();

        string feedData = await response.Content.ReadAsStringAsync();


        var schedules = JsonSerializer.Deserialize<Dictionary<string, feedData>>(feedData);

        bool isDuplicate = schedules.Any(fish => fish.Key.ToString() == pickFish.SelectedItem.ToString());

        if (isDuplicate)
        {
            await DisplayAlert("Aquacheck", pickFish.SelectedItem.ToString() + " already inserted", "OK");
        }
        else
        {
            feedInsert();
        }
    }

    private async void feedInsert() 
    {
        string apiKey = "iu311DXlOK42kGrBNHzPhQ8X8ZrmXXvbrRR8sbUz";
        string baseUrl = "https://aquacheck.site/feeding/feedAndroidUpdate.php";
        string url = $"{baseUrl}?api_key={apiKey}&SchedKey={pickFish.SelectedItem.ToString()}&date={DateTime.Now.ToString("dd-MM-yyyy")}&disptime={dispensetag}&fish={pickFish.SelectedItem.ToString()}";
        foreach (var t in timetags)
        {
            url += $"&times[]={t}";
        }

        url += "&type=AutomaticFeeding";

        using HttpClient client = new HttpClient();

        HttpResponseMessage response = await client.PutAsync(url, null);

        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            await DisplayAlert("Success", "Data Recorded", "OK");
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            await DisplayAlert("Failed", $"Error: {response.StatusCode}", "OK");
        }
    }

    private void Save_Tapped(object sender, TappedEventArgs e)
    {
        getData();
    }

    private async void Feed_Tapped(object sender, TappedEventArgs e)
    {
        string apiKey = "iu311DXlOK42kGrBNHzPhQ8X8ZrmXXvbrRR8sbUz";
        string baseUrl = "https://aquacheck.site/feeding/feedAndroidUpdate.php";
        string url = $"{baseUrl}?api_key={apiKey}&SchedKey=Manual&date={DateTime.Now.ToString("dd-MM-yyyy")}&disptime=5&fish=Manual&times[]={DateTime.Now.ToString("HH:mm")}&type=ManualFeeding";

        using HttpClient client = new HttpClient();

        HttpResponseMessage response = await client.PutAsync(url, null);

        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            bool isFirebaseSuccess = await InsertDataToFirebaseAsync();
            await DisplayAlert("Success", "Data Recorded", "OK");
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            await DisplayAlert("Failed", $"Error: {response.StatusCode}", "OK");
        }
    }
    private async Task<bool> InsertDataToFirebaseAsync()
    {
        // Database connection details
        string connectionString = "Server=srv1365.hstgr.io;Port=3306;Database=u926624511_db_aqua;User=u926624511_aquacheck;Password=Aquacheck123";

        // Adjust to UTC+8
        TimeZoneInfo utcPlus8 = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"); // UTC+8 time zone
        DateTime utcPlus8Now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, utcPlus8);

        string currentDate = utcPlus8Now.ToString("dd-MM-yyyy"); // Format as MySQL `DATE`
        string currentTime = utcPlus8Now.ToString("HH:mm:ss");   // Format as MySQL `TIME`
        string typeData = "Manual Feeding";
        string fishData = "Default"; // Your sample data


        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();

            // Insert query
            string query = @"
                INSERT INTO tbl_reports (type,date, time, fish)
                VALUES (@Type, @Date, @Time, @Fish)";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Type", typeData);
                cmd.Parameters.AddWithValue("@Date", currentDate);
                cmd.Parameters.AddWithValue("@Time", currentTime);
                cmd.Parameters.AddWithValue("@Fish", fishData);

                // Execute the query
                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                // Return true if one or more rows are affected
                return rowsAffected > 0;
            }
        }
    }
}
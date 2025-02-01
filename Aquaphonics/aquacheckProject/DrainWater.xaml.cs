namespace aquacheckProject;

using aquacheckProject.GatherData;
using System.Text.Json;
using System.Threading.Tasks;

public partial class DrainWater : ContentPage
{
    public DrainWater()
    {
        InitializeComponent();
        checkData();
    }
    public async void checkData()
    {
        getControlData getControlData = new getControlData();
        using HttpClient client = new HttpClient();
        string url = "https://aquacheck.site/Controls/Control.php?api_key=iu311DXlOK42kGrBNHzPhQ8X8ZrmXXvbrRR8sbUz"; // Replace with your desired URL

        // Send a GET request to the specified URL
        HttpResponseMessage response = await client.GetAsync(url);

        // Ensure the request was successful
        response.EnsureSuccessStatusCode();

        string controlData = await response.Content.ReadAsStringAsync();

        var controlObject = JsonSerializer.Deserialize<getControlData.Controls>(controlData);


        if (controlObject.WaterOut)
        {
            OnFrame.IsVisible = false;
            OffFrame.IsVisible = true;
        }
    }
    private void returnHomeTemp(object sender, TappedEventArgs e)
    {
        Shell.Current.GoToAsync("..");
    }


    private async void turnOn(object sender, TappedEventArgs e)
    {
        getControlData getControlData = new getControlData();
        using HttpClient client = new HttpClient();
        string url = "https://aquacheck.site/Controls/Control.php?api_key=iu311DXlOK42kGrBNHzPhQ8X8ZrmXXvbrRR8sbUz"; // Replace with your desired URL

        // Send a GET request to the specified URL
        HttpResponseMessage response = await client.GetAsync(url);

        // Ensure the request was successful
        response.EnsureSuccessStatusCode();

        string controlData = await response.Content.ReadAsStringAsync();

        var controlObject = JsonSerializer.Deserialize<getControlData.Controls>(controlData);


        if (controlObject.TempChange || controlObject.WaterIn || controlObject.WaterOut)
        {
            await DisplayAlert("AQUACHECK", "Turn off other Water Control first", "OK");
        }
        else
        {
            trunOn();
        }
    }
    private async void trunOn()
    {
        string apiKey = "iu311DXlOK42kGrBNHzPhQ8X8ZrmXXvbrRR8sbUz";
        string baseUrl = "https://aquacheck.site/Controls/ControlUpdate.php";
        string url = $"{baseUrl}?api_key={apiKey}&ContType=WaterOut&ContStatus=true";

        using HttpClient client = new HttpClient();

        HttpResponseMessage response = await client.PatchAsync(url, null);

        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            Indicator1.FontSize = 15;
            Indicator1.Text = "Turning On... Please Wait 10 Seconds";
            OnFrame.IsEnabled = false;
            OnFrame.Opacity = 0.5;
            _ = ExecuteWithDelayOn();
        }
        else
        {
            await DisplayAlert("Failed", $"Error: {response.StatusCode}", "OK");
        }
    }

    public async Task ExecuteWithDelayOn()
    {
        await Task.Delay(10000);

        Indicator1.FontSize = 20;
        Indicator1.Text = "Turn ON";
        OnFrame.IsEnabled = true;
        OnFrame.Opacity = 1;
        OnFrame.IsVisible = false;
        OffFrame.IsVisible = true;
        Indicator2.Text = "Turn OFF";
    }

    private async void turnOff(object sender, TappedEventArgs e)
    {
        string apiKey = "iu311DXlOK42kGrBNHzPhQ8X8ZrmXXvbrRR8sbUz";
        string baseUrl = "https://aquacheck.site/Controls/ControlUpdate.php";
        string url = $"{baseUrl}?api_key={apiKey}&ContType=WaterOut&ContStatus=false";

        using HttpClient client = new HttpClient();

        HttpResponseMessage response = await client.PatchAsync(url, null);

        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            Indicator2.FontSize = 15;
            Indicator2.Text = "Turning Off... Please Wait 10 Seconds";
            OffFrame.IsEnabled = false;
            OffFrame.Opacity = 0.5;
            _ = ExecuteWithDelayOff();
        }
        else
        {
            await DisplayAlert("Failed", $"Error: {response.StatusCode}", "OK");
        }
    }

    public async Task ExecuteWithDelayOff()
    {
        await Task.Delay(10000);

        Indicator2.FontSize = 20;
        Indicator2.Text = "Turn ON";
        OffFrame.IsEnabled = true;
        OffFrame.Opacity = 1;
        OffFrame.IsVisible = false;
        OnFrame.IsVisible = true;
        Indicator2.Text = "Turn OFF";
    }
}
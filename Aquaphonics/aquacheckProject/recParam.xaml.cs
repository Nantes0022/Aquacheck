using SkiaSharp;
using SkiaSharp.Views.Maui;
using Microcharts;
using Microcharts.Maui;
using aquacheckProject.GatherData;

namespace aquacheckProject;

public partial class recParam : ContentPage
{
    private static readonly HttpClient client = new HttpClient();
    private CancellationTokenSource _cancellationTokenSource;
    public getData getData = new getData();

    public async Task StartTimerAsync()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                string url = "https://aquacheck.site/Reports/connect.php?api_key=iu311DXlOK42kGrBNHzPhQ8X8ZrmXXvbrRR8sbUz";
                string jsonResponse = await getDataAsync(url);

                if (jsonResponse != "Error")
                {
                    dataRec.Text = jsonResponse; // Display raw data or parse and display specific information
                }

                await Task.Delay(1000, _cancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                // Gracefully handle task cancellation
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data: {ex.Message}");
            }
        }
    }

    private async Task<string> getDataAsync(string urlString)
    {
        try
        {
            return await client.GetStringAsync(urlString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching data from {urlString}: {ex.Message}");
            return "Error";
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await StartTimerAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _cancellationTokenSource?.Cancel();
    }

    ChartEntry[] entries = new[]
    {
        new ChartEntry(212)
        {
            Label = "Windows",
            ValueLabel = "112",
            Color = SKColor.Parse("#2c3e50")
        },
        new ChartEntry(248)
        {
            Label = "Android",
            ValueLabel = "648",
            Color = SKColor.Parse("#77d065")
        },
        new ChartEntry(128)
        {
            Label = "iOS",
            ValueLabel = "428",
            Color = SKColor.Parse("#b455b6")
        },
        new ChartEntry(514)
        {
            Label = ".NET MAUI",
            ValueLabel = "214",
            Color = SKColor.Parse("#3498db")
        }
    };

    public recParam()
    {
        InitializeComponent();

        chartView.Chart = new LineChart
        {
            Entries = entries,
            LabelTextSize = 25,
            LineMode = LineMode.Straight,
            LineSize = 5,
            PointMode = PointMode.Circle,
            PointSize = 10,
            BackgroundColor = SKColor.Parse("#FFFFFF"),
        };
    }

    private void toHome(object sender, TappedEventArgs e)
    {
        App.Current.MainPage = new HomePage();
    }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);

        var paint = new SKPaint
        {
            Color = SKColors.Red,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        canvas.DrawCircle(e.Info.Width / 2, e.Info.Height / 2, 100, paint);
    }
}

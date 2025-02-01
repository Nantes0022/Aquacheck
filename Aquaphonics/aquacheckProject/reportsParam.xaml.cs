using MySqlConnector;
using System.ComponentModel;
using System;


namespace aquacheckProject;

public partial class reportsParam : ContentPage
{
	public reportsParam()
	{
		InitializeComponent();
      
        

        // Optionally bind SelectedDate to the DatePicker
        BindingContext = new MainViewModel();
    }

    private void toHome(object sender, TappedEventArgs e)
    {
        App.Current.MainPage = new HomePage();
    }
    public class MainViewModel : INotifyPropertyChanged
    {
        public DateTime CurrentDate { get; set; } = DateTime.Now;
        public DateTime MinDate { get; set; } = new DateTime(2020, 1, 1);
        public DateTime MaxDate { get; set; } = DateTime.Today.AddYears(1);

        private DateTime selectedDate;
        public DateTime SelectedDate
        {
            get => selectedDate;
            set
            {
                if (selectedDate != value)
                {
                    selectedDate = value;
                    OnPropertyChanged(nameof(SelectedDate));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    private async void optionsPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedOption = optionsPicker.SelectedItem?.ToString();
        var selectedDate = datePicker.Date.ToString("dd-MM-yyyy"); // Get the selected date

        if (!string.IsNullOrEmpty(selectedOption))
        {
          
            var reports = await FetchReportsAsync(selectedDate, selectedOption);

         
            reportsCollectionView.ItemsSource = reports;

            if (reports.Count == 0)
            {
                await DisplayAlert("No Data", "No reports found for the selected date and type.", "OK");
            }
        }
    }

    public async Task<List<Report>> FetchReportsAsync(string selectedDate, string selectedType)
    {
        string connectionString = "Server=srv1365.hstgr.io;Port=3306;Database=u926624511_db_aqua;User=u926624511_aquacheck;Password=Aquacheck123";
        var reports = new List<Report>();

        using (var connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();

      
            string query = @"SELECT * FROM tbl_reports WHERE date = @Date AND type = @Type";

            using (var cmd = new MySqlCommand(query, connection))
            {
              
                cmd.Parameters.AddWithValue("@Date", selectedDate);
                cmd.Parameters.AddWithValue("@Type", selectedType);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        reports.Add(new Report
                        {
                            Id = reader.GetInt32("id"),
                            Type = reader.GetString("type"),
                            Date = reader.GetString("date"),
                            Time = reader.GetString("time"),
                            Fish = reader.GetString("fish")
                        });
                    }
                }
            }
        }

        return reports;
    }
    public class Report
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Fish { get; set; }
    }

    private async void datePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        var selectedOption = optionsPicker.SelectedItem?.ToString();
        var selectedDate = datePicker.Date.ToString("dd-MM-yyyy"); // Get the selected date

        if (!string.IsNullOrEmpty(selectedOption))
        {
          
            var reports = await FetchReportsAsync(selectedDate, selectedOption);

           
            reportsCollectionView.ItemsSource = reports;

         
            if (reports.Count == 0)
            {
                await DisplayAlert("No Data", "No reports found for the selected date and type.", "OK");
            }
        }
    }
}
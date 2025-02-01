using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

/* Unmerged change from project 'aquacheckProject (net8.0-windows10.0.19041.0)'
Before:
namespace aquacheckProject
{
After:
using aquacheckProject;
using aquacheckProject.GatherData;
namespace aquacheckProject.GatherData
{
*/
namespace aquacheckProject.GatherData
{
    public class getData
    {
        public double TDS { get; set; }
        public double Temperature { get; set; }
        public double Waterlvl { get; set; }
        public double Turbid { get; set; }
        public double pH { get; set; }

        public async void data()
        {
            using HttpClient client = new HttpClient();
            string url = "https://aquacheck.site/Reports/connect.php?api_key=iu311DXlOK42kGrBNHzPhQ8X8ZrmXXvbrRR8sbUz"; // Replace with your desired URL
            var response = await client.GetAsync(url);
            var data = response.Content.ReadAsStream();
            getData sensors = JsonSerializer.Deserialize<getData>(data);

            Temperature = sensors.Temperature;
            Waterlvl = sensors.Waterlvl;
            TDS = sensors.TDS;
            pH = sensors.pH;
            Turbid = sensors.Turbid;
        }
    }
}

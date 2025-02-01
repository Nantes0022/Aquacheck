using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace aquacheckProject.GatherData
{
    public class sensorData
    {
        public double TDS { get; set; }
        public double Temperature { get; set; }
        public double Waterlvl { get; set; }
        public double Turbid { get; set; }
        public double pH { get; set; }

        public async void data() {
            using HttpClient client = new HttpClient();
            string url = "https://aquacheck.site/sensors/readSensors.php?api_key=iu311DXlOK42kGrBNHzPhQ8X8ZrmXXvbrRR8sbUz"; // Replace with your desired URL
            var response = await client.GetAsync(url);
            var data = response.Content.ReadAsStream();
            sensorData sensors = JsonSerializer.Deserialize<sensorData>(data);

            Temperature = sensors.Temperature;
            Waterlvl = sensors.Waterlvl;
            TDS = sensors.TDS;
            pH = sensors.pH;
            Turbid = sensors.Turbid;
        }
    }
}

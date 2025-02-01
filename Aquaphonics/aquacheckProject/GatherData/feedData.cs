using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace aquacheckProject.GatherData
{
    public class feedData
    {
        public string feedType = "";
        public string date { get; set; }
        public int disptime { get; set; }
        public string fish { get; set; }
        public List<string> times { get; set; }
        public string type { get; set; }


    }

}

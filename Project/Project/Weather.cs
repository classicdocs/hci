using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Project
{
    public class Weather
    {
        [JsonProperty("list")]
        public List<DataForHour> dataForHour { get; set; }
        public City city { get; set; }

    }
    
    public class DataForHour
    {
        public String dt { get; set; }
        [JsonProperty("main")]
        public Data data { get; set; }
        [JsonProperty("weather")]
        public List<Condition> conditions { get; set; }
        [JsonProperty("dt_txt")]
        public String date_time { get; set; } // Time
    }

    public class Data
    {
        public double temp { get; set; }
        public double temp_max { get; set; }
        public double temp_min { get; set; } 
    }

    public class Condition
    {
        public String description { get; set; }
        public String icon { get; set; }
    }

    public class City
    {
        public String name { get; set; }
        public String country { get; set; }
    }
}

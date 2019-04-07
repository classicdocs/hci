using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Project
{
    public class CurrentWeather
    {
        [JsonProperty("name")]
        public string city { get; set; }
        [JsonProperty("weather")]
        public List<Condition> conditions { get; set; }
        [JsonProperty("main")]
        public Data data { get; set; }

        public string dateUpdated { get; set; }
    }

    public class Weather
    {
        [JsonProperty("list")]
        public List<DataForHour> dataForHour { get; set; }
        public City city { get; set; }

    }
    
    public class DataForHour
    {
        public string dt { get; set; }
        [JsonProperty("main")]
        public Data data { get; set; }
        [JsonProperty("weather")]
        public List<Condition> conditions { get; set; }
        [JsonProperty("dt_txt")]
        public string date_time { get; set; } // Time
    }

    public class Data
    {
        public string temp { get; set; }
        public string temp_max { get; set; }
        public string temp_min { get; set; } 
    }

    public class Condition
    {
        public string description { get; set; }
        public string icon { get; set; }
    }

    public class City
    {
        public string name { get; set; }
        public string country { get; set; }
    }
}

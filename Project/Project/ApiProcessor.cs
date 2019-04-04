using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    public class ApiProcessor
    {
        public static async Task<CurrentLocation> LoadLocation()
        {
            //  http://api.ipstack.com/24.135.247.78?access_key=4db9b757ec43d5674883c0d82943b666

            string url = "";
        
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            string key = "4db9b757ec43d5674883c0d82943b666";
            //url = $"http://api.ipstack.com/" + myIP + "?access_key=" + key;
            url = $"http://api.ipstack.com/24.135.247.78" + "?access_key=" + key;

            using (HttpResponseMessage response = await ApiHandler.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    CurrentLocation location = await response.Content.ReadAsAsync<CurrentLocation>();
                    return location;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public static async Task<Weather> LoadWeather(String currentLocation)
        {
            string url = "http://api.openweathermap.org/data/2.5/forecast?q=" + currentLocation + "&mode=json&units=metric";
            url += "&APPID=16d119fd8dbd2cd49427a81078519ca8";
            using (HttpResponseMessage response = await ApiHandler.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    Weather weather = await response.Content.ReadAsAsync<Weather>();
                    return weather;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }



        }
    }
}

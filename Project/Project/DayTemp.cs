using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    public class DayTemp
    {
        public string day { get; set; }
        public string temp { get; set; }
        public string date { get; set; }
        public List<HourTemp> hoursTemp { get; set; }
        public string img1 { get; set; }
        public string img2 { get; set; }
    }
}

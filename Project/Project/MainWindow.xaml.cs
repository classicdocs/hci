using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string currentLocation = "";
        private Weather weather;

        private string _currentCity { get; set; }
        public string currentCity
        {
            get { return _currentCity; }
            set { if (_currentCity != value)
                    {
                        _currentCity = value;
                        OnPropertyChanged();
                    }
                }
        }

        private CurrentWeather _CurrentWeather
        {
            get; set;
        }

        public CurrentWeather CurrentWeather
        {
            get { return _CurrentWeather; }
            set { if (_CurrentWeather != value)
                {
                    _CurrentWeather = value;
                    OnPropertyChanged();
                }
                } 
        }

        public ObservableCollection<HourTemp> HoursTemp
        {
            get; set;
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            HoursTemp = new ObservableCollection<HourTemp>();
            CurrentWeather = new CurrentWeather();
            ApiHandler.InitializeClient();
        }

        private async void LoadWeather(Object obj)
        {
            string location = (String)obj;
            while (true)
            {
                Weather weather = await ApiProcessor.LoadWeather(location);
                var dataForHour = weather.dataForHour;
                
                foreach(var data in dataForHour)
                {
                    HourTemp ht = new HourTemp();
                    int ind = data.date_time.IndexOf(" ");
                    string date_time = data.date_time;
                    ht.hour = date_time.Substring(ind + 1,5);
                    ht.temp = ((int)Double.Parse(data.data.temp)) + "°C";
                    ht.img = "http://openweathermap.org/img/w/" + data.conditions[0].icon + ".png";
                    HoursTemp.Add(ht);
                }

                CurrentWeather currentWeather = await ApiProcessor.LoadCurrentWeather(location);
                string img = "http://openweathermap.org/img/w/" + currentWeather.conditions[0].icon + ".png";
                currentWeather.conditions[0].icon = img;
                string temp =((int)Double.Parse(currentWeather.data.temp)) + "°C";
                currentWeather.data.temp = temp;
                CurrentWeather = currentWeather;
                

                Thread.Sleep(TimeSpan.FromMinutes(3));
            }
        }

        private async Task LoadLocation()
        {
            var city = await ApiProcessor.LoadLocation();
            currentLocation = city.city;

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadLocation();
            Thread thread = new Thread(new ParameterizedThreadStart(LoadWeather));
            thread.Start(currentLocation);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void searchClick(object sender, RoutedEventArgs e)
        {
            //TODO binding
            if (searchBox.Visibility == Visibility.Hidden)
            {
                searchBox.Visibility = Visibility.Visible;
            }
            else
            {
                searchBox.Visibility = Visibility.Hidden;
            }
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}

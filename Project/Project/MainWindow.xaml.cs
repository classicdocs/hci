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
using System.Windows.Resources;

namespace Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private ImageBrush favouriteImg = new ImageBrush();
        private ImageBrush notFavouriteImg = new ImageBrush();

        
        private string currentLocation = "";

        Thread thread = null;

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

        public ObservableCollection<DayTemp> DaysTemp
        {
            get; set;
        }

        public MainWindow()
        {
            InitialiseIcons();
            InitializeComponent();
            DataContext = this;
            HoursTemp = new ObservableCollection<HourTemp>();
            DaysTemp = new ObservableCollection<DayTemp>();
            CurrentWeather = new CurrentWeather();
            ApiHandler.InitializeClient();
        }

        private void InitialiseIcons()
        {
            Uri favouriteUri = new Uri("Resources/Favourite.png", UriKind.Relative);
            Uri notFavouriteUri = new Uri("Resources/NotFavourite.png", UriKind.Relative);

            StreamResourceInfo streamFavourite = Application.GetResourceStream(favouriteUri);
            StreamResourceInfo streamNotFavourite = Application.GetResourceStream(notFavouriteUri);

            BitmapFrame favouriteFrame = BitmapFrame.Create(streamFavourite.Stream);
            BitmapFrame notFavouriteFrame = BitmapFrame.Create(streamNotFavourite.Stream);

            
            this.favouriteImg.ImageSource = favouriteFrame;
            this.notFavouriteImg.ImageSource = notFavouriteFrame;
        }

        private async void LoadWeather(Object obj)
        {
            string location = (String)obj;
            while (true)
            {
                Weather weather = await ApiProcessor.LoadWeather(location);
                setHoursTemp(weather);
                setCurrentTemp(weather, location);
                setDaysTemp(weather);
                Thread.Sleep(TimeSpan.FromMinutes(30));
            }
        }

        private void setDaysTemp(Weather weather)
        {
            var dataForHour = weather.dataForHour;

            int index = 0;
            for (int i = 0; i < dataForHour.Count; i++)
            {
                int ind = dataForHour[i].date_time.IndexOf(" ");
                string time = dataForHour[i].date_time.Substring(ind + 1, 5);
                if (time.Equals("00:00")) {
                    index = i;
                    break;
                }
            }
            bool end = false;
            for (int i = 0; i < 5; i++)
            {
                DayTemp dt = new DayTemp();
                dt.hoursTemp = new List<HourTemp>();
                double max_temp = -100;
                double min_temp = 100;
                dt.dayIndex = i.ToString();
                for (int j = 0; j < 8; j++)
                {
                    int idx = index + i * 8 + j;
                    if (idx >= dataForHour.Count)
                    {
                        end = true;
                        break;
                    }
                    if (Double.Parse(dataForHour[idx].data.temp_max) > max_temp)
                    {
                        max_temp = Double.Parse(dataForHour[idx].data.temp_max);
                    }
                    if (Double.Parse(dataForHour[idx].data.temp_min) < min_temp)
                    {
                        min_temp = Double.Parse(dataForHour[idx].data.temp_min);
                    }

                    HourTemp ht = new HourTemp();
                    int ind = dataForHour[idx].date_time.IndexOf(" ");

                    DateTime date = DateTime.ParseExact(dataForHour[idx].date_time.Substring(0, ind), "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture);
                    dt.date = date.DayOfWeek.ToString();
                    ht.hour = dataForHour[idx].date_time.Substring(ind + 1, 5);
                    ht.img = "http://openweathermap.org/img/w/" + dataForHour[idx].conditions[0].icon + ".png";
                    if (ht.hour == "09:00")
                        dt.img1 = "http://openweathermap.org/img/w/" + dataForHour[idx].conditions[0].icon + ".png";
                    else if (ht.hour == "21:00")
                        dt.img2 = "http://openweathermap.org/img/w/" + dataForHour[idx].conditions[0].icon + ".png";
                    ht.temp = ((int)Double.Parse(dataForHour[idx].data.temp)) + "°C";
                    dt.hoursTemp.Add(ht);
                }
                
                dt.temp = ((int)max_temp) + "/" + ((int)min_temp);
                DaysTemp.Add(dt);
                if (end) break;
            }
            Console.Write("");

        }

        // metoda za podesavanje temperature za naredna 24h
        private void setHoursTemp(Weather weather)
        {
            var dataForHour = weather.dataForHour;
            foreach (var data in dataForHour)
            {
                HourTemp ht = new HourTemp();
                int ind = data.date_time.IndexOf(" ");
                string date_time = data.date_time;
                ht.hour = date_time.Substring(ind + 1, 5);
                ht.temp = ((int)Double.Parse(data.data.temp)) + "°C";
                ht.img = "http://openweathermap.org/img/w/" + data.conditions[0].icon + ".png";
                HoursTemp.Add(ht);
            }
        }

        // metoda za podesavanje trenutne temperature
        private async void setCurrentTemp(Weather weather, String location)
        {
            CurrentWeather currentWeather = await ApiProcessor.LoadCurrentWeather(location);
            string img = "http://openweathermap.org/img/w/" + currentWeather.conditions[0].icon + ".png";
            currentWeather.conditions[0].icon = img;
            string temp = ((int)Double.Parse(currentWeather.data.temp)) + "°C";
            currentWeather.data.temp = temp;
            currentWeather.dateUpdated = DateTime.Now.ToString("hh:mm");
            CurrentWeather = currentWeather;
        }

        private async Task LoadLocation()
        {
            var city = await ApiProcessor.LoadLocation();
            currentLocation = city.city;

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadLocation();
            startNewThread();
        }

        private void hourlyClick(object sender, RoutedEventArgs e)
        {
            var dayIndex = ((Button)sender).Tag;
            if (dayIndex.Equals("0"))
            {
                Day0Hours.Visibility = Visibility.Visible;
                Day0.Visibility = Visibility.Hidden;
            }

            if (dayIndex.Equals("1"))
            {
                Day1Hours.Visibility = Visibility.Visible;
                Day1.Visibility = Visibility.Hidden;
            }

            if (dayIndex.Equals("2"))
            {
                Day2Hours.Visibility = Visibility.Visible;
                Day2.Visibility = Visibility.Hidden;
            }

            if (dayIndex.Equals("3"))
            {
                Day3Hours.Visibility = Visibility.Visible;
                Day3.Visibility = Visibility.Hidden;
            }

            if (dayIndex.Equals("4"))
            {
                Day4Hours.Visibility = Visibility.Visible;
                Day4.Visibility = Visibility.Hidden;
            }
        }

        private void closeDay0(object sender, RoutedEventArgs e)
        {
            Day0Hours.Visibility = Visibility.Hidden;
            Day0.Visibility = Visibility.Visible;
        }

        private void closeDay1(object sender, RoutedEventArgs e)
        {
            Day1Hours.Visibility = Visibility.Hidden;
            Day1.Visibility = Visibility.Visible;
        }

        private void closeDay2(object sender, RoutedEventArgs e)
        {
            Day2Hours.Visibility = Visibility.Hidden;
            Day2.Visibility = Visibility.Visible;
        }

        private void closeDay3(object sender, RoutedEventArgs e)
        {
            Day3Hours.Visibility = Visibility.Hidden;
            Day3.Visibility = Visibility.Visible;
        }

        private void closeDay4(object sender, RoutedEventArgs e)
        {
            Day4Hours.Visibility = Visibility.Hidden;
            Day4.Visibility = Visibility.Visible;
        }

        private void searchClick(object sender, RoutedEventArgs e)
        {
            //TODO binding
        }
        private void addToFavoritesClick(object sender, RoutedEventArgs e)
        {
            if (addToFavBtn.IsDefault)
            {
                addToFavBtn.Background = this.favouriteImg;
                addToFavBtn.IsDefault = !addToFavBtn.IsDefault;
            }
            else
            {
                addToFavBtn.Background = this.notFavouriteImg;
                addToFavBtn.IsDefault = !addToFavBtn.IsDefault;
            }

            //TODO dodaj grad u favourites
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void btn_update_click(object sender, RoutedEventArgs e)
        {
            thread.Abort();
            startNewThread();
        }

        private void startNewThread()
        {
            thread = new Thread(new ParameterizedThreadStart(LoadWeather));
            thread.Start(currentLocation);
        }
    }
}

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
using System.IO;

namespace Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private ImageBrush favouriteImg = new ImageBrush();
        private ImageBrush notFavouriteImg = new ImageBrush();

        string pathFavourites = @"../../Resources\favourites.txt";
        string pathHistory = @"../../Resources\history.txt";

        private bool isFavourite = false;
        private List<MenuItem> favouriteItemsList;
        private List<MenuItem> historyItemsList;

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

        //private string _favouriteSearchCity { get; set; }

        //public string favouriteSearchCity
        //{
        //    get { return _favouriteSearchCity; }
        //    set { if(_favouriteSearchCity != value)
        //        {
        //            _favouriteSearchCity = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        public ObservableCollection<String> favouriteCities
        {
            get; set;
        }

        public ObservableCollection<String> historyCities
        {
            get; set;
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

        // metoda koja ucitava gradove koji su zapamceni kao omiljeni
        public void loadFavorites()
        {
            this.favouriteItemsList = new List<MenuItem>()
            {
                fav_0, fav_1, fav_2, fav_3, fav_4
            };
            favouriteCities = new ObservableCollection<string>();
            string contents = File.ReadAllText(this.pathFavourites);

            if (contents != null)
            {
                foreach (string cityName in contents.Split(','))
                {
                    if (cityName.Length > 1)
                    {
                        if (cityName[cityName.Length - 1] == '\n')
                        {
                            cityName.Remove(cityName.Length - 1);
                        }
                        this.favouriteCities.Add(cityName);
                    }
                }
            }
            if (this.favouriteCities.Contains(_currentCity))
            {
                isFavourite = true;
            }
            for (int i = this.favouriteCities.Count(); i < 5; i++)
            {
                if(this.favouriteItemsList[i] != null)
                {
                    this.favouriteItemsList[i].IsEnabled = false;
                }
            }
        }

        private void loadHistory()
        {
            this.historyItemsList = new List<MenuItem>()
            {
                hist_0, hist_1, hist_2, hist_3, hist_4
            };
            this.historyCities = new ObservableCollection<string>();
            string contents = File.ReadAllText(this.pathHistory);

            if (contents != null)
            {
                foreach (string cityName in contents.Split(','))
                {
                    if(cityName.Length > 1)
                    {
                        if (cityName[cityName.Length - 1] == '\n')
                        {
                            cityName.Remove(cityName.Length - 1);
                        }
                        this.historyCities.Add(cityName);
                    }
                }
            }
            for (int i = this.historyCities.Count(); i < 5; i++)
            {
                if (this.historyItemsList[i] != null)
                {
                    this.historyItemsList[i].IsEnabled = false;
                }
            }
        }

        public MainWindow()
        {
            InitialiseIcons();
            

            InitializeComponent();
            DataContext = this;
            HoursTemp = new ObservableCollection<HourTemp>();
            DaysTemp = new ObservableCollection<DayTemp>();
            CurrentWeather = new CurrentWeather();
            
            loadFavorites();
            loadHistory();
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
                HoursTemp.Clear();
                DaysTemp.Clear();
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
            int x = DaysTemp[4].hoursTemp.Count;
            if (x != 8)
            {
                for (int i = x; i < DaysTemp[3].hoursTemp.Count; i++)
                {
                    HourTemp ht = new HourTemp();
                    ht = DaysTemp[3].hoursTemp[i];
                    DaysTemp[4].hoursTemp.Add(ht);
                }
                if (DaysTemp[4].img1 == null)
                {
                    DaysTemp[4].img1 = DaysTemp[3].img1;
                } else if (DaysTemp[4].img2 == null)
                {
                    DaysTemp[4].img2 = DaysTemp[3].img2;
                }
                if (DaysTemp[4].temp.Equals("-100/100"))
                {
                    DaysTemp[4].temp = DaysTemp[3].temp;
                }
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
            _currentCity = currentLocation;
            changeFavouritesIcon();
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

        private void addToHistory()
        {
            this.historyCities.Insert(0, _currentCity);
            if (this.historyCities.Count() == 6)
            {
                this.historyCities.RemoveAt(5);
            }
            this.historyItemsList[this.historyCities.Count() - 1].IsEnabled = true;
            
            string newContent = "";
            foreach (string city in this.historyCities)
            {
                newContent = newContent + city + ',';
            }
            newContent = newContent.Remove(newContent.Length - 1);
            File.WriteAllText(this.pathHistory, newContent);
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                searchClick(sender, e);
            }
        }

        private void searchClick(object sender, RoutedEventArgs e)
        {
            changeFavouritesIcon();
            addToHistory();
            this.thread.Abort();
            startNewThread();
        }


        private void changeFavouritesIcon()
        {
            if (!this.favouriteCities.Contains(_currentCity))
            {
                addToFavBtn.Background = this.notFavouriteImg;
                addToFavBtn.ToolTip = "Remove from favourites";
                isFavourite = true;
            }
            else
            {
                addToFavBtn.Background = this.favouriteImg;
                addToFavBtn.ToolTip = "Add to favourites";
                isFavourite = false;
            }
        }

        private bool addCityToFavourites(string city)
        {
            if(this.favouriteCities.Count() == 5)   
            {
                MessageBox.Show("You tried to add new city to favourites list, it is not possible" + 
                    " because you can't have more than 5 favourite cities.\n" + 
                    "\nTo perform this action, remove at least one favourite city, and try again.",
                    "WARNING : city is not added to favourites.");
                return false;
            }else
            {
                this.favouriteCities.Add(city);
                this.favouriteItemsList[this.favouriteCities.Count()-1].IsEnabled = true;
                return true;
            }
        }


        private void addToFavoritesClick(object sender, RoutedEventArgs e)
        {
            string newContent = "";

            if (!this.favouriteCities.Contains(_currentCity))
            {
                // nije u favoritima, kliknuto je, znaci treba da bude puna zvezda i da se doda u favorite
                if (addCityToFavourites(_currentCity))
                {
                    addToFavBtn.Background = this.favouriteImg;
                    addToFavBtn.ToolTip = "Remove from favourites";

                    foreach (string city in this.favouriteCities)
                    {
                        newContent = newContent + city + ',';
                    }
                    newContent = newContent.Remove(newContent.Length - 1);
                    File.WriteAllText(this.pathFavourites, newContent);
                    isFavourite = true;
                }
            }
            else
            {
                addToFavBtn.Background = this.notFavouriteImg;
                addToFavBtn.ToolTip = "Add to favourites";
                this.favouriteCities.Remove(_currentCity);
                this.favouriteItemsList[this.favouriteCities.Count()].IsEnabled = false;
                foreach (string city in this.favouriteCities)
                {
                    newContent = newContent + city + ',';
                }
                newContent = newContent.Remove(newContent.Length - 1);
                File.WriteAllText(this.pathFavourites, newContent);
                isFavourite = false;
            }

        }
        private void moveToFav(int i)
        {
            if (this.favouriteCities[i].Length > 1)
            {
                this._currentCity = this.favouriteCities[i];
                changeFavouritesIcon();
                this.thread.Abort();
                startNewThread();
            }else
            {
                return;
            }
        }

        private void fav_0_Click(object sender, RoutedEventArgs e)
        {
            moveToFav(0);
        }
        private void fav_1_Click(object sender, RoutedEventArgs e)
        {
            moveToFav(1);
        }
        private void fav_2_Click(object sender, RoutedEventArgs e)
        {
           moveToFav(2);
        }
        private void fav_3_Click(object sender, RoutedEventArgs e)
        {
            moveToFav(3);
        }
        private void fav_4_Click(object sender, RoutedEventArgs e)
        {
            moveToFav(4);
        }


        private void moveToHist(int i)
        {
            if (this.historyCities[i].Length > 1)
            {
                this._currentCity = this.historyCities[i];
                changeFavouritesIcon();
                this.thread.Abort();
                startNewThread();
            }
            else
            {
                return;
            }
        }

        private void hist_0_Click(object sender, RoutedEventArgs e) { moveToHist(0); }
        private void hist_1_Click(object sender, RoutedEventArgs e) { moveToHist(1); }
        private void hist_2_Click(object sender, RoutedEventArgs e) { moveToHist(2); }
        private void hist_3_Click(object sender, RoutedEventArgs e) { moveToHist(3); }
        private void hist_4_Click(object sender, RoutedEventArgs e) { moveToHist(4); }

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
            thread.Start(_currentCity);
        }
    }
}

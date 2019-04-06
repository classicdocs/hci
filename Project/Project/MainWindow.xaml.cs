using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private String currentLocation = "";
        private Weather weather;

        public MainWindow()
        {
            InitializeComponent();
            ApiHandler.InitializeClient();
        }

        private void LoadWeather()
        {
            var weather =  ApiProcessor.LoadWeather(currentLocation);
        }

        private async Task LoadLocation()
        {
            var city = await ApiProcessor.LoadLocation();
            currentLocation = city.city;
            //location.Content = currentLocation;
        }

        private void refresh_click(object sender, RoutedEventArgs e)
        {
            LoadWeather();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadLocation();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void searchClick(object sender, RoutedEventArgs e)
        {
            searchBox.Visibility = Visibility.Visible;
            MyFavouritesBox.Visibility = Visibility.Hidden;
            HistoryBox.Visibility = Visibility.Hidden;
            
        }
        private void favouritesClick(object sender, RoutedEventArgs e)
        {
            if (MyFavouritesBox.Visibility == Visibility.Visible)
            {
                MyFavouritesBox.Visibility = Visibility.Hidden;

            }
            else
            {
                MyFavouritesBox.Visibility = Visibility.Visible;
                HistoryBox.Visibility = Visibility.Hidden;
            }
        }
        private void historyClick(object sender, RoutedEventArgs e)
        {
            if (HistoryBox.Visibility == Visibility.Visible)
            {
                HistoryBox.Visibility = Visibility.Hidden;
            }
            else
            {
                HistoryBox.Visibility = Visibility.Visible;
                MyFavouritesBox.Visibility = Visibility.Hidden;
            }
        }


    }
}

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
            location.Content = currentLocation;
        }

        private void refresh_click(object sender, RoutedEventArgs e)
        {
            LoadWeather();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadLocation();
        }

    }
}

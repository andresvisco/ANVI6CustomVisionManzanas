using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using App4;
using App5;
using Windows.Networking.Connectivity;
using System.Net.NetworkInformation;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace App5
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    
    public sealed partial class MainNueva : Page
    {
        public MainNueva()
        {
            this.InitializeComponent();


            ContentFrame.Navigate(typeof(HomePage));
            Windows.Storage.ApplicationDataContainer localSettings =
    Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.StorageFolder localFolder =
                Windows.Storage.ApplicationData.Current.LocalFolder;
            localSettings.Values["apiKey"] = "5ff19b57095a4d10bf64274ed9e6ef30";
            localSettings.Values["apiKeyCV"] = "47826cdef9984c8faa9cd47be4dd3c79";
            localSettings.Values["PredictionKey"] = "47826cdef9984c8faa9cd47be4dd3c79";



        }





        private void navView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {



            if (args.IsSettingsInvoked)
            {
              ContentFrame.Navigate(typeof(Settings));
            }
            else
            {
                var item = sender.MenuItems.OfType<NavigationViewItem>().First(x => (string)x.Content == (string)args.InvokedItem);
                NavView_Navigate(item as NavigationViewItem);
            }
        }
        private void NavView_Navigate(NavigationViewItem item)
        {

            switch (item.Tag)
            {

                
                case "customvisionObject":
                    ContentFrame.Navigate(typeof(CustomVisionObjects));
                    break;
                case "home":
                    ContentFrame.Navigate(typeof(HomePage));
                    break;



            }
        }
        private void navView_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void navView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {

        }

        private void check_Click(object sender, RoutedEventArgs e)
        {
            if (Connection.HasInternetAccess)
            {
                navView.IsEnabled = true;
            }
            else
            {
                navView.IsEnabled = false;
            }
        }
    }
}

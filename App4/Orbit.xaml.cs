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
using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Threading.Tasks;
using Windows.UI.Xaml.Shapes;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.Collections;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace App5
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
   

    public sealed partial class Orbit : Page
    {
            //{
        //    Microsoft.Toolkit.Uwp.UI.Controls.OrbitViewDataItem orbitViewDataItem = new Microsoft.Toolkit.Uwp.UI.Controls.OrbitViewDataItem()
        //    {
        //        Diameter = 0.5
        //    };

        //}
        public OrbitViewDataItemCollection orbitViewDataItems = new OrbitViewDataItemCollection();
        public OrbitViewDataItem orbitViewDataItemCenter = new OrbitViewDataItem();
        
        public Orbit()
        {
            this.InitializeComponent();
            
            //Microsoft.Toolkit.Uwp.UI.Controls.OrbitViewDataItem orbitViewDataItem = new  Microsoft.Toolkit.Uwp.UI.Controls.OrbitViewDataItem();
            //for (int i = 1; i < 6; i++)
            //{
            //    orbitViewDataItem = new Microsoft.Toolkit.Uwp.UI.Controls.OrbitViewDataItem() {
            //        Label = "Elemento " + i.ToString(),
            //        Distance = (double)1 / (double)i,
            //        Diameter = 0.2,
            //        Image = new BitmapImage(new Uri("ms-appx:///Assets/Icono1240.png")),
            //        Item = "Item "+i.ToString()                
            //    };
            //    orbitViewDataItems.Add(orbitViewDataItem);


            //}


                     



        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var parameter = e.Parameter as OrbitViewDataItemCollection;
            orbitViewDataItems = parameter;



            var itemCenter = orbitViewDataItems.OrderByDescending(x => x.Diameter).First();

            orbitViewDataItemCenter.Diameter = itemCenter.Diameter * (double)10;
            orbitViewDataItemCenter.Distance = itemCenter.Distance;
            orbitViewDataItemCenter.Label = itemCenter.Label;
            orbitViewDataItemCenter.Image = itemCenter.Image;
            orbitViewDataItemCenter.Item = itemCenter.Item;

            foreach (var item in orbitViewDataItems)
            {
                item.Distance = item.Distance - itemCenter.Distance ;

            }

            orbOrbit.ItemsSource = orbitViewDataItems;

        }
        



    }
    public class StringFormatConverter : IValueConverter
    {
        public string StringFormat { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!String.IsNullOrEmpty(StringFormat))
                return String.Format(StringFormat, value);

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}

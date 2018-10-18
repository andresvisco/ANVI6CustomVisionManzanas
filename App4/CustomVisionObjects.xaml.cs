using Microsoft.Toolkit.Uwp.UI.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using App5;
using System.Threading;
using Windows.Graphics.Imaging;
using Windows.UI.Input;

using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using System.Collections;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace App5
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CustomVisionObjects : Page
    {
        public double ConstanteProporcion = 1;
        public static ICollection<string> vs = new Collection<string>() { "f408e8db-09d5-4b75-a3da-09d40a62a848" };//{ "aec426b4-759a-4486-ba4e-27a43b4d01d1" }; //{ "407a3dc2-dae0-4c89-8b62-4d0dee7cb29b" };
        public string vsproject = string.Empty;
        
        Windows.Storage.ApplicationDataContainer localSettings =
       Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder localFolder =
            Windows.Storage.ApplicationData.Current.LocalFolder;
        public IRandomAccessStream randomAccessStreamImage;
        public List<StoredImagePrediction> StoredImages() {
            List<StoredImagePrediction> storedImagePredictionsLista = new List<StoredImagePrediction>();

            return storedImagePredictionsLista;
        }
        public List<PredictionQueryTag> PredictionQueryTags()
        {
            List<PredictionQueryTag> predictionQueryTags = new List<PredictionQueryTag>();
            return predictionQueryTags;
        }

        public double width;
        public double heigth;
        public List<string> lstObjetos = new List<string>() { "Manzanas" };
        public string elementoElegidoAnalisis = string.Empty;
        public static List<Tuple<OrbitViewDataItemCollection,Guid>> resultadosDistancia = new List<Tuple<OrbitViewDataItemCollection, Guid>>();
        public static string TagSelected = string.Empty;
        public Guid TagSelectedGuid = new Guid();
        public static string idPredictedImage = string.Empty;
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // Populate the scenario list from the SampleConfiguration.cs file
            tags.Clear();
            await ObtenerTags();
            var elementosTag = tags;
            lstTags.ItemsSource = tags;
            TagSelected = tags.First().ToString();

        }
        public CustomVisionObjects()
        {
            this.InitializeComponent();
            cnvCanvas.Children.Clear();
            


            lstView.ItemsSource = lstObjetos;
            if (elementoElegidoAnalisis == string.Empty)
            {
                btnCargar.IsEnabled = false;
            }
            else
            {
                btnCargar.IsEnabled = true;
            }

        }
        public Windows.Storage.StorageFile storageFile;
        public double constante = 1;
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ItemsNuevos.Clear();
            cnvCanvasSelected.Children.Clear();
            Items.Clear();
            cnvCanvas.Children.Clear();
            LoadingControl.IsLoading = true;
            resultadosDistancia.Clear();
          
            Windows.Storage.Pickers.FileOpenPicker openPicker = new Windows.Storage.Pickers.FileOpenPicker();
            openPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            openPicker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            openPicker.FileTypeFilter.Clear();
            openPicker.FileTypeFilter.Add(".bmp");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".jpg");
            storageFile = await openPicker.PickSingleFileAsync();
            

            if (storageFile != null)
            {
                var stream = await storageFile.OpenAsync(FileAccessMode.Read);
                randomAccessStreamImage = stream;
                Stream streamImage = randomAccessStreamImage.AsStream();
                byte[] b;

                using (BinaryReader br = new BinaryReader(streamImage))
                {
                    b = br.ReadBytes((int)streamImage.Length);
                }
                byte[] byteArray;
                using (Windows.Storage.Streams.IRandomAccessStream fileStream = await storageFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    

                    Windows.UI.Xaml.Media.Imaging.BitmapImage bitmapImage = new Windows.UI.Xaml.Media.Imaging.BitmapImage();
                    bitmapImage.SetSource(fileStream);
                    width = bitmapImage.PixelWidth;
                    heigth = bitmapImage.PixelHeight;
                    if (width > 400)
                    {
                        constante = 1 / (width / 600);

                        width = width * constante;
                        heigth= heigth * constante;
                    }
                    byteArray = new byte[fileStream.AsStream().Length];



                    cnvCanvas.Width = (int)width;
                    cnvCanvas.Height = (int)heigth;
                    cnvCanvasSelected.Width = (int)width;
                    cnvCanvasSelected.Height = (int)heigth;

                    imagePreview.Width = (int)width;
                    imagePreview.Height = (int)heigth;
                    imagePreview.Source = bitmapImage;
                    
                    imagePreview.Opacity = 0.4;


                    vsproject = vs.Last().ToString();


                    try
                    {
                        var client = new HttpClient();
                        client.DefaultRequestHeaders.Add("Prediction-Key", localSettings.Values["apiKeyCV"] as string);
                        ByteArrayContent bytearrayContent = new ByteArrayContent(b);
                        bytearrayContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                        var projecto = vsproject;
                        var byteArrayNuevo = bytearrayContent;
                        var result = await client.PostAsync("https://southcentralus.api.cognitive.microsoft.com/customvision/v2.0/Prediction/" + vsproject + "/image", bytearrayContent);
                        if (result.IsSuccessStatusCode)
                        {
                            var response = result.Content.ReadAsStringAsync().Result;

                            dynamic obj = JsonConvert.DeserializeObject<dynamic>(response);
                            int contador = 0;
                            var dynamicImagePredict = obj.id;
                            idPredictedImage = (string)dynamicImagePredict;

                            foreach (var item in obj.predictions)
                            {
                                var resultadoCaja = item.boundingBox;
                                var resultadoTag = item.tagName;
                                if (item.probability * 100 >= 55)
                                {
                                    imagePreview.Opacity = 1;
                                    imagePreview.Visibility = Visibility.Visible;
                                    dibujarCaja(resultadoCaja, resultadoTag, item.probability);

                                    
                                    LoadingControl.IsLoading = false;

                                    contador += 1;
                                    
                                };

                            }
                            txtResult.Text = "Se encontraron: " + contador.ToString() + " elementos";
                           
                          
                        }


                    }
                    catch (Exception ex)
                    {
                        var error = ex.Message.ToString();
                        throw;
                    }

                }
                
                
            }
        }
        public int contador = 0;
        public double cropWidth;
        public double cropHeight;
        public static OrbitViewDataItemCollection orbitViewDataItems = new OrbitViewDataItemCollection();
        public static OrbitViewDataItemCollection orbitViewDataItemCollection = new OrbitViewDataItemCollection();
        public List<Microsoft.Toolkit.Uwp.UI.Controls.OrbitViewDataItem> orbitViewDataItemsList = new List<Microsoft.Toolkit.Uwp.UI.Controls.OrbitViewDataItem>();
        private async Task dibujarCaja(dynamic ubicacion, dynamic tagName, dynamic probability)
        {
            contador += 1;
            SolidColorBrush solidColorBrush = new SolidColorBrush();


            solidColorBrush.Color = Windows.UI.Colors.Yellow;


            Rectangle rectangle = new Rectangle();
            Thickness thickness = new Thickness();

            string tagEncontrado = (string)tagName;

            rectangle.Fill = null;
            rectangle.Stroke = solidColorBrush;

       

            var constanteAmplitudWidth = width;
            var constanteAmplitudHeight = heigth;

            double ubicacionLeft = (double)ubicacion.left * constanteAmplitudWidth;
            double ubicacionTop = (double)ubicacion.top * constanteAmplitudHeight;
            double ubicacionWidth = (double)ubicacion.width * constanteAmplitudWidth;
            double ubicacionHeight = (double)ubicacion.height * constanteAmplitudHeight;
            Point pointCaja = new Point();
            pointCaja.X = ubicacionLeft;
            pointCaja.Y = ubicacionTop;
            Size sizeCaja = new Size();
            sizeCaja.Width = ubicacionWidth;
            sizeCaja.Height = ubicacionHeight;

            var items = await CortarImagenesTageadas.ImagenACortar(pointCaja, sizeCaja, storageFile, imagePreview.ActualWidth);
            Items.Add(new Tuple<ImageSource, Point, Size>(items,pointCaja,sizeCaja));
            lstViewImagenesTageadas.ItemTemplate.SetValue(WidthProperty, sizeCaja.Width);
            lstViewImagenesTageadas.ItemTemplate.SetValue(HeightProperty, sizeCaja.Height);

            Microsoft.Toolkit.Uwp.UI.Controls.OrbitViewDataItem orbitViewDataItem = new Microsoft.Toolkit.Uwp.UI.Controls.OrbitViewDataItem();
            orbitViewDataItem.Distance = ubicacionLeft / 10;
            orbitViewDataItem.Diameter = (double)probability*(double)2;
            orbitViewDataItem.Label = tagEncontrado + " - " + contador.ToString() ;

            BitmapImage bitmapImage = new BitmapImage();
            Uri uri = new Uri("ms-appx:///Assets/Icono1240.png");
            bitmapImage.UriSource = uri;



            orbitViewDataItem.Image = bitmapImage;
            orbitViewDataItem.Item = tagEncontrado + " - " + probability.ToString();
            orbitViewDataItemCollection.Add(orbitViewDataItem);

            Guid guid;
            guid = Guid.NewGuid();
            resultadosDistancia.Add(new Tuple<OrbitViewDataItemCollection, Guid>(orbitViewDataItemCollection, guid));
            //orbitViewDataItemsList.Add(orbitViewDataItem);

            thickness.Left = ubicacionLeft;
            thickness.Top = ubicacionTop;
            rectangle.Margin = thickness;
            rectangle.Width = ubicacionWidth;
            rectangle.Height = ubicacionHeight;
            cnvCanvas.Children.Add(rectangle);


            Thickness thicknessTexto = new Thickness();
            thicknessTexto.Left = ubicacionLeft;
            thicknessTexto.Top = ubicacionTop - 10;

            string textoResultado = probability * 100;

            SolidColorBrush solidColorBrushTexto = new SolidColorBrush();
            SolidColorBrush solidColorBrushTextoBG = new SolidColorBrush();
            solidColorBrushTextoBG.Color = Windows.UI.Colors.Transparent;
            solidColorBrushTexto.Color = Windows.UI.Colors.Black;


            TextBox textBox = new TextBox();
            textBox.Text = (string)tagName + " - " + textoResultado.Substring(0, 4).ToString() + " %";
            textBox.Margin = thicknessTexto;
            textBox.FontSize = 10;
            textBox.Padding = new Thickness(0, 0, 0, 0);
            textBox.BorderThickness = new Thickness(0, 0, 0, 0);
            textBox.FocusVisualPrimaryBrush = solidColorBrushTexto;
            textBox.Foreground = solidColorBrushTexto;
            textBox.Background = solidColorBrush;
            textBox.Height = 13;
            textBox.MaxHeight = 13;
            textBox.MinHeight = 13;
            textBox.MaxLength = 10;

            cnvCanvas.Children.Add(textBox);


        }

        private async void lstView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tags.Clear();
            
            elementoElegidoAnalisis = this.lstView.SelectedItem.ToString();
            btnCargar.IsEnabled = true;
            vs.Clear();
            switch (elementoElegidoAnalisis)
            {
                case "Manzanas":
                    vs.Add("f408e8db-09d5-4b75-a3da-09d40a62a848");
                    break;
      
            }
            await ObtenerTags();
            lstTags.ItemsSource = tags;
            lstTags.UpdateLayout();

            Button_Click((object)sender, (RoutedEventArgs)e);
            
        }
        public async Task ObtenerImagen()
        {
        }
        private void Button_ClickOrbit(object sender, RoutedEventArgs e)
        {
            ContentFrame.Visibility = Visibility.Visible;
            ContentFrame.Navigate(typeof(Orbit),orbitViewDataItemCollection);

        }

        private void Button_ClickOrbitCerrar(object sender, RoutedEventArgs e)
        {
            this.ContentFrame.Visibility = Visibility.Collapsed;
        }
        public  ObservableCollection<Tuple<Windows.UI.Xaml.Media.ImageSource, Point, Size>> _items = new ObservableCollection<Tuple<Windows.UI.Xaml.Media.ImageSource, Point, Size>>();
        public  ObservableCollection<Tuple<Windows.UI.Xaml.Media.ImageSource, Point, Size>> Items
        {
            get { return _items; }
        }
        public ObservableCollection<Tuple<Windows.UI.Xaml.Media.ImageSource, Point, Size>> _itemsNuevos = new ObservableCollection<Tuple<Windows.UI.Xaml.Media.ImageSource, Point, Size>>();
        public ObservableCollection<Tuple<Windows.UI.Xaml.Media.ImageSource, Point, Size>> ItemsNuevos
        {
            get { return _itemsNuevos; }
        }
        class ElementosAgregados
        {
            public ImageSource ImageSource { get; set; }
            public Point PointSource { get; set; }
        }
        private async void lstViewImagenesTageadas_ItemClick(object sender, ItemClickEventArgs e)
        {
            
            var point = ((System.Tuple<Windows.UI.Xaml.Media.ImageSource, Windows.Foundation.Point, Size>)e.ClickedItem).Item2;
            var pointSizeHeigth = ((System.Tuple<Windows.UI.Xaml.Media.ImageSource, Windows.Foundation.Point, Windows.Foundation.Size>)e.ClickedItem).Item3.Height;
            var pointSizeWidth = ((System.Tuple<Windows.UI.Xaml.Media.ImageSource, Windows.Foundation.Point, Windows.Foundation.Size>)e.ClickedItem).Item3.Width;

            SolidColorBrush solidColorBrush = new SolidColorBrush();
            solidColorBrush.Color = Windows.UI.Colors.BlueViolet;
            Rectangle rectangleSelect = new Rectangle();
            Thickness thicknessSelect = new Thickness();
            thicknessSelect.Left = point.X;
            thicknessSelect.Top = point.Y;
            rectangleSelect.Margin = thicknessSelect;
            rectangleSelect.Width = pointSizeWidth;
            rectangleSelect.Height = pointSizeHeigth;
            rectangleSelect.StrokeThickness = 2;
            rectangleSelect.Stroke = solidColorBrush;
            rectangleSelect.FocusVisualPrimaryBrush = solidColorBrush;
            var solidOpacity = solidColorBrush;
            solidOpacity.Opacity=0.4;

            rectangleSelect.Fill = solidOpacity;

            cnvCanvasSelected.Children.Add(rectangleSelect);
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            cnvCanvasSelected.Children.Clear();

        }

 

      
        public Point pointDragInicio;
        public Point pointDragFin;
        public Size sizeDrag;
        public PointerPoint Point1, Point2;

        private void ImagePreview_DragEnter(object sender, PointerRoutedEventArgs e)
        {
            Point1 = e.GetCurrentPoint(imagePreview);
            

        }

        private async void ImagePreview_DragLeave(object sender, PointerRoutedEventArgs e)
        {
            Point2 = e.GetCurrentPoint(imagePreview);
            Rectangle rect = new Rectangle();
            rect.Width = (int)Math.Abs(Point2.Position.X - Point1.Position.X);
            rect.Height = (int)Math.Abs(Point2.Position.Y - Point1.Position.Y);
            rect.SetValue(Canvas.LeftProperty, (Point1.Position.X < Point2.Position.X) ? Point1.Position.X : Point2.Position.X);
            rect.SetValue(Canvas.TopProperty, (Point1.Position.Y < Point2.Position.Y) ? Point1.Position.Y : Point2.Position.Y);
            Task.Delay(100);
            RectangleGeometry geometry = new RectangleGeometry();
            geometry.Rect = new Rect(Point1.Position, Point2.Position);
            rect.StrokeThickness = 1;
            rect.Stroke = new SolidColorBrush(Windows.UI.Colors.BlueViolet);

            cnvCanvas.Children.Add(rect);

            Point pointCaja = new Point();
            //pointCaja.X = Point1.Position.X;
            //pointCaja.Y = Point2.Position.Y;
            pointCaja.X = geometry.Rect.X;
            pointCaja.Y = geometry.Rect.Y;
            Size sizeCaja = new Size();

            sizeCaja.Width = rect.Width;
            sizeCaja.Height = rect.Height;


            //imagePreview.Clip = geometry;

            var itemsNuevos = await CortarImagenesTageadas.ImagenACortar(pointCaja, sizeCaja, storageFile, imagePreview.ActualWidth);
            ItemsNuevos.Add(new Tuple<ImageSource, Point, Size>(itemsNuevos, pointCaja, sizeCaja));

            //var imagenCropeada = await CortarImagenesTageadas.ImagenACortar(pointDragInicio, sizeDrag, storageFile, width);

        }
        public Dictionary<string,Region> region = new Dictionary<string,Region>()
        {             
        };

        private async void lstViewImagenesTageadasNuevas_ItemClick(object sender, ItemClickEventArgs e)
        {
            var point = ((System.Tuple<Windows.UI.Xaml.Media.ImageSource, Windows.Foundation.Point, Size>)e.ClickedItem).Item2;
            var pointSizeHeigth = ((System.Tuple<Windows.UI.Xaml.Media.ImageSource, Windows.Foundation.Point, Windows.Foundation.Size>)e.ClickedItem).Item3.Height;
            var pointSizeWidth = ((System.Tuple<Windows.UI.Xaml.Media.ImageSource, Windows.Foundation.Point, Windows.Foundation.Size>)e.ClickedItem).Item3.Width;

            SolidColorBrush solidColorBrush = new SolidColorBrush();
            solidColorBrush.Color = Windows.UI.Colors.OrangeRed;
            Rectangle rectangleSelect = new Rectangle();
            Thickness thicknessSelect = new Thickness();
            thicknessSelect.Left = point.X;
            thicknessSelect.Top = point.Y;
            rectangleSelect.Margin = thicknessSelect;
            rectangleSelect.Width = pointSizeWidth;
            rectangleSelect.Height = pointSizeHeigth;
            rectangleSelect.StrokeThickness = 2;
            rectangleSelect.Stroke = solidColorBrush;
            rectangleSelect.FocusVisualPrimaryBrush = solidColorBrush;
            var solidOpacity = solidColorBrush;
            solidOpacity.Opacity = 0.4;

            rectangleSelect.Fill = solidOpacity;

            cnvCanvasSelected.Children.Add(rectangleSelect);
        }



        


        private void cmbTags_Loaded(object sender, RoutedEventArgs e)
        {

            var selectTagSelected = this.lstTags.SelectedItem;
            if (selectTagSelected != null)
            {
                TagSelected = ((App5.Tag)lstTags.SelectedItem).IdTag.ToString();
                txtTag.Text = TagSelected;

            }
            else
            {
                TagSelected = null;
            }
        }

        private void cmbTags_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectTagSelected = this.lstTags.SelectedItem;
            if (selectTagSelected != null)
            {
                TagSelected = ((App5.Tag)lstTags.SelectedItem).IdTag.ToString();
                txtTag.Text = TagSelected;
            }
            else
            {
                TagSelected = null; 
            }

        }



        private async void btnSubirNuevosTags_Click(object sender, RoutedEventArgs e)
        {
            TrainingApi trainingApi = new TrainingApi() { ApiKey = localSettings.Values["apiKeyCVTraining"] as string };
            var domains = await trainingApi.GetDomainsAsync();
            var objDetectionDomain = domains.FirstOrDefault(d => d.Type == "ObjectDetection");
            var project = vs.First().ToString();

            try
            {

                Iteration iteration = new Iteration();
                var iterationsList = await trainingApi.GetIterationsAsync(new Guid(vsproject));
                var iterationUltima = iterationsList.Last().ToString();
                ListaTags listaTags = new ListaTags();
                var TagsPred = await trainingApi.GetTagsAsync(new Guid(vsproject));

                PredictionQueryTag predictionQueryTag = new PredictionQueryTag(new Guid(TagsPred.Last().Id.ToString()), 0.45, 1);

                StoredImagePrediction storedImagePredictiong = new StoredImagePrediction();
                listaTags.Add(predictionQueryTag);
                
                PredictionQueryToken predictionToken = new PredictionQueryToken() { Application = null, Continuation = null, EndTime = null, StartTime = null, IterationId = new Guid(iterationUltima), MaxCount = 100, OrderBy = "Newest", Session = null, Tags = listaTags   };
                PredictionQueryResult predictionQueryResult = new PredictionQueryResult(predictionToken, StoredImages());

                var imagePath = System.IO.Path.Combine("Images", "fork");
                var imageFileEntries = new List<ImageIdCreateEntry>();
                ImageIdCreateEntry imageIdCreateEntry = new ImageIdCreateEntry();


                foreach (var item in region)
                {
                    imageIdCreateEntry.Id = new Guid(idPredictedImage);
                    //imageFileEntries.Add(new ImageFileCreateEntry(item, File.ReadAllBytes(item), null, new List<Region>(new Region[] { new Region(tagId, region[0], region[1], region[2], region[3])})));


                }


                ImageRegionCreateBatch imageRegionCreateBatch = new ImageRegionCreateBatch();

            }
            catch (Exception ex)
            {
                var error = ex.Message.ToString();
            }
           
        }

        private void imagePreview_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Point2 = e.GetCurrentPoint(imagePreview);
        }
        class ListaTags : IList<PredictionQueryTag>
        {
            public PredictionQueryTag this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            private PredictionQueryTag _contents = new PredictionQueryTag();
            private int _count;
          

            public int Count => throw new NotImplementedException();
            public ListaTags()
            {
                _count = 0;
            }
            public bool IsReadOnly => throw new NotImplementedException();

            public void Add(PredictionQueryTag item)
            {
                    _contents = item;
                
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(PredictionQueryTag item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(PredictionQueryTag[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<PredictionQueryTag> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public int IndexOf(PredictionQueryTag item)
            {
                throw new NotImplementedException();
            }

            public void Insert(int index, PredictionQueryTag item)
            {
                throw new NotImplementedException();
            }

            public bool Remove(PredictionQueryTag item)
            {
                throw new NotImplementedException();
            }

            public void RemoveAt(int index)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
    }




}

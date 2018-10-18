using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Windows.UI.Xaml.Controls;
using App5;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace App5
{
    public partial class CustomVisionObjects : Page
    {

        public static ObservableCollection<Tag> tags = new ObservableCollection<Tag>
        {
            new Tag()
            {
                IdTag = "aaaa",
                NameTag="hbbb"
            }

        };
        
        public async Task<bool> ObtenerTags()
        {

            localSettings.Values["apiKey"] = "5ff19b57095a4d10bf64274ed9e6ef30";
            localSettings.Values["apiKeyCV"] = "47826cdef9984c8faa9cd47be4dd3c79";
            localSettings.Values["apiKeyCVTraining"] = "57b3c94601f5471fad32d2f239541f5a";

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            string httpResponseBody = string.Empty;
            string projectCustomVision = CustomVisionObjects.vs.Last().ToString();
            Uri requestUri = new Uri("https://southcentralus.api.cognitive.microsoft.com/customvision/v1.1/Training/projects/" + projectCustomVision + "/tags");

            httpClient.DefaultRequestHeaders.Add("Training-Key", localSettings.Values["apiKeyCVTraining"] as string);
            try
            {
                var result = await httpClient.GetAsync(requestUri);
                
                if (result.IsSuccessStatusCode)
                {
                    var response = result.Content.ReadAsStringAsync().Result;

                    dynamic obj = JsonConvert.DeserializeObject<dynamic>(response);
                    foreach (var item in obj.Tags)
                    {
                        string id = item.Id;
                        string nombre = item.Name;

                        Tag TagAgregar = new Tag()
                        {
                            IdTag = id,
                            NameTag = nombre
                        };
                        tags.Add(TagAgregar);

                        
                    }
                    
                }
            }
            catch (HttpRequestException ex)
            {
                var error = ex.Message.ToString();

            }
            return true;
            


        }



    }
    public class Tag
    {
        public string IdTag { get; set; }
        public string NameTag { get; set; }
        
    }
}

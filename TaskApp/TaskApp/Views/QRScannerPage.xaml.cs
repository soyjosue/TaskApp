using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TaskApp.Helper;
using TaskAppBackend.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TaskApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QRScannerPage : ContentPage
    {
        private bool _isScanning = true;

        public QRScannerPage()
        {
            InitializeComponent();
        }

        private void ZXingScannerView_OnScanResult(ZXing.Result result)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (_isScanning)
                {
                    _isScanning = false;
                    scanner.IsAnalyzing = false;
                    Uri uri = new Uri($"{Literals.WEBAPIKEY}/ProyectApi/AddMember");

                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Add(Literals.TOKEN, Utils.GetToken());

                    var sharedProyect = JsonConvert.DeserializeObject<SharedProyect>(result.Text);
                    var json = Utils.ConvertJson(sharedProyect);

                    try
                    {
                        var response = await client.PostAsync(uri, json);

                        var message = await response.Content.ReadAsStringAsync();

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            await Navigation.PopToRootAsync();
                            await DisplayAlert("Se agrego al proyecto", "Usted ha sido agregado correctamente al proyecto", "Ok");
                        }
                        else
                        {
                            await DisplayAlert("Codigo QR invalido", "Verifique si el codigo QR del proyecto no ha cambiado.", "Ok");
                        }

                    }
                    catch
                    {
                        await DisplayAlert("No se pudo agregar al proyecto", "Verifique su conexión a internet.", "OK");
                    }

                    scanner.IsAnalyzing = true;
                    _isScanning = true;
                }
            });
        }
    }
}
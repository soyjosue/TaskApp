using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApi.Models;
using Xamarin.Forms;

namespace TaskApp.Helper
{
    public static class Utils
    {

        public static StringContent ConvertJson(Object element)
        {
            var json = JsonConvert.SerializeObject(element);
            var convertJson = new StringContent(json, Encoding.UTF8, "application/json");

            return convertJson;
        }

        public static string GetToken()
        {
            return App.SQLiteDB.GetValueConfigUser(Literals.TOKEN);
        }

        public async static Task<User> GetUser()
        {
            Uri requestUri = new Uri($"{Literals.WEBAPIKEY}/UserApi/");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(Literals.TOKEN, Utils.GetToken());

            try
            {
                var response = await client.GetAsync(requestUri);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());

            }
            catch
            {
            }

            return null;
        }
    }
}

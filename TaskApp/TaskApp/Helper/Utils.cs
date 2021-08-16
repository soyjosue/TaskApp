using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
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
    }
}

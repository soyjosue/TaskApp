using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using WebApi.Data;

namespace WebApi.Helper
{
    public static class Utils
    {
        public static string GetHeaderElement(string nameItem, HttpRequestMessage request)
        {
            string itemValue = "";
            foreach (var item in request.Headers)
            {
                if (item.Key.Equals("token"))
                {
                    itemValue = item.Value.First();
                    break;
                }
            }

            return itemValue;
        } 

        public static bool IsUserExist(string id, TaskWebApiContext db)
        {
            var user = (from usr in db.Users
                        where usr.Id.ToString() == id
                        select usr).ToList();

            return user.Count() > 0;
        }
    }
}
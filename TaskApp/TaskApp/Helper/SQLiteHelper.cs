using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TaskApp.Models;

namespace TaskApp.Helper
{
    public class SQLiteHelper
    {
        SQLiteAsyncConnection db;

        public SQLiteHelper(string dbPath)
        {
            db = new SQLiteAsyncConnection(dbPath);
            db.CreateTableAsync<ConfigUser>().Wait();
        }

        public Task<int> SaveConfigAsync(ConfigUser configUser)
        {
            var config = db.Table<ConfigUser>().Where(c => c.Key == configUser.Key).FirstOrDefaultAsync();

            if (config.Result == null)
                return db.InsertAsync(configUser);
            else
            {
                config.Result.Value = configUser.Value;
                return db.UpdateAsync(config.Result);
            }
                

        }

        /// <summary>
        /// Obtener por el key un de las configuraciones del usuario
        /// </summary>
        /// <param name="key">Key unica de la configuración</param>
        /// <returns></returns>

        public string GetValueConfigUser(string key)
        {
            var config = (db.Table<ConfigUser>().Where(c => c.Key == key).FirstOrDefaultAsync()).Result;
            if (config == null)
                return null;
            else
                return config.Value;
        }

        public Task<int> RemoveConfigUserAsync(string key)
        {
            var config = db.Table<ConfigUser>().Where(c => c.Key == key).FirstOrDefaultAsync();
            return db.DeleteAsync(config.Result);
        }
    }
}

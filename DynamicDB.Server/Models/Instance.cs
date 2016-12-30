using Compete.DynamicDB.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Timers;

namespace Compete.DynamicDB.Models
{
    public sealed class Instance : ItemDictionary<Database>
    {
        private static int saveInterval = 60000;

        private static string dataPath = ConfigurationManager.AppSettings["DataPath"];

        private Timer saveTimer = new Timer(saveInterval);

        public ICollection<User> Users { get; private set; }

        public IDictionary<string, string> DatabasePaths { get; private set; }

        static Instance()
        {
            var interval = ConfigurationManager.AppSettings["SaveInterval"];
            if (!string.IsNullOrWhiteSpace(interval))
            {
                int saveIntervalMilliseconds;
                if (int.TryParse(interval, out saveIntervalMilliseconds))
                    saveInterval = saveIntervalMilliseconds;
            }
        }

        public Instance()
        {
            DatabasePaths = new Dictionary<string, string>();
            Users = new List<User>();

            if (string.IsNullOrWhiteSpace(dataPath))
                dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);

            Load();

            saveTimer.Elapsed += SaveTimer_Elapsed;
            saveTimer.Start();
        }

        private void SaveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Save();
            saveTimer.Start();
        }

        protected override bool Added(string name)
        {
            var path = Path.Combine(dataPath, Path.ChangeExtension(name, ".db"));
            ulong no = 0L;
            while(File.Exists(path))
            {
                path = Path.Combine(dataPath, Path.ChangeExtension(name + no.ToString(), ".db"));
                no++;
            }

            DatabasePaths.Add(name, path);

            if (Items[name].Save(path))
                return true;

            return base.Added(name);
        }

        public void Load()
        {
            var databasesPath = Path.Combine(dataPath, "Databases.db");
            if (!File.Exists(databasesPath))
                return;
            DatabasePaths = Commons.JsonHelper.Deserialize<Dictionary<string, string>>(databasesPath);

            var usersPath = Path.Combine(dataPath, "Users.db");
            if (!File.Exists(usersPath))
                return;
            Users = Commons.JsonHelper.Deserialize<List<User>>(usersPath);

            Database db;
            foreach (var item in DatabasePaths)
            {
                db = new Database();
                if (db.Load(item.Value))
                    continue;
                Items.Add(item.Key, db);
            }
        }

        public bool Save()
        {
            bool result = false;
            foreach(var item in DatabasePaths)
                if (Items[item.Key].Save(item.Value))
                    result = true;

            try
            {
                Users.SaveAs(Path.Combine(dataPath, "Users.db"));
            }
            catch
            {
                result = true;
            }

            try
            {
                DatabasePaths.SaveAs(Path.Combine(dataPath, "Databases.db"));
            }
            catch
            {
                result = true;
            }

            return result;
        }
    }
}

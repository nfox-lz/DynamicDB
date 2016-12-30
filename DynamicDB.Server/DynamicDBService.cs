using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Compete.DynamicDB
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“NetDBService”。
    public class DynamicDBService : IDynamicDBService
    {
        //private static int saveInterval = 60000;

        private static Models.Instance instance = new Models.Instance();

        private static readonly JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

        //private static Thread saveThread = new Thread(new ThreadStart(SaveDate));

        //private static void SaveDate()
        //{
        //    while (true)
        //    {
        //        Thread.Sleep(saveInterval);

        //        instance.Save();
        //    }
        //}

        //static DynamicDBService()
        //{
        //    var interval = ConfigurationManager.AppSettings["SaveInterval"];
        //    if (!string.IsNullOrWhiteSpace(interval))
        //    {
        //        int saveIntervalMilliseconds;
        //        if (int.TryParse(interval, out saveIntervalMilliseconds))
        //            saveInterval = saveIntervalMilliseconds;
        //    }

        //    saveThread.Start();
        //}

        public Guid Insert(string database, string collection, byte[] data)
        {
            var items = instance[database]?[collection];
            if (items == null)
                return Guid.Empty;

            
            return items.Add(DynamicEntityFrame.EntityGenerator.ToObject(javaScriptSerializer.Deserialize<IDictionary<string, object>>(Commons.CompressionHelper.DecompressToString(data)), collection));
        }

        public bool Update(string database, string collection, Guid id, byte[] data)
        {
            var items = instance[database]?[collection]; ;
            if (items?[id] == null)
                return false;

            return items.Modify(id, DynamicEntityFrame.EntityGenerator.ToObject(javaScriptSerializer.Deserialize<IDictionary<string, object>>(Commons.CompressionHelper.DecompressToString(data)), collection));
        }

        public bool Delete(string database, string collection, Guid id)
        {
            var items = instance[database]?[collection];
            if (items == null)
                return false;
            return items.Remove(id);
        }

        public bool Lock(string database, string collection)
        {
            var result = instance[database]?[collection].Lock();
            return result == null ? false : result.Value;
        }

        public void Unlock(string database, string collection)
        {
            instance[database]?[collection].Unlock();
        }

        public byte[] Get(string database, string collection, Guid id)
        {
            var item = instance[database]?[collection]?[id];
            return item == null ? null : Commons.CompressionHelper.Compress(JsonConvert.SerializeObject(item));

        }

        public byte[] LoadAll(string database, string collection)
        {
            var data = instance[database]?[collection];
            return data == null ? null : Commons.CompressionHelper.Compress(JsonConvert.SerializeObject(data));
        }

        public byte[] Query(string database, string script, string language = "C#")
        {
            object result;
            return (new Script.ScriptCompiler(instance, instance[database]) { Language = language }).ExecuteExpression(script, out result) ? Commons.CompressionHelper.Compress(JsonConvert.SerializeObject(result)): null;
        }


        public int ExecuteNonQuery(string script, string database = null)
        {
            return 0;
        }

        public byte[] Execute(string script, string database = null)
        {
            return null;
        }

        public bool CreateProcedure(string database, string name, string script, string language = "C#")
        {
            var db = instance[database];
            if (db == null)
                return true;

            return db.CreateProcedure(name, script, language);
        }

        public bool DropProcedure(string database, string name)
        {
            var db = instance[database];
            if (db == null)
                return true;

            return db.DropProcedure(name);
        }

        public int ExecuteProcedure(string database, string name, out object result, params object[] parameters)
        {
            result = null;
            return 0;
        }

        public bool Save()
        {
            return instance.Save();
        }

        public bool CreateDatabase(string name)
        {
            return instance.Add(name);
        }

        public bool DropDatabase(string name)
        {
            return instance.Remove(name);
        }

        public bool CreateCollection(string database, string name)
        {
            var db = instance[database];
            return db == null ? true : db.Add(name);
        }

        public bool DropCollection(string database, string name)
        {
            var db = instance[database];
            return db == null ? true : db.Remove(name);
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Compete.DynamicDB
{
    public partial class NetDatabase
    {
        public static bool CreateCollection(string database, string name)
        {
            return WcfServiceHelper.Execute(service => service.CreateCollection(database, name));
        }

        public static bool DropCollection(string database, string name)
        {
            return WcfServiceHelper.Execute(service => service.DropCollection(database, name));
        }

        public static Task<bool> CreateCollectionAsync(string database, string name)
        {
            return WcfServiceHelper.Execute(service => service.CreateCollectionAsync(database, name));
        }

        public static Task<bool> DropCollectionAsync(string database, string name)
        {
            return WcfServiceHelper.Execute(service => service.DropCollectionAsync(database, name));
        }

        public static Guid Insert<T>(string database, string collection, T obj)
        {
            return WcfServiceHelper.Execute(service => service.Insert(database, collection, Commons.CompressionHelper.Compress(JsonConvert.SerializeObject(obj))));
        }

        public static bool Update<T>(string database, string collection, Guid id, T obj)
        {
            return WcfServiceHelper.Execute(service => service.Update(database, collection, id, Commons.CompressionHelper.Compress(JsonConvert.SerializeObject(obj))));
        }

        public static bool Delete(string database, string collection, Guid id)
        {
            return WcfServiceHelper.Execute(service => service.Delete(database, collection, id));
        }

        public static Task<Guid> InsertAsync<T>(string database, string collection, T obj)
        {
            return WcfServiceHelper.Execute(service => service.InsertAsync(database, collection, Commons.CompressionHelper.Compress(JsonConvert.SerializeObject(obj))));
        }

        public static Task<bool> UpdateAsync<T>(string database, string collection, Guid id, T obj)
        {
            return WcfServiceHelper.Execute(service => service.UpdateAsync(database, collection, id, Commons.CompressionHelper.Compress(JsonConvert.SerializeObject(obj))));
        }

        public static Task<bool> DeleteAsync(string database, string collection, Guid id)
        {
            return WcfServiceHelper.Execute(service => service.DeleteAsync(database, collection, id));
        }

        public static bool Lock(string database, string collection)
        {
            return WcfServiceHelper.Execute(service => service.Lock(database, collection));
        }

        public static void Unlock(string database, string collection)
        {
            WcfServiceHelper.Execute(service => service.Unlock(database, collection));
        }

        public static Task<bool> LockAsync(string database, string collection)
        {
            return WcfServiceHelper.Execute(service => service.LockAsync(database, collection));
        }

        public static Task UnlockAsync(string database, string collection)
        {
            return WcfServiceHelper.Execute(service => service.UnlockAsync(database, collection));
        }

        public static T Get<T>(string database, string collection, Guid id)
        {
            return JsonConvert.DeserializeObject<T>(Commons.CompressionHelper.DecompressToString(WcfServiceHelper.Execute(service => service.Get(database, collection, id))));
        }

        public static async Task<T> GetAsync<T>(string database, string collection, Guid id)
        {
            return JsonConvert.DeserializeObject<T>(Commons.CompressionHelper.DecompressToString(await WcfServiceHelper.Execute(service => service.GetAsync(database, collection, id))));
        }

        public static T Query<T>(string script, string language, string database)
        {
            var data = WcfServiceHelper.Execute(service => service.Query(database, script, language));
            return data == null ? default(T) : JsonConvert.DeserializeObject<T>(Commons.CompressionHelper.DecompressToString(data));
        }

        public static async Task<T> QueryAsync<T>(string script, string language, string database)
        {
            var data = await WcfServiceHelper.Execute(service => service.QueryAsync(database, script, language));
            return data == null ? default(T) : JsonConvert.DeserializeObject<T>(Commons.CompressionHelper.DecompressToString(data));
        }

        public static bool CreateProcedure(string database, string name, string script, string language)
        {
            return WcfServiceHelper.Execute(service => service.CreateProcedure(database, name, script, language));
        }

        public static Task<bool> CreateProcedureAsync(string database, string name, string script, string language)
        {
            return WcfServiceHelper.Execute(service => service.CreateProcedureAsync(database, name, script, language));
        }

        public static bool DropProcedure(string database, string name)
        {
            return WcfServiceHelper.Execute(service => service.DropProcedure(database, name));
        }

        public static Task<bool> DropProcedureAsync(string database, string name)
        {
            return WcfServiceHelper.Execute(service => service.DropProcedureAsync(database, name));
        }

        public static int ExecuteProcedure(string database, string name, out object result, params object[] parameters)
        {
            var response = WcfServiceHelper.Execute(service => service.ExecuteProcedure(new Service.ExecuteProcedureRequest() { database = database, name = name, parameters = parameters }));
            result = response.result;
            return response.ExecuteProcedureResult;
        }

        public static async Task<ExecuteProcedureResponse> ExecuteProcedureAsync(string database, string name, params object[] parameters)
        {
            var response = await WcfServiceHelper.Execute(service => service.ExecuteProcedureAsync(new Service.ExecuteProcedureRequest() { database = database, name = name, parameters = parameters }));
            return new ExecuteProcedureResponse() { Result = response.result, ReturnValue = response.ExecuteProcedureResult };
        }
    }

    public sealed class ExecuteProcedureResponse
    {
        public object Result { get; set; }

        public int ReturnValue { get; set; }
    }
}

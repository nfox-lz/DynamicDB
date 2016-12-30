using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Compete.DynamicDB
{
    public partial class NetDatabase : IDisposable
    {
        private Service.DynamicDBServiceClient service = new Service.DynamicDBServiceClient();

        public string Name { get; set; }

        public void Open()
        {
            service.Open();
        }

        public void Close()
        {
            service.Close();
        }

        public bool CreateCollection(string name)
        {
            return service.CreateCollection(Name, name);
        }

        public bool DropCollection(string name)
        {
            return service.DropCollection(Name, name);
        }

        public Task<bool> CreateCollectionAsync(string name)
        {
            return service.CreateCollectionAsync(Name, name);
        }

        public Task<bool> DropCollectionAsync(string name)
        {
            return service.DropCollectionAsync(Name, name);
        }

        #region IDisposable Support

        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    service.Close();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~NetDatabase() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }

        #endregion // IDisposable Support

        public Guid Insert<T>(string collection, T obj)
        {
            return service.Insert(Name, collection, Commons.CompressionHelper.Compress(JsonConvert.SerializeObject(obj)));
        }

        public bool Update<T>(string collection, Guid id, T obj)
        {
            return service.Update(Name, collection, id, Commons.CompressionHelper.Compress(JsonConvert.SerializeObject(obj)));
        }

        public bool Delete(string collection, Guid id)
        {
            return service.Delete(Name, collection, id);
        }

        public Task<Guid> InsertAsync<T>(string collection, T obj)
        {
            return service.InsertAsync(Name, collection, Commons.CompressionHelper.Compress(JsonConvert.SerializeObject(obj)));
        }

        public Task<bool> UpdateAsync<T>(string collection, Guid id, T obj)
        {
            return service.UpdateAsync(Name, collection, id, Commons.CompressionHelper.Compress(JsonConvert.SerializeObject(obj)));
        }

        public Task<bool> DeleteAsync(string collection, Guid id)
        {
            return service.DeleteAsync(Name, collection, id);
        }

        public bool Lock(string collection)
        {
            return service.Lock(Name, collection);
        }

        public void Unlock(string collection)
        {
            service.Unlock(Name, collection);
        }

        public Task<bool> LockAsync(string collection)
        {
            return service.LockAsync(Name, collection);
        }

        public Task UnlockAsync(string collection)
        {
            return service.UnlockAsync(Name, collection);
        }

        public T Get<T>(string collection, Guid id)
        {
            return JsonConvert.DeserializeObject<T>(Commons.CompressionHelper.DecompressToString(service.Get(Name, collection, id)));
        }

        public async Task<T> GetAsync<T>(string collection, Guid id)
        {
            return JsonConvert.DeserializeObject<T>(Commons.CompressionHelper.DecompressToString(await service.GetAsync(Name, collection, id)));
        }

        public T Query<T>(string script, string language = "C#")
        {
            var data = service.Query(Name, script, language);
            return data == null ? default(T) : JsonConvert.DeserializeObject<T>(Commons.CompressionHelper.DecompressToString(data));
        }

        public async Task<T> QueryAsync<T>(string script, string language = "C#")
        {
            var data = await service.QueryAsync(Name, script, language);
            return data == null ? default(T) : JsonConvert.DeserializeObject<T>(Commons.CompressionHelper.DecompressToString(data));
        }

        public bool CreateProcedure(string name, string script, string language = "C#")
        {
            return service.CreateProcedure(Name, name, script, language);
        }

        public Task<bool> CreateProcedureAsync(string name, string script, string language = "C#")
        {
            return service.CreateProcedureAsync(Name, name, script, language);
        }

        public bool DropProcedure(string name)
        {
            return service.DropProcedure(Name, name);
        }

        public Task<bool> DropProcedureAsync(string name)
        {
            return service.DropProcedureAsync(Name, name);
        }

        public int ExecuteProcedure(string name, out object result, params object[] parameters)
        {
            return service.ExecuteProcedure(Name, name, parameters, out result);
        }

        public async Task<ExecuteProcedureResponse> ExecuteProcedureAsync(string name, params object[] parameters)
        {
            var response = await service.ExecuteProcedureAsync(new Service.ExecuteProcedureRequest() { database = Name, name = name, parameters = parameters });
            return new ExecuteProcedureResponse() { Result = response.result, ReturnValue = response.ExecuteProcedureResult };
        }
    }
}

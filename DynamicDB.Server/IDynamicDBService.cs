using System;
using System.ServiceModel;

namespace Compete.DynamicDB
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“INetDBService”。
    [ServiceContract]
    public interface IDynamicDBService
    {
        [OperationContract]
        Guid Insert(string database, string collection, byte[] data);

        [OperationContract]
        bool Update(string database, string collection, Guid id, byte[] data);

        [OperationContract]
        bool Delete(string database, string collection, Guid id);

        [OperationContract]
        bool Lock(string database, string collection);

        [OperationContract]
        void Unlock(string database, string collection);

        [OperationContract]
        byte[] Get(string database, string collection, Guid id);

        [OperationContract]
        byte[] LoadAll(string database, string collection);

        [OperationContract]
        byte[] Query(string database, string script, string language = "C#");

        [OperationContract]
        int ExecuteNonQuery(string script, string database = null);

        [OperationContract]
        byte[] Execute(string script, string database = null);

        [OperationContract]
        bool CreateProcedure(string database, string name, string script, string language = "C#");

        [OperationContract]
        bool DropProcedure(string database, string name);

        [OperationContract]
        int ExecuteProcedure(string database, string name, out object result, params object[] parameters);

        [OperationContract]
        bool CreateDatabase(string name);

        [OperationContract]
        bool DropDatabase(string name);

        [OperationContract]
        bool Save();

        [OperationContract]
        bool CreateCollection(string database, string name);

        [OperationContract]
        bool DropCollection(string database, string name);
    }
}

using System.Threading.Tasks;

namespace Compete.DynamicDB
{
    public sealed class NetDBInstance
    {
        public bool CreateDatabase(string name)
        {
            return WcfServiceHelper.Execute(service => service.CreateDatabase(name));
        }

        public bool DropDatabase(string name)
        {
            return WcfServiceHelper.Execute(service => service.DropDatabase(name));
        }

        public Task<bool> CreateDatabaseAsync(string name)
        {
            return WcfServiceHelper.Execute(service => service.CreateDatabaseAsync(name));
        }

        public Task<bool> DropDatabaseAsync(string name)
        {
            return WcfServiceHelper.Execute(service => service.DropDatabaseAsync(name));
        }

        public bool Save()
        {
            return WcfServiceHelper.Execute(service => service.Save());
        }
    }
}

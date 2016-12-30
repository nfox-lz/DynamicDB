using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Compete.DynamicDB
{
    internal static class WcfServiceHelper
    {
        internal delegate T ServiceAction<out T>(Service.IDynamicDBService service);

        public static void Execute(Action<Service.IDynamicDBService> action)
        {
            using (var service = new Service.DynamicDBServiceClient())
                try
                {
                    service.Open();
                    //service.ClientCredentials.UserName.UserName = "SuperMan";
                    //service.ClientCredentials.UserName.Password = "PASSWORD";

                    action(service);
                }
                catch (Exception exception)
                {
                    //Logging.LogHelper.Logger.LogException(exception);
                    return;
                }
                finally
                {
                    if (service.State == CommunicationState.Opened)
                        service.Close();
                }
        }

        public static T Execute<T>(ServiceAction<T> action)
        {
            using (var service = new Service.DynamicDBServiceClient())
                try
                {
                    service.Open();
                    //service.ClientCredentials.UserName.UserName = "SuperMan";
                    //service.ClientCredentials.UserName.Password = "PASSWORD";

                    return action(service);
                }
                catch (Exception exception)
                {
                    //Logging.LogHelper.Logger.LogException(exception);
                    return default(T);
                }
                finally
                {
                    if (service.State == CommunicationState.Opened)
                        service.Close();
                }
        }

        public static async Task Execute(ServiceAction<Task> action)
        {
            using (var service = new Service.DynamicDBServiceClient())
                try
                {
                    service.Open();
                    //service.ClientCredentials.UserName.UserName = "SuperMan";
                    //service.ClientCredentials.UserName.Password = "PASSWORD";

                    await action(service);
                }
                catch (Exception exception)
                {
                    //Logging.LogHelper.Logger.LogException(exception);
                }
                finally
                {
                    if (service.State == CommunicationState.Opened)
                        service.Close();
                }
        }

        public static async Task<T> Execute<T>(ServiceAction<Task<T>> action)
        {
            using (var service = new Service.DynamicDBServiceClient())
                try
                {
                    service.Open();
                    //service.ClientCredentials.UserName.UserName = "SuperMan";
                    //service.ClientCredentials.UserName.Password = "PASSWORD";

                    return await action(service);
                }
                catch (Exception exception)
                {
                    //Logging.LogHelper.Logger.LogException(exception);
                    return default(T);
                }
                finally
                {
                    if (service.State == CommunicationState.Opened)
                        service.Close();
                }
        }
    }
}

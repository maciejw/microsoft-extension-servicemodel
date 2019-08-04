using System.Collections.Generic;
using System.Linq;

namespace System.ServiceModel.Description
{
    public static class ServiceDescriptionExtensions
    {

        public static IEnumerable<string> GetContractNames(this ServiceDescription serviceDescription, Type serviceContract)
        {
            return from serviceEndpoint in serviceDescription.Endpoints
                    where serviceEndpoint.Contract.ContractType.IsAssignableFrom(serviceContract)
                    select serviceEndpoint.Contract.Name;
        }
        /// <summary>
        /// Checks <see cref="InstanceContextMode.Single" /> mode of <see cref="ServiceDescription.Behaviors"/> attribute value <see cref="ServiceBehaviorAttribute.InstanceContextMode"/>.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static bool IsInstanceContextModeSingle(this ServiceDescription @this)
        {
            return @this.Behaviors.Find<ServiceBehaviorAttribute>()?.InstanceContextMode == InstanceContextMode.Single;
        }
    }
}

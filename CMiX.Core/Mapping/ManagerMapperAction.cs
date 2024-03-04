// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.BaseControls;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Prefabs.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Core.Mapping
{
    public class ManagerMapperAction : IMappingAction<IControlModel, IControl>
    {
        public ManagerMapperAction(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        private readonly IServiceProvider ServiceProvider;

        public void Process(IControlModel source, IControl destination, ResolutionContext context)
        {

            if (destination is PrefabManager prefabManager)
            {
                prefabManager.ControlFactory = ServiceProvider.GetRequiredService<ControlFactory>();
                prefabManager.ManagerMessenger = ServiceProvider.GetRequiredService<ManagerMessenger>();
            }
            if (destination.GetType() == typeof(GenericValue<float>))
            {
                var dest = (GenericValue<float>)destination;
                dest.ControlMessenger = ServiceProvider.GetRequiredService<ControlMessenger>();
            }
        }
    }
}

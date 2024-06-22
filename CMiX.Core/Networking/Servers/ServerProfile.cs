// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messenger;
using CMiX.Core.Networking.Servers;

namespace CMiX.Core.Mapping
{
    public class ServerProfile : Profile
    {
        public ServerProfile()
        {
            CreateMap<Server, ServerModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<ServerManager, ServerManagerModel>().ReverseMap().ConstructUsingServiceLocator();

            CreateMap<IControl, IControlModel>()
                .Include<Server, ServerModel>()
                .Include<ServerManager, ServerManagerModel>()
                .ReverseMap()
                .ConstructUsingServiceLocator();
        }
    }
}

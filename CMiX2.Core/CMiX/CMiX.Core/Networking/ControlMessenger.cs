// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Mapper;
using CMiX.Core.Models;
using CMiX.Core.Networking.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Network
{
    public static class ControlMessenger
    {
        private static IMapper Mapper;
        public static bool CanSend = true;

        static ControlMessenger()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile(new MappingProfile());
            });

            Mapper = config.CreateMapper();
        }

        public static void Receive(IIDObject iDObject, MessageRequestControl message)
        {
            CanSend = false;

            if (message.ID == iDObject.ID && !message.HasReceivedResponse)
                message.Reply(iDObject);

            CanSend = true;
        }

        public static void Send<T>(IControl control) where T : IModel
        {
            if (CanSend)
            {
                var model = Mapper.Map<T>(control);
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(model), MessageType.Out);
            }     
        }
    }
}

// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Diagnostics;
using System.Windows.Controls;
using AutoMapper;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Networking.Servers;
using CMiX.Core.ViewModels;

namespace CMiX.Core.Windows
{
    public class MainMenuMessenger
    {
        public MainMenuMessenger(IMapper mapper, ServerRepository serverRepository)
        {
            Mapper = mapper;
            ServerRepository = serverRepository;
        }

        public IMapper Mapper;
        public bool CanSend = true;
        ServerRepository ServerRepository { get; set; }

        public void SendOpenProject(string filePath)
        {
            if (CanSend)
            {
                var message = new MessageOpenProject(filePath);

                ServerRepository.GetServers().ForEach(x => x.SendMessage(message));

                Debug.WriteLine("Message Sent with Value : " + filePath);
            }
        }

        internal void Receive(MainMenu mainMenu, IMessage message)
        {
            if (message is MessageOpenProject messageOpenProject)
                mainMenu.OpenProject(messageOpenProject.FilePath);
        }
    }
}

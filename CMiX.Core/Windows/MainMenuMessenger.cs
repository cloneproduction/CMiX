// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Diagnostics;
using AutoMapper;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Networking.Messenger;
using CMiX.Core.Prefabs;
using CMiX.Core.ViewModels;

namespace CMiX.Core.Windows
{
    public class MainMenuMessenger
    {
        public MainMenuMessenger(IMapper mapper, ControlRepository controlRepository)
        {
            Mapper = mapper;
            ControlRepository = controlRepository;
        }

        public IMapper Mapper;
        public bool CanSend = true;
        ControlRepository ControlRepository { get; set; }

        public void SendOpenProject(string filePath)
        {
            if (CanSend)
            {
                var message = new MessageOpenProject(filePath);
                foreach (Server server in ControlRepository.Servers)
                    server.SendMessage(message);

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

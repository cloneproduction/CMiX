// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Compositing;
using CMiX.Core.Network;
using CMiX.Core.Networking.Messages;
using CMiX.Core.ViewModels.Assets;
using CMiX.Core.ViewModels.Windows;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.ViewModels
{
    public class MainViewModel
    {
        public MainViewModel(Project project, IMessageService messageService)
        {
            Project = project;
            ServerManager = new ServerManager(messageService);
            MainWindowController = new MainWindowController();
            AssetManager = new AssetManager(project);
            MainMenu = new MainMenu(project);

            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.Internal);
            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.In);
        }

        public Project Project { get; set; }
        public ServerManager ServerManager { get; set; }
        public AssetManager AssetManager { get; set; }
        public MainMenu MainMenu { get; set; }
        public MainWindowController MainWindowController { get; set; }
    }
}

// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using AutoMapper;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentations.Components;
using CMiX.Core.Presentations.Transform;
using CMiX.Core.Presentations.ViewModels.Assets;
using CMiX.Core.Presentations.Beat;
using CMiX.Core.Presentations.Network;
using CMiX.Core.Presentations.ViewModels.Windows;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentations.ViewModels
{
    public class MainViewModel
    {
        public MainViewModel(IProject project, IMessageService messageService)
        {
            Project = project;
            ServerManager = new ServerManager(messageService);

            MainWindowController = new MainWindowController();

            AssetManager = new AssetManager();

            BeatManager = new BeatManager(project);
            MainMenu = new MainMenu(project);

            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.Internal);
            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.In);


            //Mapper = mapper;

            Transform2D trans = new Transform2D(new Transform2DModel(), null);
        }

        //public IMapper Mapper { get; set; }
        public ICommand OpenProjectCommand { get; set; }


        public IProject Project { get; set; }
        public BeatManager BeatManager { get; set; }
        public ServerManager ServerManager { get; set; }
        public AssetManager AssetManager { get; set; }
        public MainMenu MainMenu { get; set; }
        public MainWindowController MainWindowController { get; set; }
    }
}

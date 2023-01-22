// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Models.BaseControls;
using CMiX.Core.Models;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentation.ViewModels.Assets;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Presentation.ViewModels.Scheduling;
using CMiX.Core.Presentation.ViewModels.Windows;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows.Input;
using CMiX.Core.Presentation.ViewModels.BaseControl;
using CMiX.Core.Presentation.ViewModels.Components;

namespace CMiX.Core.Presentation.ViewModels
{ 
    public class MainViewModel
    {
        public MainViewModel(IProject project, IMessageService messageService, IMapper mapper)
        {
            Project = project;
            ServerManager = new ServerManager(messageService);

            MainWindowController = new MainWindowController();

            AssetManager = new AssetManager();

            BeatManager = new BeatManager(project);
            //SchedulerManager = new SchedulerManager(project, dialogService);
            //PlaylistEditor = new PlaylistEditor(project);
            MainMenu = new MainMenu(project);

            //OpenProjectCommand = new RelayCommand(FocusMeshEntity);

            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.Internal);
            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.In);


            Mapper = mapper;

            Transform2D trans = new Transform2D(new Transform2DModel(), null);

            var prout = Mapper.Map<Transform2DModel>(trans);
            Vector2 vector2 = new Vector2(new Vector2Model(11.0f, 123.0f));
            var pouet = Mapper.Map<Vector2Model>(vector2);
            Console.WriteLine(prout);
        }

        public IMapper Mapper { get; set; }
        public ICommand OpenProjectCommand { get; set; }


        public IProject Project { get; set; }
        public BeatManager BeatManager { get; set; }
        public ServerManager ServerManager { get; set; }
        //public PlaylistEditor PlaylistEditor { get; set; }
        public AssetManager AssetManager { get; set; }
        public MainMenu MainMenu { get; set; }
        //public SchedulerManager SchedulerManager { get; set; }
        public MainWindowController MainWindowController { get; set; }
    }
}

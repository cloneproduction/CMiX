// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Assets;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Presentation.ViewModels.Scheduling;
using CMiX.Core.Presentation.ViewModels.Windows;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MvvmDialogs;
using System.Windows.Input;

namespace CMiX.Core.Presentation.ViewModels
{ 
    public class MainViewModel
    {
        public MainViewModel(IProject project, IDialogService dialogService, IMessageService messageService)
        {
            Project = project;
            DialogService = dialogService;
            ServerManager = new ServerManager(messageService, dialogService);

            MainWindowController = new MainWindowController(dialogService);

            AssetManager = new AssetManager(dialogService);

            SchedulerManager = new SchedulerManager(project, dialogService);
            PlaylistEditor = new PlaylistEditor(project);
            MainMenu = new MainMenu(project, dialogService);

            KeyPressedCommand = new RelayCommand<Key>(KeyPressed);
            //OpenProjectCommand = new RelayCommand(FocusMeshEntity);

            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.Internal);
            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.In);
        }

        public Key MyKey
        {
            get => Key.D1;
        }

        public ICommand KeyPressedCommand { get; set; }
        public ICommand OpenProjectCommand { get; set; }

        public IDialogService DialogService { get; set; }
        public IProject Project { get; set; }
        //public BeatManager BeatManager { get; set; }
        public ServerManager ServerManager { get; set; }
        public PlaylistEditor PlaylistEditor { get; set; }
        public AssetManager AssetManager { get; set; }
        public MainMenu MainMenu { get; set; }
        public SchedulerManager SchedulerManager { get; set; }
        public MainWindowController MainWindowController { get; set; }

        public bool TextInputFocusState { get; private set; }



        public void KeyPressed(Key key)
        {
            if(!TextInputFocusState)
                WeakReferenceMessenger.Default.Send(new MessageKeyPressed(key), MessageType.In);
        }
    }
}

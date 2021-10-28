// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentation.ViewModels.Assets;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Components;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Presentation.ViewModels.Scheduling;
using CMiX.Core.Presentation.ViewModels.Windows;
using CMiX.Core.Network.Messages;
using CommunityToolkit.Mvvm.Messaging;
using MvvmDialogs;

namespace CMiX.Core.Presentation.ViewModels
{
    public class MainViewModel : IRecipient<MessageRequestDialogService>
    {
        public MainViewModel(IProject project, IDialogService dialogService, IMessageService messageService)
        {
            Project = project;
            DialogService = dialogService;
            ServerManager = new ServerManager(messageService, dialogService);

            MainWindowController = new MainWindowController(dialogService);

            AssetManager = new AssetManager(dialogService);

            BeatManager = new BeatManager(project);
            ComponentManager = new ComponentManager(project);
            SchedulerManager = new SchedulerManager(project, dialogService);
            PlaylistEditor = new PlaylistEditor(project);
            MainMenu = new MainMenu(project, dialogService);

            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.Internal);
        }

        public IDialogService DialogService { get; set; }
        public IProject Project { get; set; }
        public BeatManager BeatManager { get; set; }
        public ServerManager ServerManager { get; set; }
        public PlaylistEditor PlaylistEditor { get; set; }
        public AssetManager AssetManager { get; set; }
        public ComponentManager ComponentManager { get; set; }
        public MainMenu MainMenu { get; set; }
        public SchedulerManager SchedulerManager { get; set; }
        public MainWindowController MainWindowController { get; set; }

        public void Receive(MessageRequestDialogService message)
        {
            message.Reply(DialogService);
        }
    }
}

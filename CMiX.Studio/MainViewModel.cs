// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentation.ViewModels.Assets;
using CMiX.Core.Presentation.ViewModels.Components;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Presentation.ViewModels.Scheduling;
using CMiX.Core.Presentation.ViewModels.Windows;
using MvvmDialogs;

namespace CMiX.Core.Presentation.ViewModels
{
    public class MainViewModel
    {
        public MainViewModel(IProject project, IDialogService dialogService, IMessageService messageService)
        {
            ServerManager = new ServerManager(messageService, dialogService);

            MainWindowController = new MainWindowController(dialogService);
            MainMenu = new MainMenu(project, dialogService);
            AssetManager = new AssetManager(project, dialogService);

            ComponentManager = new ComponentManager(project);
            SchedulerManager = new SchedulerManager(project);
            Outliner = new Outliner(project);
            PlaylistEditor = new PlaylistEditor(project);
        }


        public Outliner Outliner { get; set; }
        public ServerManager ServerManager { get; set; }
        public PlaylistEditor PlaylistEditor { get; set; }
        public AssetManager AssetManager { get; set; }
        public ComponentManager ComponentManager { get; set; }
        public MainMenu MainMenu { get; set; }
        public SchedulerManager SchedulerManager { get; set; }
        public MainWindowController MainWindowController { get; set; }
    }
}

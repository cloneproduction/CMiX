// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentation.ViewModels.Assets;
using CMiX.Core.Presentation.ViewModels.Beat;
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
            Project = project;
            ServerManager = new ServerManager(messageService, dialogService);

            MainWindowController = new MainWindowController(dialogService);

            AssetManager = new AssetManager(dialogService);

            BeatManager = new BeatManager(project);
            ComponentManager = new ComponentManager(project);
            SchedulerManager = new SchedulerManager(project, dialogService);

            MainMenu = new MainMenu(project, dialogService);
            Outliner = new Outliner(project);
        }

        public IProject Project { get; set; }
        public Outliner Outliner { get; set; }
        public BeatManager BeatManager { get; set; }
        public ServerManager ServerManager { get; set; }
        public PlaylistEditor PlaylistEditor { get; set; }
        public AssetManager AssetManager { get; set; }
        public ComponentManager ComponentManager { get; set; }
        public MainMenu MainMenu { get; set; }
        public SchedulerManager SchedulerManager { get; set; }
        public MainWindowController MainWindowController { get; set; }
    }
}

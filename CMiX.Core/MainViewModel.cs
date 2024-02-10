// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Compositing;
using CMiX.Core.Network;
using CMiX.Core.Prefabs;
using CMiX.Core.Services;
using CMiX.Core.ViewModels.Assets;
using CMiX.Core.ViewModels.Windows;

namespace CMiX.Core.ViewModels
{
    public class MainViewModel
    {
        public MainViewModel(Project project, ControlRepository controlRepository, ServerManager serverManager, AssetManager assetManager, MainWindowController mainWindowController, MainMenu mainMenu)
        {
            Project = project;
            ServerManager = serverManager;
            MainWindowController = mainWindowController;
            AssetManager = assetManager;
            MainMenu = mainMenu;
            PrefabRepositories = controlRepository;
        }

        public ControlRepository PrefabRepositories { get; set; }
        public Project Project { get; set; }
        public ServerManager ServerManager { get; set; }
        public AssetManager AssetManager { get; set; }
        public MainMenu MainMenu { get; set; }
        public MainWindowController MainWindowController { get; set; }
    }
}

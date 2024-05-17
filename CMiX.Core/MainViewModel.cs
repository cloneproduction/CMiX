// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Compositing;
using CMiX.Core.Network;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.ViewModels.Assets;
using CMiX.Core.ViewModels.Windows;

namespace CMiX.Core.ViewModels
{
    public class MainViewModel
    {
        public MainViewModel(Project project, 
                             PrefabManager textureManager,
                             PrefabManager materialManager,
                             ControlRepository controlRepository, 
                             ServerManager serverManager, 
                             AssetManager assetManager, 
                             MainWindowController mainWindowController, 
                             MainMenu mainMenu)
        {
            MaterialManager = materialManager;
            materialManager.ManagerData.ID = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF02");
            TextureManager = textureManager;
            textureManager.ManagerData.ID = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF01");

            Project = project;
            ServerManager = serverManager;
            MainWindowController = mainWindowController;
            AssetManager = assetManager;
            MainMenu = mainMenu;
            PrefabRepositories = controlRepository;
        }

        public PrefabManager TextureManager { get; set; }
        public PrefabManager MaterialManager { get; set; }
        public ControlRepository PrefabRepositories { get; set; }
        public Project Project { get; set; }
        public ServerManager ServerManager { get; set; }
        public AssetManager AssetManager { get; set; }
        public MainMenu MainMenu { get; set; }
        public MainWindowController MainWindowController { get; set; }
    }
}

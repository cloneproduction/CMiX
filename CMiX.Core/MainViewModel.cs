// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Ceras;
using CMiX.Core.Compositing;
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
                             PrefabManager entityManager,
                             PrefabManager cameraManager,
                             PrefabManager lightManager,
                             PrefabManager serverManager,
                             ControlRepository controlRepository, 
                             AssetManager assetManager, 
                             MainWindowController mainWindowController, 
                             MainMenu mainMenu)
        {
            LightManager = lightManager;
            lightManager.ManagerData.ID = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF05");
            CameraManager = cameraManager;
            cameraManager.ManagerData.ID = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF04");
            EntityManager = entityManager;
            entityManager.ManagerData.ID = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF03");
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

        public PrefabManager ServerManager { get; set; }
        public PrefabManager LightManager { get; set; }
        public PrefabManager EntityManager { get; set; }
        public PrefabManager TextureManager { get; set; }
        public PrefabManager MaterialManager { get; set; }
        public PrefabManager CameraManager { get; set; }
        public ControlRepository PrefabRepositories { get; set; }
        public Project Project { get; set; }

        public AssetManager AssetManager { get; set; }
        public MainMenu MainMenu { get; set; }
        public MainWindowController MainWindowController { get; set; }
    }
}

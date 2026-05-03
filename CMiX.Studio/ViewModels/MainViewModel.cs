// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using CMiX.Core;
using CMiX.Core.Assets;
using CMiX.Core.Compositing;
using CMiX.Core.Networking.Servers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;

namespace CMiX.Studio.ViewModels
{
    public class MainViewModel : IControl
    {
        public MainViewModel(Project project,
                             PrefabManager textureManager,
                             PrefabManager materialManager,
                             PrefabManager entityManager,
                             PrefabManager cameraManager,
                             PrefabManager lightManager,
                             PrefabManager beatManager,
                             PrefabManager colorPaletteManager,
                             ServerManager serverManager,
                             ControlRepository controlRepository,
                             AssetManager assetManager,
                             MainWindowController mainWindowController,
                             MainMenu mainMenu,
                             ControlActivationService activationService,
                             UndoManager undoManager)
        {
            _undoManager = undoManager;
            TextureManager = SetupManager(textureManager, ManagerIDs.TextureManager);
            MaterialManager = SetupManager(materialManager, ManagerIDs.MaterialManager);
            EntityManager = SetupManager(entityManager, ManagerIDs.EntityManager);
            CameraManager = SetupManager(cameraManager, ManagerIDs.CameraManager);
            LightManager = SetupManager(lightManager, ManagerIDs.LightManager);
            BeatManager = SetupManager(beatManager, ManagerIDs.BeatManager);
            ColorPaletteManager = SetupManager(colorPaletteManager, ManagerIDs.ColorPaletteManager);

            Project = project;
            ServerManager = serverManager;
            MainWindowController = mainWindowController;
            AssetManager = assetManager;
            MainMenu = mainMenu;
            PrefabRepositories = controlRepository;
            // Clear any stale registrations from top-level managers activated via SetupManager.
            // These were activated manually and never went through ActivateAll(),
            // so they may still be in the list. Without this, ControlFactory.Create
            // could re-activate them unexpectedly during nested control creation.
            activationService.Clear();
        }

        private readonly UndoManager _undoManager;

        private PrefabManager SetupManager(PrefabManager manager, Guid id)
        {
            manager.ManagerData.ID = id;
            manager.UndoManager = _undoManager;
            manager.Activate();
            return manager;
        }

        public IControlModel ToModel()
        {
            throw new NotImplementedException();
        }

        public void FromModel(IControlModel model)
        {
            throw new NotImplementedException();
        }

        public ServerManager ServerManager { get; set; }
        public PrefabManager ColorPaletteManager { get; set; }
        public PrefabManager BeatManager { get; set; }
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
        public Guid ID { get; set; }
    }
}

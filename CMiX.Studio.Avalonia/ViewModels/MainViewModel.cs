// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using CMiX.Core;
using CMiX.Core.Assets;
using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Studio.Avalonia.ViewModels
{
    // ObservableObject satisfies the INotifyPropertyChanged requirement of the
    // dialog service owner lookup, which casts the main window DataContext.
    public class MainViewModel : ObservableObject, IControl
    {
        public MainViewModel(Project project,
                             PrefabManager textureManager,
                             PrefabManager entityManager,
                             PrefabManager cameraManager,
                             PrefabManager lightManager,
                             PrefabManager beatManager,
                             PrefabManager colorPaletteManager,
                             SyncPeer sync,
                             ControlRepository controlRepository,
                             AssetManager assetManager,
                             MainWindowController mainWindowController,
                             MainMenu mainMenu,
                             ControlActivationService activationService,
                             UndoManager undoManager)
        {
            _undoManager = undoManager;
            TextureManager = SetupManager(textureManager, ManagerIDs.TextureManager);
            EntityManager = SetupManager(entityManager, ManagerIDs.EntityManager);
            CameraManager = SetupManager(cameraManager, ManagerIDs.CameraManager);
            LightManager = SetupManager(lightManager, ManagerIDs.LightManager);
            BeatManager = SetupManager(beatManager, ManagerIDs.BeatManager);
            ColorPaletteManager = SetupManager(colorPaletteManager, ManagerIDs.ColorPaletteManager);

            Project = project;
            Sync = sync;
            MainWindowController = mainWindowController;
            AssetManager = assetManager;
            MainMenu = mainMenu;
            _repositoryManagers = new[]
            {
                TextureManager, EntityManager, CameraManager,
                LightManager, BeatManager, ColorPaletteManager
            };
            MainMenu.RepositoryManagers = _repositoryManagers;
            Sync.SnapshotApplied += OnSnapshotApplied;
            ControlRepository = controlRepository;
            // Clear any stale registrations from top level managers activated via SetupManager.
            // These were activated manually and never went through ActivateAll(),
            // so they may still be in the list. Without this, ControlFactory.Create
            // could re activate them unexpectedly during nested control creation.
            activationService.Clear();
        }

        private readonly UndoManager _undoManager;
        private readonly PrefabManager[] _repositoryManagers;

        // A snapshot carries the project only. The six repository managers keep their items, so
        // a Pull or a gap re-join empties them the way File > New and File > Open do. The undo
        // manager stays untouched here, because ProjectSyncTarget already clears it.
        public void ClearRepositoryManagers()
        {
            foreach (var manager in _repositoryManagers)
                manager.ClearAll();
        }

        private void OnSnapshotApplied(object? sender, EventArgs e) => ClearRepositoryManagers();

        private PrefabManager SetupManager(PrefabManager manager, Guid id)
        {
            manager.ManagerData.ID = id;
            // Re registers under the final ID; the ctor registration used the id ManagerData had
            // before this assignment and would otherwise be looked up under the wrong key.
            manager.RegisterDeleter();
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

        public SyncPeer Sync { get; set; }
        public PrefabManager ColorPaletteManager { get; set; }
        public PrefabManager BeatManager { get; set; }
        public PrefabManager LightManager { get; set; }
        public PrefabManager EntityManager { get; set; }
        public PrefabManager TextureManager { get; set; }
        public PrefabManager CameraManager { get; set; }
        // Named after its type so the window relative binding paths in the views read the
        // same as the manager relative ControlRepository paths the tabs use.
        public ControlRepository ControlRepository { get; set; }
        public Project Project { get; set; }
        public AssetManager AssetManager { get; set; }
        public MainMenu MainMenu { get; set; }
        public MainWindowController MainWindowController { get; set; }
        public Guid ID { get; set; }
    }
}

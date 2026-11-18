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
using CMiX.Studio.Avalonia.Services;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
                             UndoManager undoManager,
                             ShortcutMap shortcuts,
                             InterfaceOptions interfaceOptions,
                             PathDropService pathDropService,
                             ProjectTree projectTree,
                             PanelFocusCycle panelFocus)
        {
            PanelFocus = panelFocus;
            _undoManager = undoManager;
            TextureManager = SetupManager(textureManager, ManagerIDs.TextureManager);
            EntityManager = SetupManager(entityManager, ManagerIDs.EntityManager);
            CameraManager = SetupManager(cameraManager, ManagerIDs.CameraManager);
            LightManager = SetupManager(lightManager, ManagerIDs.LightManager);
            BeatManager = SetupManager(beatManager, ManagerIDs.BeatManager);
            ColorPaletteManager = SetupManager(colorPaletteManager, ManagerIDs.ColorPaletteManager);

            Project = project;
            Shortcuts = shortcuts;
            Interface = interfaceOptions;
            ShortcutSettings = new ShortcutSettings(shortcuts);
            UndoCommand = new RelayCommand(undoManager.Undo);
            RedoCommand = new RelayCommand(undoManager.Redo);
            PathDropService = pathDropService;
            ProjectTree = projectTree;
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
            // SetupManager activates the top level managers by hand, so ActivateAll never runs for them.
            // Remove their registrations, so ControlFactory.Create does not activate them again in a nested control.
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
        public ProjectTree ProjectTree { get; }

        // The rules of dropped files. The drop behaviors of the views take it from here.
        public PathDropService PathDropService { get; }

        // The keyboard shortcuts. The key bindings of the window and of the tree read their gestures here.
        public ShortcutMap Shortcuts { get; }

        // The panel keys of the window (Window.*, Tree.Focus) run its commands.
        public PanelFocusCycle PanelFocus { get; }

        // The settings tab edits the shortcuts with this.
        public ShortcutSettings ShortcutSettings { get; }

        // The settings tab edits the interface settings with this. The tree uses the same object.
        public InterfaceOptions Interface { get; }

        public IRelayCommand UndoCommand { get; }
        public IRelayCommand RedoCommand { get; }

        // What the left and the right panel of the window show. The user picks them with the icon button at the top of each panel.
        // The tree can be on the left, on the right or on both sides.
        public System.Collections.Generic.IReadOnlyList<PanelChoice> PanelChoices => PanelChoice.All;

        private PanelChoice _leftPanel = PanelChoice.Tree;
        public PanelChoice LeftPanel
        {
            get => _leftPanel;
            set => SetProperty(ref _leftPanel, value);
        }

        private PanelChoice _rightPanel = PanelChoice.Tabs;
        public PanelChoice RightPanel
        {
            get => _rightPanel;
            set => SetProperty(ref _rightPanel, value);
        }
        public AssetManager AssetManager { get; set; }
        public MainMenu MainMenu { get; set; }
        public MainWindowController MainWindowController { get; set; }
        public Guid ID { get; set; }
    }
}

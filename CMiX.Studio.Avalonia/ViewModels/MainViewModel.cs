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
using CMiX.Studio.Avalonia.Session;
using CMiX.Studio.Avalonia.Settings;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Studio.Avalonia.ViewModels
{
    public class MainViewModel : ObservableObject, IControl
    {
        public MainViewModel(Project project,
                             ProjectSession session,
                             SyncPeer sync,
                             ControlRepository controlRepository,
                             AssetManager assetManager,
                             MainWindowController mainWindowController,
                             MainMenuViewModel mainMenu,
                             ProjectFileViewModel projectFile,
                             SettingsSyncViewModel settingsSync,
                             ControlActivationService activationService,
                             UndoManager undoManager,
                             ShortcutMap shortcuts,
                             InterfaceState interfaceState,
                             PathDropRules pathDropRules,
                             ProjectTree projectTree,
                             Views.PanelFocusCycle panelFocus)
        {
            PanelFocus = panelFocus;
            _session = session;
            Project = project;
            Shortcuts = shortcuts;
            Interface = interfaceState;
            ShortcutSettings = new ShortcutSettings(shortcuts);
            UndoCommand = new RelayCommand(undoManager.Undo, () => undoManager.CanUndo);
            RedoCommand = new RelayCommand(undoManager.Redo, () => undoManager.CanRedo);
            undoManager.Changed += () =>
            {
                UndoCommand.NotifyCanExecuteChanged();
                RedoCommand.NotifyCanExecuteChanged();
            };
            BeginUndoCaptureCommand = new RelayCommand(undoManager.BeginCapture);
            EndUndoCaptureCommand = new RelayCommand(undoManager.EndCapture);
            PathDropRules = pathDropRules;
            ProjectTree = projectTree;
            Sync = sync;
            MainWindowController = mainWindowController;
            AssetManager = assetManager;
            MainMenu = mainMenu;
            ProjectFile = projectFile;
            SettingsSync = settingsSync;
            ControlRepository = controlRepository;
            // This clear runs last. The session activates the six lists by hand, so a nested control must not activate them again.
            activationService.Clear();
        }

        private readonly ProjectSession _session;

        public IControlModel ToModel()
        {
            throw new NotImplementedException();
        }

        public void FromModel(IControlModel model)
        {
            throw new NotImplementedException();
        }

        public SyncPeer Sync { get; set; }
        public PrefabManager ColorPaletteManager => _session.ColorPalettes;
        public PrefabManager BeatManager => _session.Beats;
        public PrefabManager LightManager => _session.Lights;
        public PrefabManager EntityManager => _session.Entities;
        public PrefabManager TextureManager => _session.Textures;
        public PrefabManager CameraManager => _session.Cameras;
        // The views bind ControlRepository from the window and from a manager with the same path.
        public ControlRepository ControlRepository { get; set; }
        public Project Project { get; set; }
        public ProjectTree ProjectTree { get; }

        // The rules of dropped files. The drop behaviors of the views take it from here.
        public PathDropRules PathDropRules { get; }

        // The keyboard shortcuts. The key bindings of the window and of the tree read their gestures here.
        public ShortcutMap Shortcuts { get; }

        // The panel keys of the window (Window.*, Tree.Focus) run its commands.
        public Views.PanelFocusCycle PanelFocus { get; }

        // The settings tab edits the shortcuts with this.
        public ShortcutSettings ShortcutSettings { get; }

        // The settings tab edits the interface settings with this. The tree uses the same object.
        public InterfaceState Interface { get; }

        public IRelayCommand UndoCommand { get; }
        public IRelayCommand RedoCommand { get; }

        public IRelayCommand BeginUndoCaptureCommand { get; }
        public IRelayCommand EndUndoCaptureCommand { get; }

        // What the left and the right panel of the window show. The tree can be on both sides.
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
        public MainMenuViewModel MainMenu { get; }
        public ProjectFileViewModel ProjectFile { get; }
        public SettingsSyncViewModel SettingsSync { get; }
        public MainWindowController MainWindowController { get; set; }
        public Guid ID { get; set; }
    }
}

// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using CMiX.Core;
using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using MvvmDialogs.FrameworkDialogs.SaveFile;

namespace CMiX.Studio.ViewModels
{
    public partial class MainMenu : ObservableRecipient, IControl, IRecipient<IMessage>
    {
        public MainMenu(Project project, ControlFactory controlFactory, UndoManager undoManager)
        {
            Project = project;
            IsActive = true;

            NewProjectCommand = new RelayCommand(NewProject);
            OpenProjectCommand = new RelayCommand(OpenProject);
            SaveProjectCommand = new RelayCommand(SaveProject);
            SaveAsProjectCommand = new RelayCommand(SaveAsProject);
            AddCompositionCommand = new RelayCommand(AddComposition);
            AddLayerCommand = new RelayCommand(AddLayer);
            UndoCommand = new RelayCommand(() => undoManager.Undo());
            RedoCommand = new RelayCommand(() => undoManager.Redo());
            CloseWindowCommand = new RelayCommand(() => Application.Current.MainWindow.Close());

            DialogService = new DialogService();
            ControlFactory = controlFactory;
        }

        [ObservableProperty]
        private string _folderPath;

        public Guid ID { get; set; } = Guid.NewGuid();
        public ControlFactory ControlFactory { get; set; }
        public DialogService DialogService { get; set; }
        public Project Project { get; set; }

        public ICommand NewProjectCommand { get; }
        public ICommand SaveProjectCommand { get; }
        public ICommand SaveAsProjectCommand { get; }
        public ICommand OpenProjectCommand { get; }
        public ICommand AddCompositionCommand { get; }
        public ICommand AddLayerCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand CloseWindowCommand { get; }

        public void AddLayer()
        {
            //WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageAddLayer(), MessageType.Internal);
        }

        public void AddComposition()
        {
            //WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageAddComposition(), MessageType.Internal);
        }

        private void NewProject()
        {
            Project.CompositionManager.ClearAll();
            FolderPath = null;
        }

        private void OpenProject()
        {
            var settings = new OpenFileDialogSettings
            {
                Filter = "CMiX Project (*.cmix)|*.cmix"
            };

            if (DialogService.ShowOpenFileDialog(this, settings) != true) return;
            if (string.IsNullOrWhiteSpace(settings.FileName)) return;

            var projectModel = ProjectSerializer.Load(settings.FileName);
            if (projectModel.CompositionManager.ManagerData.Items.FirstOrDefault() is not CompositionModel compositionModel) return;

            var json = JsonSerializer.Serialize(compositionModel, ProjectSerializer.Options);
            json = ReplaceAllGuids(json);
            var cloned = JsonSerializer.Deserialize<CompositionModel>(json, ProjectSerializer.Options);

            Project.CompositionManager.AddItem(cloned);
        }

        private static string ReplaceAllGuids(string json)
        {
            var guidPattern = @"[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}";
            var guidMap = new Dictionary<string, string>();
            return Regex.Replace(json, guidPattern, match =>
            {
                var original = match.Value;
                if (!guidMap.TryGetValue(original, out var newGuid))
                {
                    newGuid = Guid.NewGuid().ToString();
                    guidMap[original] = newGuid;
                }
                return newGuid;
            });
        }

        private void SaveProject()
        {
            if (!string.IsNullOrWhiteSpace(FolderPath))
            {
                WriteProject(FolderPath);
                return;
            }
            SaveAsProject();
        }

        private void SaveAsProject()
        {
            var settings = new SaveFileDialogSettings
            {
                Filter = "CMiX Project (*.cmix)|*.cmix",
                DefaultExt = "cmix",
                AddExtension = true
            };

            if (DialogService.ShowSaveFileDialog(this, settings) != true) return;
            if (string.IsNullOrWhiteSpace(settings.FileName)) return;

            FolderPath = settings.FileName;
            WriteProject(FolderPath);
        }

        private void WriteProject(string path)
        {
            var selectedComposition = Project.CompositionManager.SelectedItem as Composition;
            if (selectedComposition == null) return;

            var compositionModel = (CompositionModel)selectedComposition.ToModel();
            var projectModel = new ProjectModel
            {
                MasterBeat = (MasterBeatModel)Project.MasterBeat.ToModel(),
                CompositionManager = new PrefabManagerModel
                {
                    ManagerData = new ManagerDataModel
                    {
                        Items = new Collection<IControlModel> { compositionModel },
                        SelectedIndex = 0
                    }
                }
            };
            ProjectSerializer.Save(projectModel, path);
        }

        public void Receive(IMessage message)
        {
            //MainMenuMessenger.Receive(this, message);
        }

        public IControlModel ToModel() => throw new NotImplementedException();
        public void FromModel(IControlModel model) => throw new NotImplementedException();
    }
}

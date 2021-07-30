// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.IO;
using System.Windows.Input;
using Ceras;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using MvvmDialogs.FrameworkDialogs.SaveFile;

namespace CMiX.Core.Presentation.ViewModels
{
    public class MainMenu : ObservableRecipient
    {
        public MainMenu(IProject project, IDialogService dialogService)
        {
            Project = project;
            DialogService = dialogService;
            Serializer = new CerasSerializer();

            IsActive = true;

            AddCompositionCommand = new RelayCommand(AddComposition);
            NewProjectCommand = new RelayCommand(NewProject);
            OpenProjectCommand = new RelayCommand(OpenProject);
            SaveProjectCommand = new RelayCommand(SaveProject);
            SaveAsProjectCommand = new RelayCommand(SaveAsProject);
        }


        public CerasSerializer Serializer { get; set; }
        public IDialogService DialogService { get; set; }
        public IProject Project { get; set; }
        public ICommand NewProjectCommand { get; }
        public ICommand SaveProjectCommand { get; }
        public ICommand SaveAsProjectCommand { get; }
        public ICommand OpenProjectCommand { get; }
        public ICommand AddCompositionCommand { get; }
        public string FolderPath { get; set; }


        public void AddComposition()
        {
            Messenger.Send<IMessage, int>(new MessageAddComposition(Project), MessageType.Internal);
        }

        private void NewProject()
        {
            var projectModel = new ProjectModel();
            Project.SetViewModel(projectModel);
            //Project = new Project();
            //AssetManager = new AssetManager(project);
        }

        private void OpenProject()
        {
            OpenFileDialogSettings settings = new OpenFileDialogSettings();
            settings.Filter = "Project (*.cmix)|*.cmix";

            bool? success = DialogService.ShowOpenFileDialog(this, settings);
            if (success == true)
            {
                string folderPath = settings.FileName;
                if (settings.FileName.Trim() != string.Empty) // Check if you really have a file name 
                {
                    byte[] data = File.ReadAllBytes(folderPath);
                    NewProject();
                    //Project.SetViewModel(Serializer.Deserialize<ProjectModel>(data));
                    //FolderPath = folderPath;
                }
            }
        }

        private void SaveProject()
        {
            if (!string.IsNullOrEmpty(FolderPath))
            {
                var data = Serializer.Serialize(Project.GetModel());
                File.WriteAllBytes(FolderPath, data);
                return;
            }
            SaveAsProject();
        }

        private void SaveAsProject()
        {
            SaveFileDialogSettings settings = new SaveFileDialogSettings();
            settings.Filter = "Project (*.cmix)|*.cmix";
            settings.DefaultExt = "cmix";
            settings.AddExtension = true;

            bool? success = DialogService.ShowSaveFileDialog(this, settings);
            if (success == true)
            {
                var data = Serializer.Serialize(Project.GetModel());
                string folderPath = settings.FileName;
                File.WriteAllBytes(folderPath, data);
                FolderPath = folderPath;
            }
        }
    }
}

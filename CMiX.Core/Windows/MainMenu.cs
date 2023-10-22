// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using Ceras;
using CMiX.Core.Compositing;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.ViewModels
{
    public class MainMenu : ObservableRecipient
    {
        public MainMenu(Project project)
        {
            Project = project;
            Serializer = new CerasSerializer();

            IsActive = true;

            AddLayerCommand = new RelayCommand(AddLayer);
            AddCompositionCommand = new RelayCommand(AddComposition);
            NewProjectCommand = new RelayCommand(NewProject);
            OpenProjectCommand = new RelayCommand(OpenProject);
            SaveProjectCommand = new RelayCommand(SaveProject);
            SaveAsProjectCommand = new RelayCommand(SaveAsProject);
        }


        public CerasSerializer Serializer { get; set; }
        public Project Project { get; set; }
        public ICommand NewProjectCommand { get; }
        public ICommand SaveProjectCommand { get; }
        public ICommand SaveAsProjectCommand { get; }
        public ICommand OpenProjectCommand { get; }
        public ICommand AddCompositionCommand { get; }
        public ICommand AddLayerCommand { get; }


        public string FolderPath { get; set; }

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
            ProjectModel projectModel = new ProjectModel();
        }

        private void OpenProject()
        {
            //OpenFileDialogSettings settings = new OpenFileDialogSettings();
            //settings.Filter = "Project (*.cmix)|*.cmix";

            //bool? success = DialogService.ShowOpenFileDialog(this, settings);
            //if (success == true)
            //{
            //    string folderPath = settings.FileName;
            //    if (settings.FileName.Trim() != string.Empty) // Check if you really have a file name 
            //    {
            //        byte[] data = File.ReadAllBytes(folderPath);
            //        NewProject();
            //        //map model to viewmodel
            //        //FolderPath = folderPath;
            //    }
            //}
        }

        private void SaveProject()
        {
            //if (!string.IsNullOrEmpty(FolderPath))
            //{
            //    var data = Serializer.Serialize(Project.GetModel());
            //    File.WriteAllBytes(FolderPath, data);
            //    return;
            //}
            //SaveAsProject();
        }

        private void SaveAsProject()
        {
            //SaveFileDialogSettings settings = new SaveFileDialogSettings();
            //settings.Filter = "Project (*.cmix)|*.cmix";
            //settings.DefaultExt = "cmix";
            //settings.AddExtension = true;

            //bool? success = DialogService.ShowSaveFileDialog(this, settings);
            //if (success == true)
            //{
            //    var data = Serializer.Serialize(Project.GetModel());
            //    string folderPath = settings.FileName;
            //    File.WriteAllBytes(folderPath, data);
            //    FolderPath = folderPath;
            //}
        }
    }
}

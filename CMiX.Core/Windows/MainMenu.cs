// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using AutoMapper;
using Ceras;
using CMiX.Core.Compositing;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs;
using CMiX.Core.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using MvvmDialogs.FrameworkDialogs.SaveFile;

namespace CMiX.Core.ViewModels
{
    public partial class MainMenu : ObservableRecipient, IRecipient<IMessage>
    {
        public MainMenu(Project project, IMapper mapper, ControlFactory controlFactory, MainMenuMessenger mainMenuMessenger)
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

            DialogService = new DialogService();
            Mapper = mapper;
            ControlFactory = controlFactory;

            MainMenuMessenger = mainMenuMessenger;
        }


        public IMapper Mapper { get; set; }
        public ControlFactory ControlFactory { get; set; }
        public DialogService DialogService { get; set; }
        public CerasSerializer Serializer { get; set; }
        public Project Project { get; set; }
        public MainMenuMessenger MainMenuMessenger { get; set; }

        public ICommand NewProjectCommand { get; }
        public ICommand SaveProjectCommand { get; }
        public ICommand SaveAsProjectCommand { get; }
        public ICommand OpenProjectCommand { get; }
        public ICommand AddCompositionCommand { get; }
        public ICommand AddLayerCommand { get; }



        [ObservableProperty]
        private bool path;

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

            OpenFileDialogSettings settings = new OpenFileDialogSettings();
            settings.Filter = "Project (*.cmix)|*.cmix";

            bool? success = DialogService.ShowOpenFileDialog(this, settings);
            if (success == true)
            {
                string folderPath = settings.FileName;
                if (settings.FileName.Trim() != string.Empty) // Check if you really have a file name 
                {
                    byte[] data = File.ReadAllBytes(folderPath);
                    var compositionModel = Serializer.Deserialize<CompositionModel>(data);
                    var composition = ControlFactory.Create(compositionModel);
                    Project.CompositionManager.SelectedItem = composition;

                    MainMenuMessenger.SendOpenProject(folderPath);
                }
            }
        }

        public IControl Composition { get; set; }

        public void OpenProject(string filePath)
        {
            byte[] data = File.ReadAllBytes(filePath); 
            var compositionModel = Serializer.Deserialize<CompositionModel>(data);
            var composition = ControlFactory.Create(compositionModel);
            Composition = composition;

            Project.CompositionManager.SelectedItem = composition;
            Project.CompositionManager.ManagerData.Items.Insert(0, composition);
            Project.CompositionManager.ManagerData.SelectedIndex = 0;
        }


        private void SaveProject()
        {
            if (!string.IsNullOrEmpty(FolderPath))
            {
                var model = Mapper.Map(Project, typeof(Project), typeof(ProjectModel));
                var data = Serializer.Serialize(model);
                File.WriteAllBytes(FolderPath, data);
                return;
            }
            SaveAsProject();
        }

        private void SaveAsProject()
        {
            SaveFileDialogSettings settings = new SaveFileDialogSettings();
            settings.Filter = "Composition (*.cmix)|*.cmix";
            settings.DefaultExt = "cmix";
            settings.AddExtension = true;

            bool? success = DialogService.ShowSaveFileDialog(this, settings);
            if (success == true)
            {
                var model = Mapper.Map(Project.CompositionManager.SelectedItem, typeof(Composition), typeof(CompositionModel));
                var data = Serializer.Serialize(model);
                string folderPath = settings.FileName;
                File.WriteAllBytes(folderPath, data);
                FolderPath = folderPath;
            }
        }

        public void Receive(IMessage message)
        {
            MainMenuMessenger.Receive(this, message);
        }
    }
}

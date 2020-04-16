﻿using Ceras;
using CMiX.MVVM.Models;
using CMiX.MVVM.Services;
using CMiX.MVVM.ViewModels;
using CMiX.ViewModels;
using Memento;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using MvvmDialogs.FrameworkDialogs.SaveFile;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace CMiX.Studio.ViewModels
{
    public class MainViewModel : ViewModel
    {
        public MainViewModel()
        {
            Mementor = new Mementor();
            Serializer = new CerasSerializer();

            DialogService = new DialogService(new CustomFrameworkDialogFactory(), new CustomTypeLocator());

            Projects = new ObservableCollection<Component>();
            CurrentProject = new Project(0, string.Empty, null, new MessageService(), Mementor, DialogService);
            Projects.Add(CurrentProject);

            ComponentEditor = new ComponentEditor(CurrentProject);
            ComponentManager = new ComponentManager(CurrentProject);
            ComponentManager.ComponentDeletedEvent += ComponentEditor.ComponentDeletedEvent;

            Outliner = new Outliner(Projects);

            CloseWindowCommand = new RelayCommand(p => CloseWindow(p));
            MinimizeWindowCommand = new RelayCommand(p => MinimizeWindow(p));
            MaximizeWindowCommand = new RelayCommand(p => MaximizeWindow(p));

            NewProjectCommand = new RelayCommand(p => NewProject());
            OpenProjectCommand = new RelayCommand(p => OpenProject());
            SaveProjectCommand = new RelayCommand(p => SaveProject());
            SaveAsProjectCommand = new RelayCommand(p => SaveAsProject());
        }

        #region PROPERTIES
        public ICommand NewProjectCommand { get; }
        public ICommand SaveProjectCommand { get; }
        public ICommand SaveAsProjectCommand { get; }
        public ICommand OpenProjectCommand { get; }

        public ICommand CloseWindowCommand { get; }

        public ICommand MinimizeWindowCommand { get; }
        public ICommand MaximizeWindowCommand { get; }

        private readonly IDialogService DialogService;

        public Mementor Mementor { get; set; }
        public CerasSerializer Serializer { get; set; }
        public ComponentEditor ComponentEditor { get; set; }
        public ComponentManager ComponentManager { get; set; }
        public Outliner Outliner { get; set; }

        public string FolderPath { get; set; }

        public ObservableCollection<Component> Projects { get; set; }

        private Project _currentProject;
        public Project CurrentProject
        {
            get { return _currentProject; }
            set { _currentProject = value; }
        }


        public void MaximizeWindow(object obj)
        {
            if (obj is Window)
            {
                var window = obj as Window;
                if (window.WindowState == WindowState.Normal)
                    window.WindowState = WindowState.Maximized;
                else
                    window.WindowState = WindowState.Normal;
            }
        }

        public void MinimizeWindow(object obj)
        {
            if (obj is Window)
                ((Window)obj).WindowState = WindowState.Minimized;
        }

        public void CloseWindow(object obj)
        {
            if (obj is Window)
            {
                var window = obj as Window;

                var modalDialog = new ModalDialog();
                bool? success = DialogService.ShowDialog(this, modalDialog);
                if (success == true)
                {
                    if (modalDialog.SaveProject)
                    {
                        var projectSaved = SaveAsProject();
                        if (projectSaved)
                            window.Close();
                    }
                    else
                        window.Close();
                }
            }
        }
        #endregion

        #region MENU METHODS
        private void NewProject()
        {
            CurrentProject.ComponentsInEditing.Clear();
            Projects.Clear();
            Projects.Add(ComponentManager.CreateProject());
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
                    CurrentProject.ComponentsInEditing.Clear();
                    Projects.Clear();

                    byte[] data = File.ReadAllBytes(folderPath);
                    var newProject = ComponentManager.CreateProject();
                    newProject.SetViewModel(Serializer.Deserialize<ProjectModel>(data));
                    Projects.Add(newProject);
                    CurrentProject = newProject;
                    FolderPath = folderPath;
                }
            }
        }

        private void SaveProject()
        {
            System.Windows.Forms.SaveFileDialog savedialog = new System.Windows.Forms.SaveFileDialog();
            if (!string.IsNullOrEmpty(FolderPath))
            {
                var data = Serializer.Serialize(CurrentProject.GetModel());
                File.WriteAllBytes(FolderPath, data);
            }
            else
            {
                SaveAsProject();
            }
        }

        private bool SaveAsProject()
        {
            SaveFileDialogSettings settings = new SaveFileDialogSettings();
            settings.Filter = "Project (*.cmix)|*.cmix";
            settings.DefaultExt = "cmix";
            settings.AddExtension = true;

            bool? success = DialogService.ShowSaveFileDialog(this, settings);
            if (success == true && CurrentProject != null)
            {
                var data = Serializer.Serialize(CurrentProject.GetModel());
                string folderPath = settings.FileName;
                File.WriteAllBytes(folderPath, data);
                FolderPath = folderPath;
                return true;
            }
            else
                return false;
        }

        private void Quit(object p)
        {
            var window = p as Window;
            window.Close();
        }
        #endregion
    }
}

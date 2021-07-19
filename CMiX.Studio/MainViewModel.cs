// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Assets;
using CMiX.Core.Presentation.ViewModels.Components;
using CMiX.Core.Presentation.ViewModels.Components.Components;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Presentation.ViewModels.Scheduling;
using MediatR;
using MvvmDialogs;
using System.Windows;
using System.Windows.Input;

namespace CMiX.Core.Presentation.ViewModels
{
    public class MainViewModel : ViewModel
    {
        public MainViewModel(IProject project, IMessageService messageService, IDialogService dialogService)
        {

            CurrentProject = project as Project;
            DialogService = dialogService;
            ServerManager = new ServerManager(messageService, dialogService);
            AssetManager = new AssetManager(project, dialogService);

            SchedulerManager = new SchedulerManager(project);

            MainMenu = new MainMenu(project, dialogService);
            ComponentManager = new ComponentManager(project);
            Outliner = new Outliner(project);
            PlaylistEditor = new PlaylistEditor(project);

            //SchedulerManager.SetCommunicator(componentCommunicator);

            CloseWindowCommand = new RelayCommand(p => CloseWindow(p));
            MinimizeWindowCommand = new RelayCommand(p => MinimizeWindow(p));
            MaximizeWindowCommand = new RelayCommand(p => MaximizeWindow(p));



            //UndoCommand = new RelayCommand(p => Undo());
            //RedoCommand = new RelayCommand(p => Redo());
        }

        //DialogService = dialogService; //new DialogService(new CustomFrameworkDialogFactory(), new CustomTypeLocator());


        public ICommand CloseWindowCommand { get; }
        public ICommand MinimizeWindowCommand { get; }
        public ICommand MaximizeWindowCommand { get; }

        //public ICommand UndoCommand { get; }
        //public ICommand RedoCommand { get; }


        //public Mementor Mementor { get; set; }


        private readonly IDialogService DialogService;
        public Outliner Outliner { get; set; }
        public ServerManager ServerManager { get; set; }
        public MessageService MessageService { get; set; }
        public PlaylistEditor PlaylistEditor { get; set; }
        public ServerManager MessengerManager { get; set; }
        public AssetManager AssetManager { get; set; }
        public ComponentManager ComponentManager { get; set; }
        public MainMenu MainMenu { get; set; } 
        public SchedulerManager SchedulerManager { get; set; }


        private Project _currentProject;
        public Project CurrentProject
        {
            get => _currentProject;
            set => SetAndNotify(ref _currentProject, value);
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
                        return;
                        //var projectSaved = SaveAsProject();
                        //if (projectSaved)
                        //    window.Close();
                    }
                    window.Close();
                }
            }
        }

        private void Quit(object p)
        {
            var window = p as Window;
            window.Close();
        }




        //public void Undo()
        //{
        //    //Mementor.Undo();
        //}

        //public void Redo()
        //{
        //    //Mementor.Redo();
        //}





    }
}

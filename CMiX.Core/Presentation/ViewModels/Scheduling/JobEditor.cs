using System.Windows.Input;
using CMiX.Core.Presentation.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmDialogs;

namespace CMiX.Core.Presentation.ViewModels.Scheduling
{
    public class JobEditor : ObservableObject
    {
        public JobEditor(IDialogService dialogService)
        {
            DialogService = dialogService;
            PopupIsOpen = false;

            ToRunType = new ToRunType();

            OpenPopupCommand = new RelayCommand(OpenPopup);
            ClosePopupCommand = new RelayCommand(ClosePopup);
            AddJobToSchedulerCommand = new RelayCommand<CompositionScheduler>(AddJobToScheduler);
            AddTaskCommand = new RelayCommand(AddTask);
        }

        private void AddTask()
        {
            Settings settings = new Settings("192.168.0.192", 2222);
            bool? success = DialogService.ShowDialog<TaskEditor>(this, settings);
            //if (success == true)
            //this.SetSettings(settings);
        }

        public void AddJobToScheduler(CompositionScheduler compositionScheduler)
        {
            if (SelectedPlaylist != null)
            {
                Job job = new JobNextComposition(JobName, SelectedPlaylist, (s) => ToRunType.SetRunType(s.WithName(JobName)));
                compositionScheduler.JobScheduler.AddJob(job);
            }
        }

        public IDialogService DialogService { get; set; }
        public ICommand AddTaskCommand { get; }
        public ICommand ClosePopupCommand { get; set; }
        public ICommand OpenPopupCommand { get; set; }
        public ICommand AddJobToSchedulerCommand { get; set; }


        private Playlist _selectedplaylist;
        public Playlist SelectedPlaylist
        {
            get => _selectedplaylist;
            set => SetProperty(ref _selectedplaylist, value);
        }

        private ToRunType _toruntype;
        public ToRunType ToRunType
        {
            get => _toruntype;
            set => SetProperty(ref _toruntype, value);
        }

        private string _jobName;
        public string JobName
        {
            get => _jobName;
            set => SetProperty(ref _jobName, value);
        }

        private bool _popupIsOpen;
        public bool PopupIsOpen
        {
            get => _popupIsOpen;
            set => SetProperty(ref _popupIsOpen, value);
        }




        public void ClosePopup()
        {
            PopupIsOpen = false;
        }

        public void OpenPopup()
        {
            PopupIsOpen = true;
        }
    }
}

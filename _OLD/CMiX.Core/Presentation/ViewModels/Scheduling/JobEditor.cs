// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmDialogs;

namespace CMiX.Core.Presentation.ViewModels.Scheduling
{
    public class JobEditor : ObservableObject, IModalDialogViewModel
    {
        public JobEditor(IProject project)
        {
            Playlists = project.Playlists;
            ToRunType = new ToRunType();
            ApplyCommand = new RelayCommand(Apply);
        }

        public ICommand ApplyCommand { get; set; }
        public ObservableCollection<Playlist> Playlists { get; set; }
        public bool? DialogResult { get; set; }

        public void Apply()
        {
            if (SelectedPlaylist != null)
                DialogResult = true;
        }


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
    }
}

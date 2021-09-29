using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CMiX.Core.Models.Scheduler;
using CMiX.Core.Presentation.ViewModels.Components;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Presentation.ViewModels.Scheduling
{
    public class PlaylistEditor : ObservableObject
    {
        public PlaylistEditor(IProject project)
        {
            Project = project;
            Playlists = new ObservableCollection<Playlist>();

            AddItemCommand = new RelayCommand(NewPlaylist);
            DeleteItemCommand = new RelayCommand(DeletePlaylist);

            DeleteSelectedCompoCommand = new RelayCommand(DeleteSelectedCompo);
            DeleteAllCompoCommand = new RelayCommand(DeleteAllCompo);
            AddCompositionToPlaylistCommand = new RelayCommand<Composition>(AddCompositionToPlaylist);
            RemoveCompositionFromPlaylistCommand = new RelayCommand<Composition>(RemoveCompositionFromPlaylist);
        }

        int plCreateIndex = 0;

        public IProject Project { get; set; }

        public ICommand AddCompositionToPlaylistCommand { get; set; }
        public ICommand RemoveCompositionFromPlaylistCommand { get; set; }
        public ICommand AddItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }
        public ICommand DeleteSelectedCompoCommand { get; set; }
        public ICommand DuplicateSelectedCompoCommand { get; set; }
        public ICommand DeleteAllCompoCommand { get; set; }

        public ObservableCollection<Playlist> Playlists { get; set; }


        private bool _dropDownOpen;
        public bool DropDownOpen
        {
            get => _dropDownOpen;
            set => SetProperty(ref _dropDownOpen, value);
        }

        private Playlist _selectedplaylist;
        public Playlist SelectedPlaylist
        {
            get => _selectedplaylist;
            set => SetProperty(ref _selectedplaylist, value);
        }

        private Composition _selectedComposition;
        public Composition SelectedComposition
        {
            get => _selectedComposition;
            set => SetProperty(ref _selectedComposition, value);
        }


        public void AddCompositionToPlaylist(Composition composition)
        {
            if (composition != null)
            {
                SelectedPlaylist.Compositions.Add(composition);
                DropDownOpen = false;
                Console.WriteLine("Composition added to playlist " + SelectedPlaylist.Name);
            }
        }

        public void RemoveCompositionFromPlaylist(Composition composition)
        {
            if (composition != null)
            {
                SelectedPlaylist.Compositions.Remove(composition);
            }
        }

        public void DeleteSelectedCompo()
        {
            if (SelectedComposition != null)
                SelectedPlaylist.Compositions.Remove(SelectedComposition);
        }

        public void DeleteAllCompo()
        {
            if (SelectedPlaylist != null)
                SelectedPlaylist.Compositions.Clear();
        }

        public void NewPlaylist()
        {
            plCreateIndex++;
            Playlist playlist = new Playlist(new PlaylistModel());
            playlist.Name = $"Playlist ({plCreateIndex})";
            Playlists.Add(playlist);
            SelectedPlaylist = playlist;

            Console.WriteLine("New Playlist Created");
        }

        public void DeletePlaylist()
        {
            int index = Playlists.IndexOf(SelectedPlaylist);

            if (SelectedPlaylist != null)
                Playlists.Remove(SelectedPlaylist);

            if (index > 0)
            {
                SelectedPlaylist = Playlists[index - 1];
                return;
            }

            if (index == 0 && Playlists.Count > 0)
            {
                SelectedPlaylist = Playlists[0];
                return;
            }

            if (Playlists.Count == 0)
                plCreateIndex = 0;
        }
    }
}

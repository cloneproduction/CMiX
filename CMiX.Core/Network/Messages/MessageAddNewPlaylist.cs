// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models.Scheduler;
using CMiX.Core.Presentation.ViewModels.Scheduling;

namespace CMiX.Core.Network.Messages
{
    internal class MessageAddNewPlaylist : Message
    {
        public MessageAddNewPlaylist()
        {

        }

        public MessageAddNewPlaylist(Playlist playlist)
        {
            PlaylistModel = playlist.GetModel() as PlaylistModel;
        }

        public PlaylistModel PlaylistModel { get; set; }

        public override void Process<T>(T receiver)
        {
            PlaylistEditor playlistEditor = receiver as PlaylistEditor;
            Playlist playlist = new Playlist(PlaylistModel);
            playlistEditor.Project.Playlists.Add(playlist);
            playlistEditor.SelectedPlaylist = playlist;
        }
    }
}

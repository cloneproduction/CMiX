// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentScheduler;

namespace CMiX.Core.ViewModels.Scheduling
{
    public abstract class Job : ObservableObject, IJob
    {
        public Action<Schedule> Action { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private DateTime _nextRun;
        public DateTime NextRun
        {
            get => _nextRun;
            set => SetProperty(ref _nextRun, value);
        }

        private bool _disabled;
        public bool Disabled
        {
            get => _disabled;
            set => SetProperty(ref _disabled, value);
        }

        private Playlist _playlist;
        public Playlist Playlist
        {
            get => _playlist;
            set => SetProperty(ref _playlist, value);
        }

        public abstract void Execute();
        public abstract IModel GetModel();
    }
}

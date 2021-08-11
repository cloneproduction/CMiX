// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Models.Beat;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Presentation.ViewModels.Beat
{
    public class Resync : ObservableObject, IControl
    {
        public Resync(BeatAnimations beatAnimations, ResyncModel resyncModel)
        {
            BeatAnimations = beatAnimations;
            ResyncCommand = new RelayCommand(DoResync);
        }


        public Guid ID { get; set; }
        public BeatAnimations BeatAnimations { get; set; }
        public ICommand ResyncCommand { get; }

        private bool _resynced;
        public bool Resynced
        {
            get { return _resynced; }
            set { _resynced = value; }
        }


        public void DoResync()
        {
            BeatAnimations.ResetAnimation();
            OnBeatResync();
        }


        public event EventHandler BeatResync;
        public void OnBeatResync()
        {
            EventHandler handler = BeatResync;
            if (null != handler) handler(this, EventArgs.Empty);
        }

        public void SetViewModel(IModel model)
        {
            ResyncModel resyncModel = model as ResyncModel;
            this.ID = resyncModel.ID;
        }

        public IModel GetModel()
        {
            ResyncModel model = new ResyncModel();
            model.ID = this.ID;
            return model;
        }
    }
}

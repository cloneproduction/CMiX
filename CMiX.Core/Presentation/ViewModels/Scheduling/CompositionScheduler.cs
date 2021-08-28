// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using CMiX.Core.Models;
using CMiX.Core.Models.Scheduling;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels.Scheduling
{
    public class CompositionScheduler : ObservableObject, IControl
    {
        public CompositionScheduler(CompositionSchedulerModel compositionSchedulerModel, ObservableCollection<Playlist> playlists)
        {
            this.ID = compositionSchedulerModel.ID;
            this.Name = $"Scheduler({SchedulerID})";
            SchedulerID++;

            JobScheduler = new JobScheduler(compositionSchedulerModel.JobSchedulerModel);
            JobEditor = new JobEditor(compositionSchedulerModel.JobEditorModel);
        }

        public static int SchedulerID = -1;

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }


        public Guid ID { get; set; }


        public JobScheduler JobScheduler { get; set; }
        public JobEditor JobEditor { get; set; }


        public IModel GetModel()
        {
            CompositionSchedulerModel compositionSchedulerModel = new CompositionSchedulerModel();
            compositionSchedulerModel.ID = this.ID;
            compositionSchedulerModel.JobEditorModel = (JobEditorModel)this.JobEditor.GetModel();
            compositionSchedulerModel.JobSchedulerModel = (JobSchedulerModel)this.JobScheduler.GetModel();
            return compositionSchedulerModel;
        }

        public void SetViewModel(IModel model)
        {
            CompositionSchedulerModel compositionSchedulerModel = model as CompositionSchedulerModel;
            this.ID = compositionSchedulerModel.ID;
            this.JobEditor.SetViewModel(compositionSchedulerModel.JobEditorModel);
            this.JobScheduler.SetViewModel(compositionSchedulerModel.JobSchedulerModel);
        }
    }
}

// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Models.Scheduling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentScheduler;

namespace CMiX.Core.Presentation.ViewModels.Scheduling
{
    public class CompositionScheduler : ObservableObject, IControl
    {
        public CompositionScheduler(CompositionSchedulerModel compositionSchedulerModel)
        {
            this.ID = compositionSchedulerModel.ID;
            this.Name = $"Scheduler({SchedulerID})";
            SchedulerID++;
            Schedules = new ObservableCollection<Job>();
            RemoveJobCommand = new RelayCommand<Job>(RemoveJob);
        }


        public ICommand RemoveJobCommand { get; set; }
        public static int SchedulerID = -1;

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }


        public Guid ID { get; set; }
        public ObservableCollection<Job> Schedules { get; set; }


        public void AddJob(Job job)
        {
            JobManager.AddJob(job, job.Action);
            Schedules.Add(job);
        }

        public void RemoveJob(Job job)
        {
            JobManager.RemoveJob(job.Name);
            Schedules.Remove(job);
        }


        public IModel GetModel()
        {
            CompositionSchedulerModel compositionSchedulerModel = new CompositionSchedulerModel();
            compositionSchedulerModel.ID = this.ID;
            return compositionSchedulerModel;
        }
    }
}

// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using CMiX.Core.Presentations.Scheduling;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentScheduler;

namespace CMiX.Core.Presentations.ViewModels.Scheduling
{
    public class JobScheduler : ObservableObject, IControl
    {
        public JobScheduler(JobSchedulerModel jobSchedulerModel)
        {
            this.ID = jobSchedulerModel.ID;
            Schedules = new ObservableCollection<Job>();
        }


        public ObservableCollection<Job> Schedules { get; set; }
        public Guid ID { get; set; }


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
            JobSchedulerModel jobSchedulerModel = new JobSchedulerModel();
            jobSchedulerModel.ID = this.ID;
            return jobSchedulerModel;
        }
    }
}

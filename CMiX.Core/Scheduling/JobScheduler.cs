// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using CMiX.Core.Scheduling;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentScheduler;

namespace CMiX.Core.ViewModels.Scheduling
{
    public class JobScheduler : ObservableObject, IControl
    {
        public JobScheduler()
        {
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
            JobManager.RemoveJob(job.PrefabService.Name.Value);
            Schedules.Remove(job);
        }
    }
}

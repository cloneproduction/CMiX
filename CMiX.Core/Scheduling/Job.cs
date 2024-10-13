// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentScheduler;

namespace CMiX.Core.Scheduling
{
    public class Job : ObservableObject, IPrefab//, IJob
    {
        public Job(PrefabService prefabService, GenericValue<float> floatTest)
        {
            PrefabService = prefabService;
            FloatTest = floatTest;
            //var schedule = JobManager.GetSchedule(this.Name);
        }


        //public Action<Schedule> Action { get; set; }
        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }

        public GenericValue<float> FloatTest { get; set; }
        //private DateTime _nextRun;
        //public DateTime NextRun
        //{
        //    get => _nextRun;
        //}


        public event EventHandler OnTriggerBySchedule;

        public void Execute()
        {
            OnTriggerBySchedule.Invoke(this, EventArgs.Empty);
        }
    }
}

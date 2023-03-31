using System;
using CMiX.Core.Presentations.Components.Composition;
using CMiX.Core.Presentations.Scheduling;
using FluentScheduler;

namespace CMiX.Core.Presentation.ViewModels.Scheduling
{
    public class JobNextComposition : Job
    {
        public JobNextComposition(string name, Playlist playlist, Action<Schedule> action)
        {
            this.Name = name;
            this.Action = action;
            this.Playlist = playlist;
        }


        public Composition _currentComposition;
        public Composition CurrentComposition
        {
            get => _currentComposition;
            set => SetProperty(ref _currentComposition, value);
        }


        public bool Pause { get; set; }
        public Guid ID { get; set; }

        int CompositionIndex = -1;


        public override void Execute()
        {
            if (!Pause)
                Next();
            var schedule = JobManager.GetSchedule(this.Name);
            this.NextRun = schedule.NextRun;
            Console.WriteLine("JobNextComposition NowPlayer : " + CurrentComposition.Name);
        }

        public void Next()
        {
            CompositionIndex += 1;

            if (CompositionIndex > Playlist.Compositions.Count - 1)
                CompositionIndex = 0;

            CurrentComposition = Playlist.Compositions[CompositionIndex];
        }

        public void Previous()
        {
            CompositionIndex -= 1;
            if (CompositionIndex < 0)
                CompositionIndex = Playlist.Compositions.Count - 1;

            CurrentComposition = Playlist.Compositions[CompositionIndex];
        }

        public override IModel GetModel()
        {
            JobModel jobModel = new JobModel();
            jobModel.ID = this.ID;
            jobModel.Name = this.Name;

            return jobModel;
        }
    }
}

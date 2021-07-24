// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentation.ViewModels.Scheduling;

namespace CMiX.Core.Network.Messages
{
    public class MessageSelectedSchedulerIndex : ISchedulerMessage
    {
        public MessageSelectedSchedulerIndex()
        {

        }

        public MessageSelectedSchedulerIndex(int selectedIndex)
        {
            SelectedIndex = selectedIndex;
        }


        public int SelectedIndex { get; set; }
        public void Process<T>(T receiver)
        {
            SchedulerManager schedulerManager = receiver as SchedulerManager;
            //schedulerManager.SelectedSchedulerIndex = SelectedIndex;
            //schedulerManager.SelectedScheduler = schedulerManager.CompositionSchedulers[SelectedIndex];
        }
    }
}

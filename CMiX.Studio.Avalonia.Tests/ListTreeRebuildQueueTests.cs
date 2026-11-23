// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using CMiX.Studio.Avalonia.Views;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class ListTreeRebuildQueueTests
    {
        private int rebuilds;

        private ListTreeRebuildQueue CreateQueue() => new(() => rebuilds++);

        [AvaloniaFact]
        public void Post_TwoChanges_RebuildsOnceAfterTheDispatcher()
        {
            var queue = CreateQueue();

            queue.Post();
            queue.Post();
            var before = rebuilds;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(0, before);
            Assert.Equal(1, rebuilds);
            Assert.False(queue.IsPosted);
        }

        [AvaloniaFact]
        public void Run_APostedRebuild_RebuildsNowAndNotAgain()
        {
            var queue = CreateQueue();
            queue.Post();

            queue.Run();
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(1, rebuilds);
        }

        [AvaloniaFact]
        public void Run_NothingPosted_DoesNotRebuild()
        {
            var queue = CreateQueue();

            queue.Run();

            Assert.Equal(0, rebuilds);
        }
    }
}

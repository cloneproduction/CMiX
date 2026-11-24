// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia.Threading;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // A rest of the pointer next to a closed card during a drag of the tree. After the delay the session opens the card.
    internal sealed class ListTreeDragRest : ListTreeBehavior
    {
        public static readonly TimeSpan RestDelay = TimeSpan.FromMilliseconds(500);

        private readonly DispatcherTimer timer = new() { Interval = RestDelay };

        private TreeNode? resting;

        public event EventHandler? Rested;

        public TreeNode? RestingNode => resting;

        public bool IsTimerRunning => timer.IsEnabled;

        protected override void OnWatch(ProjectTree tree) => timer.Tick += OnTick;

        protected override void OnUnwatch()
        {
            Rest(null);
            timer.Tick -= OnTick;
        }

        public void Rest(TreeNode? near)
        {
            if (ReferenceEquals(near, resting))
                return;

            timer.Stop();
            resting = near;
            if (near != null)
                timer.Start();
        }

        public TreeNode? TakeResting()
        {
            var card = resting;
            Rest(null);
            return card;
        }

        private void OnTick(object? sender, EventArgs e) => Rested?.Invoke(this, EventArgs.Empty);
    }
}

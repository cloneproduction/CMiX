// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Xaml.Interactivity;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // A behavior of the new tree. It watches the tree in the DataContext while the view is in the visual tree.
    internal abstract class ListTreeBehavior : Behavior<ListTreeProject>
    {
        private ProjectTree? watched;

        protected ListTreeContext Context => AssociatedObject!.Context;

        protected T Part<T>() where T : ListTreeBehavior => Context.Part<T>();

        protected virtual void OnWatch(ProjectTree tree)
        {
        }

        protected virtual void OnUnwatch()
        {
        }

        protected override void OnAttachedToVisualTree()
        {
            base.OnAttachedToVisualTree();
            Watch();
        }

        protected override void OnDetachedFromVisualTree()
        {
            Unwatch();
            base.OnDetachedFromVisualTree();
        }

        protected override void OnDataContextChangedEvent()
        {
            base.OnDataContextChangedEvent();
            if (AssociatedObject?.Context.Lifetime.IsAttached != true)
                return;

            Unwatch();
            Watch();
        }

        private void Watch()
        {
            watched = AssociatedObject!.DataContext as ProjectTree;
            if (watched != null)
                OnWatch(watched);
        }

        private void Unwatch()
        {
            if (watched == null)
                return;

            watched = null;
            OnUnwatch();
        }
    }
}

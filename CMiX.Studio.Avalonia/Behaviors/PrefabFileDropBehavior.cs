// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using CMiX.Core.Prefabs.Managers;
using CMiX.Studio.Avalonia.Session;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.UI.Behaviors;

namespace CMiX.Studio.Avalonia.Behaviors
{
    // Files and folders dropped on the control become items of the prefab manager.
    // PathDropRules holds the rules. A drop of several paths is one undo step.
    public abstract class PrefabFileDropBehavior : FileDropBehavior
    {
        static PrefabFileDropBehavior()
        {
            ModeProperty.OverrideDefaultValue<PrefabFileDropBehavior>(FileDropMode.FilesAndFolders);
        }

        public static readonly StyledProperty<PrefabManager> PrefabManagerProperty =
            AvaloniaProperty.Register<PrefabFileDropBehavior, PrefabManager>(nameof(PrefabManager));

        public PrefabManager PrefabManager
        {
            get => GetValue(PrefabManagerProperty);
            set => SetValue(PrefabManagerProperty, value);
        }

        public static readonly StyledProperty<PathDropRules?> PathDropRulesProperty =
            AvaloniaProperty.Register<PrefabFileDropBehavior, PathDropRules?>(nameof(PathDropRules));

        // The rules. When this is not set, the behavior uses the service of the view model of the window.
        public PathDropRules? PathDropRules
        {
            get => GetValue(PathDropRulesProperty);
            set => SetValue(PathDropRulesProperty, value);
        }

        protected PathDropRules? Service =>
            PathDropRules
            ?? (TopLevel.GetTopLevel(AssociatedObject)?.DataContext as MainViewModel)?.PathDropRules;

        // True when Add makes an item from this path. DragOver uses it, once per path and drag.
        protected abstract override bool Accepts(string path);

        // Adds the item of one path. Service and PrefabManager are not null here.
        protected abstract void Add(string path);

        protected override void OnPathsDropped(IReadOnlyList<string> paths)
        {
            base.OnPathsDropped(paths);

            if (PrefabManager == null || Service == null)
                return;

            using (PrefabManager.UndoManager.Capture())
            {
                foreach (var path in paths)
                    Add(path);
            }
        }
    }
}

// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The view pushes its pointer and its focus here. The hooks of TreeViewSet read it.
    public sealed partial class ListTreeViewState : ObservableObject
    {
        [ObservableProperty]
        private bool isPointerOverTree;

        [ObservableProperty]
        private bool isTreeFocused;

        [ObservableProperty]
        private bool isTextBoxFocused;

        [ObservableProperty]
        private bool isFocusInOtherPanel;

        [ObservableProperty]
        private bool isPanelFocused;
    }
}

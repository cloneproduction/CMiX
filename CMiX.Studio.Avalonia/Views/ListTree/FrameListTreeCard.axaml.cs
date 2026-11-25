// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // The area of a card of the tree, under its rows: from the top of the header to the bottom of its last shown row.
    public partial class FrameListTreeCard : UserControl
    {
        public const string DropTargetClass = ":droptarget";

        public static readonly StyledProperty<ListTreePlace?> PlaceProperty =
            AvaloniaProperty.Register<FrameListTreeCard, ListTreePlace?>(nameof(Place));

        public FrameListTreeCard()
        {
            InitializeComponent();
        }

        public ListTreePlace? Place
        {
            get => GetValue(PlaceProperty);
            set => SetValue(PlaceProperty, value);
        }

        internal Border Surface => surface;

        public void ShowMarks(ListTreeMarks marks) => PseudoClasses.Set(DropTargetClass, marks.IsDropTarget(DataContext as TreeNode));
    }
}

// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // The area of a card of the new tree, under its rows: from the top of the header to the bottom of its last shown row.
    public partial class FrameListTreeCard : UserControl
    {
        public const string HoveredClass = ":hovered";
        public const string SelectedClass = ":selected";
        public const string InSelectionClass = ":inselection";

        public FrameListTreeCard()
        {
            InitializeComponent();
        }

        public ListTreePlace? Place { get; set; }

        public Thickness IndentMargin => new(Place?.Indent ?? 0, 0, 0, 0);

        internal Border Surface => surface;

        internal Border Mark => mark;

        public void ShowMarks(ListTreeMarks marks)
        {
            var node = DataContext as TreeNode;
            PseudoClasses.Set(HoveredClass, marks.IsHoveredCard(node));
            PseudoClasses.Set(SelectedClass, marks.IsSelected(node));
            PseudoClasses.Set(InSelectionClass, marks.IsInSelection(node));
        }
    }
}

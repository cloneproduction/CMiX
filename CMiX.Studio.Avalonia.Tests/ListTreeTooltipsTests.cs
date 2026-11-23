// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Panels;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // The tooltips of the rows of the new list tree open and close as in the old tree (TreeTooltipsTests), with the fill of the old row.
    public class ListTreeTooltipsTests : IDisposable
    {
        private readonly Scene scene = CreateScene([1], s => AddKinds(s.Layer));
        private readonly Window window;
        private readonly TreeViewProject oldView;
        private readonly ListTreeProject newView;

        public ListTreeTooltipsTests()
        {
            oldView = new TreeViewProject { DataContext = scene.Tree };
            newView = new ListTreeProject { DataContext = scene.Tree };
            newView.Rows.MoveDuration = TimeSpan.Zero;
            var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*") };
            Grid.SetColumn(newView, 1);
            grid.Children.Add(oldView);
            grid.Children.Add(newView);
            window = new Window { Content = grid, Width = 900, Height = 700 };
            window.Show();
            foreach (var panel in oldView.GetVisualDescendants().OfType<PanelCollapse>())
                panel.Duration = TimeSpan.Zero;
            TreeTestScreen.Settle(window);
        }

        public void Dispose() => window.Close();

        private TreeNode Composition => scene.Tree.CompositionNodes()[0];

        private static ToggleButton MaskIn(Visual row) =>
            row.GetVisualDescendants().OfType<ToggleButton>().First(toggle => Equals(ToolTip.GetTip(toggle), "Mask") && toggle.IsEffectivelyVisible);

        private bool HasNewMask(TreeNode node) =>
            newView.Rows.RowOf(node) is { } row
            && row.GetVisualDescendants().OfType<ToggleButton>().Any(toggle => Equals(ToolTip.GetTip(toggle), "Mask") && toggle.IsEffectivelyVisible);

        private ToggleButton NewMaskOf(TreeNode node) => MaskIn(Assert.IsType<RowListTree>(newView.Rows.RowOf(node)));

        private Point MiddleOf(Control control)
        {
            TreeTestScreen.Settle(window);
            return control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
        }

        private void MoveTo(Point point)
        {
            window.MouseMove(point);
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void Create_TheNewTree_SwitchesTheTooltipServiceOffForItsRows()
        {
            Assert.False(ToolTip.GetServiceEnabled(newView.Rows));
            Assert.False(ToolTip.GetServiceEnabled(NewMaskOf(Composition)));
        }

        [AvaloniaFact]
        public void MouseMove_OntoTheMaskWithNoDelay_OpensItsTooltip()
        {
            var mask = NewMaskOf(Composition);
            ToolTip.SetShowDelay(mask, 0);

            MoveTo(MiddleOf(mask));

            Assert.True(ToolTip.GetIsOpen(mask));
            Assert.Same(mask, newView.Tooltips.Open);
        }

        [AvaloniaFact]
        public void KeyPress_WithAnOpenTooltip_ClosesItAndMutesTheTooltips()
        {
            var mask = NewMaskOf(Composition);
            ToolTip.SetShowDelay(mask, 0);
            MoveTo(MiddleOf(mask));
            Assert.True(ToolTip.GetIsOpen(mask));

            window.KeyPressQwerty(PhysicalKey.A, RawInputModifiers.None);
            Dispatcher.UIThread.RunJobs();

            Assert.False(ToolTip.GetIsOpen(mask));
            Assert.Null(newView.Tooltips.Open);
            Assert.True(newView.Tooltips.IsMuted);
        }

        [AvaloniaFact]
        public void Show_TheMaskOfEachRow_HasTheTooltipFillOfTheOldRow()
        {
            var masked = TreeTestScreen.All(scene.Tree.Roots).Where(HasNewMask).ToList();
            Assert.NotEmpty(masked);
            foreach (var node in masked)
            {
                var oldFill = ToolTipCard.GetFill(MaskIn(TreeTestScreen.ItemOf(oldView, node)));
                var newFill = ToolTipCard.GetFill(NewMaskOf(node));

                Assert.NotNull(newFill);
                Assert.Equal(((ISolidColorBrush)oldFill!).Color, ((ISolidColorBrush)newFill!).Color);
            }
        }
    }
}

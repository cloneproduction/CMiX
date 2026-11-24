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
using CMiX.Studio.Avalonia.Themes;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls.Parts;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // The tooltips of the rows of the list tree open on the buttons of a row and close on a key. They have the hover fill of the kind of the row.
    public class ListTreeTooltipsTests : IDisposable
    {
        private readonly Scene scene = CreateScene([1], s => AddKinds(s.Layer));
        private readonly Window window;
        private readonly ListTreeProject newView;

        public ListTreeTooltipsTests()
        {
            newView = new ListTreeProject { DataContext = scene.Tree };
            newView.Rows.MoveDuration = TimeSpan.Zero;
            window = new Window { Content = newView, Width = 900, Height = 700 };
            window.Show();
            ListTreeTestScreen.Settle(window);
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
            ListTreeTestScreen.Settle(window);
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
        public void Show_TheMaskOfEachRow_HasTheHoverFillOfTheKindOfItsNode()
        {
            var masked = ListTreeTestScreen.All(scene.Tree.Roots).Where(HasNewMask).ToList();
            Assert.NotEmpty(masked);
            var palette = CategoryPalette.FromApplication();
            foreach (var node in masked)
            {
                var expected = palette.For(node.Category!.Value, node.ComposableDepth, node.KindDepth).Of(ToneRole.FillHover);

                var fill = ToolTipCard.GetFill(NewMaskOf(node));

                Assert.Equal(expected, Assert.IsAssignableFrom<ISolidColorBrush>(fill).Color);
            }
        }
    }
}

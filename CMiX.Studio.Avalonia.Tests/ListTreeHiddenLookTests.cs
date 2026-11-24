// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Converters;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using static CMiX.Studio.Avalonia.Tests.Support.ListTreeLook;

namespace CMiX.Studio.Avalonia.Tests
{
    // A hidden row of the list tree, and every row under a hidden card, show a fixed grey text and icon, never an opacity.
    // ListTreeTogglesAndBadgesTests measures the look of the toggles of these rows.
    public class ListTreeHiddenLookTests : IDisposable
    {
        private readonly Scene scene = CreateScene([2], s => AddKinds(s.Layer));
        private readonly Window window;
        private readonly ListTreeProject view;

        public ListTreeHiddenLookTests()
        {
            scene.Tree.SelectedNode = null;
            var screen = ListTreeTestScreen.Show(scene.Tree, 400, 900);
            view = screen.View;
            window = screen.Window;
        }

        public void Dispose() => window.Close();

        private TreeNode Layer => scene.LayerNodeOfA(0);

        private TreeNode Entities => scene.Group<EntitiesGroup>(0);

        private TreeNode Entity => Entities.Children[0];

        private void HideTheLayer()
        {
            scene.Layer.PrefabService.Visibility.Value = true;
            window.Settle();
        }

        private RowListTree RowOf(TreeNode node) => Assert.IsType<RowListTree>(view.Rows.RowOf(node));

        private static AppIcon IconOf(RowListTree row) => row.GetVisualDescendants().OfType<AppIcon>().First(icon => icon.Name == "kindIcon");

        private double OpacityOf(Visual visual)
        {
            var opacity = 1.0;
            for (Visual? current = visual; current != null && !ReferenceEquals(current, window); current = current.GetVisualParent())
                opacity *= current.Opacity;
            return opacity;
        }

        private void AssertHiddenLook(RowListTree row)
        {
            Assert.Equal(Token("ListTreeTextHiddenBrush"), TextOf(row));
            Assert.Equal(Token("ListTreeIconHiddenBrush"), ColorOf(IconOf(row).Foreground));
            Assert.Equal(1, OpacityOf(NameOf(row)));
            Assert.Equal(1, OpacityOf(IconOf(row)));
        }

        [AvaloniaFact]
        public void Show_AHiddenRow_HasTheHiddenTextAndIconAndNoOpacity()
        {
            HideTheLayer();

            Assert.True(Layer.IsHidden);
            AssertHiddenLook(RowOf(Layer));
        }

        [AvaloniaFact]
        public void Show_TheRowsUnderAHiddenCard_HaveTheHiddenTextAndIconAndNoOpacity()
        {
            HideTheLayer();

            Assert.True(Entity.IsUnderHiddenParent);
            AssertHiddenLook(RowOf(Entities));
            AssertHiddenLook(RowOf(Entity));
        }

        [AvaloniaFact]
        public void Show_ARowShownAgain_HasTheRestTextAndTheKindIcon()
        {
            HideTheLayer();

            scene.Layer.PrefabService.Visibility.Value = false;
            window.Settle();

            var kind = (ISolidColorBrush)new TreeToneConverter().Convert(Entity, typeof(IBrush), "KindSelected", System.Globalization.CultureInfo.InvariantCulture)!;
            Assert.Equal(Token("ListTreeTextRestBrush"), TextOf(RowOf(Entity)));
            Assert.Equal(kind.Color, ColorOf(IconOf(RowOf(Entity)).Foreground));
        }

        [AvaloniaFact]
        public void Show_ARowBesideAHiddenRow_KeepsTheRestText()
        {
            HideTheLayer();

            Assert.Equal(Token("ListTreeTextRestBrush"), TextOf(RowOf(scene.LayerNodeOfA(1))));
        }
    }
}

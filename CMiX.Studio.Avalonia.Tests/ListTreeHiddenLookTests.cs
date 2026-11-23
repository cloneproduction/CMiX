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
using CMiX.UI.Controls.String;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // A hidden row of the new list tree, and every row under a hidden card, show a fixed grey text and icon, never an opacity.
    // ListTreeTogglesAndBadgesTests measures that the toggles of these rows keep the look of the old tree.
    public class ListTreeHiddenLookTests : IDisposable
    {
        private readonly Scene scene = CreateScene([2], s => AddKinds(s.Layer));
        private readonly Window window;
        private readonly ListTreeProject view;

        public ListTreeHiddenLookTests()
        {
            scene.Tree.SelectedNode = null;
            view = new ListTreeProject { DataContext = scene.Tree };
            view.Rows.MoveDuration = TimeSpan.Zero;
            window = new Window { Content = view, Width = 400, Height = 900 };
            window.Show();
            Settle();
        }

        public void Dispose() => window.Close();

        private TreeNode Layer => scene.LayerNodeOfA(0);

        private TreeNode Entities => scene.Group<EntitiesGroup>(0);

        private TreeNode Entity => Entities.Children[0];

        private void Settle() => TreeTestScreen.Settle(window);

        private void HideTheLayer()
        {
            scene.Layer.PrefabService.Visibility.Value = true;
            Settle();
        }

        private RowListTree RowOf(TreeNode node) => Assert.IsType<RowListTree>(view.Rows.RowOf(node));

        private static Color ColorOf(IBrush? brush) => Assert.IsAssignableFrom<ISolidColorBrush>(brush).Color;

        private static Color Token(string key) => ColorOf((IBrush)Application.Current!.FindResource(key)!);

        private static Control NameOf(RowListTree row) =>
            (Control?)row.GetVisualDescendants().OfType<TextBoxEditable>().FirstOrDefault(box => box.IsVisible)
            ?? row.GetVisualDescendants().OfType<TextBlock>().First(text => text.Name == "groupName");

        private static Color TextOf(RowListTree row) =>
            NameOf(row) is TextBoxEditable box ? ColorOf(box.Foreground) : ColorOf(((TextBlock)NameOf(row)).Foreground);

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
            Settle();

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

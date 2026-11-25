// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.VisualTree;
using CMiX.Core.Prefabs;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls;
using Xunit;
using EchoFilter = CMiX.Core.Texturing.Filters.Echo;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using static CMiX.Studio.Avalonia.Tests.Support.ListTreeLook;

namespace CMiX.Studio.Avalonia.Tests
{
    // The look of the rows during a search: a match keeps its look, a parent shown for a match is dimmed.
    public class ListTreeSearchLookTests : IDisposable
    {
        // A with two layers and B with one. The first layer of A holds an Echo filter named Trail.
        private readonly Scene scene = CreateScene([2, 1], s =>
        {
            s.Layer.TextureModifierManager.AddItem(typeof(EchoFilter));
            ((IPrefab)s.Layer.TextureModifierManager.ManagerData.Items[0]).PrefabService.Name.Value = "Trail";
        });

        private readonly Window window;
        private readonly ListTreeProject view;

        public ListTreeSearchLookTests()
        {
            scene.Tree.SelectedNode = null;
            var screen = ListTreeTestScreen.Show(scene.Tree, 400, 900);
            view = screen.View;
            window = screen.Window;
        }

        public void Dispose() => window.Close();

        private TreeNode Match => scene.All().Single(node => node.Item is EchoFilter);

        private TreeNode Layer => scene.LayerNodeOfA(0);

        private RowListTree RowOf(TreeNode node) => Assert.IsType<RowListTree>(view.Rows.RowOf(ListTreeKeys.Of(node)));

        private static Color IconOf(RowListTree row) =>
            ColorOf(row.GetVisualDescendants().OfType<AppIcon>().First(icon => icon.Name == "kindIcon").Foreground);

        private static IEnumerable<ToggleButton> TogglesOf(RowListTree row) =>
            row.Toggles.GetVisualDescendants().OfType<ToggleButton>().Where(toggle => toggle.IsEffectivelyVisible);

        private void Search(string text)
        {
            scene.Tree.SearchText = text;
            window.Settle();
        }

        [AvaloniaFact]
        public void Row_AParentOfAMatch_IsDimmed()
        {
            Search("Trail");
            var row = RowOf(Layer);

            Assert.Contains("searchparent", row.Classes);
            Assert.Equal(Token("ListTreeRowDimmedBandBrush"), ColorOf(Named(row, "band").Background));
            Assert.Equal(Token("ListTreeTextDimmedBrush"), TextOf(row));
            Assert.Equal(Token("ListTreeIconDimmedBrush"), IconOf(row));
            Assert.NotEmpty(TogglesOf(row));
            Assert.All(TogglesOf(row), toggle => Assert.Equal(Token("ListTreeTextDimmedBrush"), FillOf(toggle.Foreground)));
        }

        [AvaloniaFact]
        public void Row_AMatch_KeepsItsNormalLook()
        {
            Search("Trail");
            scene.Tree.CursorNode = Layer;
            window.Settle();
            var row = RowOf(Match);

            Assert.DoesNotContain("searchparent", row.Classes);
            Assert.Equal(Token("ListTreeRowRestBandBrush"), ColorOf(Named(row, "band").Background));
            Assert.Equal(Token("ListTreeTextRestBrush"), TextOf(row));
            Assert.NotEqual(Token("ListTreeIconDimmedBrush"), IconOf(row));
        }

        [AvaloniaFact]
        public void Row_ASelectedParentOfAMatch_KeepsTheSelectedLook()
        {
            Search("Trail");
            scene.Tree.SelectedNode = Layer;
            window.Settle();
            var row = RowOf(Layer);

            Assert.Contains("searchparent", row.Classes);
            Assert.Equal(Token("ListTreeRowSelectedBrush"), FillOf(row.Background));
            Assert.Equal(Colors.Transparent, FillOf(Named(row, "band").Background));
            Assert.Equal(Token("ListTreeTextSelectedBrush"), TextOf(row));
            Assert.NotEqual(Token("ListTreeIconDimmedBrush"), IconOf(row));
        }

        [AvaloniaFact]
        public void Row_TheEndOfTheSearch_RemovesTheDimmedLook()
        {
            Search("Trail");
            Search("");
            var row = RowOf(Layer);

            Assert.DoesNotContain("searchparent", row.Classes);
            Assert.NotEqual(Token("ListTreeTextDimmedBrush"), TextOf(row));
        }
    }
}

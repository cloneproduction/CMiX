// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using CMiX.Core.Prefabs;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls;
using CMiX.UI.Controls.String;
using Xunit;
using EchoFilter = CMiX.Core.Texturing.Filters.Echo;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using static CMiX.Studio.Avalonia.Tests.Support.ListTreeLook;

namespace CMiX.Studio.Avalonia.Tests
{
    // The look of the rows during a search: a match keeps its look with its matched letters in the highlight, a parent shown for a match is dimmed.
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

        private static TextBlockHighlighted ShownNameOf(RowListTree row) =>
            row.GetVisualDescendants().OfType<TextBlockHighlighted>().First(text => text.IsEffectivelyVisible);

        private static string[] RunTextsOf(TextBlock text) => text.Inlines?.OfType<Run>().Select(run => run.Text ?? "").ToArray() ?? [];

        private static Run HighlightOf(TextBlock text) => text.Inlines!.OfType<Run>().ElementAt(1);

        [AvaloniaFact]
        public void Row_AMatch_DrawsTheMatchedLettersInTheHighlight()
        {
            Search("rai");
            var name = ShownNameOf(RowOf(Match));

            Assert.Equal("Trail", name.Text);
            Assert.Equal(new[] { "T", "rai", "l" }, RunTextsOf(name));
            Assert.Equal(Token("AccentHighlightBrush"), ColorOf(HighlightOf(name).Foreground));
            Assert.All(name.Inlines!.OfType<Run>().Where(run => run != HighlightOf(name)),
                run => Assert.NotEqual(Token("AccentHighlightBrush"), FillOf(run.Foreground)));
        }

        [AvaloniaFact]
        public void Row_AGroupMatch_DrawsTheMatchedLettersInTheHighlight()
        {
            var group = Scene.LayersOf(scene.NodeA);
            Search(group.Header.Substring(1, 3));
            var name = ShownNameOf(RowOf(group));

            Assert.Equal("groupName", name.Name);
            Assert.Equal(group.Header.Substring(1, 3), RunTextsOf(name)[1]);
            Assert.Equal(Token("AccentHighlightBrush"), ColorOf(HighlightOf(name).Foreground));
        }

        [AvaloniaFact]
        public void Row_AMatchOnTheKindOnly_DrawsNoHighlight()
        {
            Search("Echo");

            Assert.True(Match.IsSearchMatch);
            Assert.Empty(RunTextsOf(ShownNameOf(RowOf(Match))));
        }

        [AvaloniaFact]
        public void Row_TheEndOfTheSearch_RemovesTheHighlight()
        {
            Search("rai");
            Search("");
            var name = ShownNameOf(RowOf(Match));

            Assert.Equal("Trail", name.Text);
            Assert.Empty(RunTextsOf(name));
        }

        [AvaloniaFact]
        public void DoubleClick_TheHighlightedLettersOfAMatch_RenamesItAndHighlightsTheNewName()
        {
            Search("Tra");
            var box = RowOf(Match).GetVisualDescendants().OfType<TextBoxEditable>().First(edit => edit.Name == "nameBox");

            window.DoubleClick(box.TranslatePoint(new Point(10, box.Bounds.Height / 2), window)!.Value);
            var input = box.GetVisualDescendants().OfType<TextBox>().Single(edit => edit.IsVisible);
            input.Text = "Trace";
            window.Press(PhysicalKey.Enter);
            window.Settle();

            Assert.Equal("Trace", ((IPrefab)Match.Item!).PrefabService.Name.Value);
            Assert.Equal(new[] { "", "Tra", "ce" }, RunTextsOf(ShownNameOf(RowOf(Match))));
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

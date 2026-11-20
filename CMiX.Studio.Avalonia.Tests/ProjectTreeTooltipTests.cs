using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Panels;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using static CMiX.Studio.Avalonia.Tests.Support.TreeTestScreen;
using Composition = CMiX.Core.Compositing.Composition;
using Entity = CMiX.Core.Compositing.Entity;
using Layer = CMiX.Core.Compositing.Layer;
using Project = CMiX.Core.Compositing.Project;

namespace CMiX.Studio.Avalonia.Tests
{
    // The tooltips of the tree show their text, and they open below the control like every tooltip of the app.
    // A data template for any object in a parent of a control turns the text of its tooltip into an empty box.
    public class ProjectTreeTooltipTests : IDisposable
    {
        private Window? window;

        public void Dispose() => window?.Close();

        // A composition with a layer that has two entities, a filter and three modifiers.
        private (ProjectTreeView View, ProjectTree Tree) Show()
        {
            var scene = CreateScene([1], s => AddKinds(s.Layer));
            var screen = TreeTestScreen.Show(scene.Tree, 500, 600);
            window = screen.Window;
            return (screen.View, scene.Tree);
        }

        private void Settle() => TreeTestScreen.Settle(window!);

        // Opens the tooltip of a control and returns the texts that it shows. The tooltip control is an internal value of the control,
        // so it is read by its property: the test needs what the user would see.
        private List<string> TextOfTooltip(Control control)
        {
            ToolTip.SetIsOpen(control, true);
            Settle();

            var property = (AvaloniaProperty)typeof(ToolTip).GetField("ToolTipProperty", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)!.GetValue(null)!;
            var tip = (ToolTip)control.GetValue(property)!;
            var texts = tip.GetVisualDescendants().OfType<TextBlock>().Select(text => text.Text ?? string.Empty).ToList();
            ToolTip.SetIsOpen(control, false);
            return texts;
        }


        private static List<Border> SquaresOf(ProjectTreeView view, TreeNode node) =>
            ItemOf(view, node).GetVisualDescendants().OfType<KindCountBadges>().First()
                .GetVisualDescendants().OfType<Border>().Where(border => border.Classes.Contains("kindcount")).ToList();

        [AvaloniaFact]
        public void Show_TheMaskToggle_HasItsTooltipForACompositionAndALayer()
        {
            var (view, tree) = Show();

            foreach (var node in new[] { tree.CompositionNodes()[0], tree.CompositionNodes()[0].Children[0].Children[0] })
            {
                var toggle = ItemOf(view, node).GetVisualDescendants().OfType<RowToggles>().First()
                    .GetVisualDescendants().OfType<ToggleButton>().Single(button => Equals(ToolTip.GetTip(button), "Mask"));

                Assert.Equal(new[] { "Mask" }, TextOfTooltip(toggle));
            }
        }

        [AvaloniaFact]
        public void Show_TheMaskToggle_OpensItsTooltipBelowIt()
        {
            var (view, tree) = Show();
            var toggle = ItemOf(view, tree.CompositionNodes()[0]).GetVisualDescendants().OfType<RowToggles>().First()
                .GetVisualDescendants().OfType<ToggleButton>().Single(button => Equals(ToolTip.GetTip(button), "Mask"));

            Assert.Equal(PlacementMode.Custom, ToolTip.GetPlacement(toggle));
            Assert.Equal(CMiX.UI.Popups.PopupSide.Bottom, CMiX.UI.AttachedProperties.ToolTipDefaults.GetSide(toggle));
        }

        [AvaloniaFact]
        public void VisibilityToggle_Show_HasTheVisibleTip()
        {
            var (view, tree) = Show();
            var toggle = ItemOf(view, tree.CompositionNodes()[0]).GetVisualDescendants().OfType<RowToggles>().First()
                .GetVisualDescendants().OfType<ToggleButton>()
                .First(button => ReferenceEquals(button.Theme, Application.Current!.FindResource("ToggleButtonVisibility")));

            Assert.Equal("Visible", ToolTip.GetTip(toggle));
            Assert.Equal(new[] { "Visible" }, TextOfTooltip(toggle));
            Assert.Equal(PlacementMode.Custom, ToolTip.GetPlacement(toggle));
            Assert.Equal(CMiX.UI.Popups.PopupSide.Bottom, CMiX.UI.AttachedProperties.ToolTipDefaults.GetSide(toggle));
        }

        [AvaloniaFact]
        public void Show_ACountSquare_TellsHowManyOfWhichKindInItsTooltip()
        {
            var (view, tree) = Show();
            var layer = tree.CompositionNodes()[0].Children[0].Children[0];
            layer.IsExpanded = false;
            Settle();

            var texts = SquaresOf(view, layer).Select(square => TextOfTooltip(square).Single()).ToList();

            Assert.Equal(new[] { "2 Entities", "1 Filter", "3 Modifiers" }, texts);
        }

        // A row under the pointer makes the number of a square dark. That style must not reach the text of the tooltip of the square,
        // which would then be dark on the dark card and look empty.
        [AvaloniaFact]
        public void Show_ACountSquareOfAHoveredRow_KeepsTheTextOfItsTooltipBright()
        {
            var (view, tree) = Show();
            var layer = tree.CompositionNodes()[0].Children[0].Children[0];
            layer.IsExpanded = false;
            Settle();
            var badges = ItemOf(view, layer).GetVisualDescendants().OfType<KindCountBadges>().First();
            badges.IsHovered = true;
            Settle();
            var square = SquaresOf(view, layer).First();

            ToolTip.SetIsOpen(square, true);
            Settle();
            var property = (AvaloniaProperty)typeof(ToolTip).GetField("ToolTipProperty", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)!.GetValue(null)!;
            var text = ((ToolTip)square.GetValue(property)!).GetVisualDescendants().OfType<TextBlock>().Single();
            var source = global::Avalonia.Diagnostics.AvaloniaObjectExtensions.GetDiagnostic(text, TextBlock.ForegroundProperty).Priority;
            ToolTip.SetIsOpen(square, false);

            Assert.Equal("2 Entities", text.Text);
            // The color of the text comes from the template of the tooltip (ActiveForegroundBrush), not from a style of the squares.
            Assert.Equal(global::Avalonia.Data.BindingPriority.Template, source);
        }

        [AvaloniaFact]
        public void Show_TheCountOfAGroup_NamesItsKindInItsTooltip()
        {
            var (view, tree) = Show();
            var entities = tree.CompositionNodes()[0].Children[0].Children[0].Children.Single(child => child.Group is EntitiesGroup);
            entities.IsExpanded = false;
            Settle();

            var square = Assert.Single(SquaresOf(view, entities));

            Assert.Equal("2 Entities", TextOfTooltip(square).Single());
            Assert.Equal(PlacementMode.Custom, ToolTip.GetPlacement(square));
            Assert.Equal(CMiX.UI.Popups.PopupSide.Bottom, CMiX.UI.AttachedProperties.ToolTipDefaults.GetSide(square));
        }

        [Theory]
        [InlineData(TreeCategory.Composable, 1, "1 Composable")]
        [InlineData(TreeCategory.Composable, 4, "4 Composables")]
        [InlineData(TreeCategory.Entity, 1, "1 Entity")]
        [InlineData(TreeCategory.Entity, 2, "2 Entities")]
        [InlineData(TreeCategory.Camera, 1, "1 Camera")]
        [InlineData(TreeCategory.Camera, 3, "3 Cameras")]
        [InlineData(TreeCategory.Light, 1, "1 Light")]
        [InlineData(TreeCategory.Light, 2, "2 Lights")]
        [InlineData(TreeCategory.Texture, 1, "1 Texture")]
        [InlineData(TreeCategory.Texture, 2, "2 Textures")]
        [InlineData(TreeCategory.Filter, 1, "1 Filter")]
        [InlineData(TreeCategory.Filter, 5, "5 Filters")]
        [InlineData(TreeCategory.Modifier, 1, "1 Modifier")]
        [InlineData(TreeCategory.Modifier, 3, "3 Modifiers")]
        [InlineData(TreeCategory.Modulator, 1, "1 Modulator")]
        [InlineData(TreeCategory.Modulator, 2, "2 Modulators")]
        public void Describe_ACount_GivesTheNumberAndTheNameOfTheKind(TreeCategory category, int count, string expected)
        {
            Assert.Equal(expected, new KindCount(category, count).Description);
        }

        // A layer cannot hold a modifier of an entity: during its drag the layer row gives the reason in its tooltip.
        [AvaloniaFact]
        public void BlockedRow_DuringADrag_ShowsTheReason()
        {
            var (view, tree) = Show();
            var layer = tree.CompositionNodes()[0].Children[0].Children[0];
            var entity = layer.Children.Single(child => child.Group is EntitiesGroup).Children[0];
            ((Entity)entity.Item!).ModifierManager.AddItem(typeof(CMiX.Core.Modulation.Modifiers.TranslateModifier));
            Settle();
            var modifier = entity.Children.Single(child => child.Group is ModifiersGroup).Children[0];
            var row = ItemOf(view, layer).GetVisualDescendants().OfType<Border>().First(border => border.Name == "RowBorder");
            Assert.Null(ToolTip.GetTip(row));

            tree.BeginDrag(modifier);
            Settle();

            Assert.Equal(new[] { "A Translate cannot go to a layer" }, TextOfTooltip(row));
            Assert.Equal(PlacementMode.Custom, ToolTip.GetPlacement(row));
            Assert.Equal(CMiX.UI.Popups.PopupSide.Bottom, CMiX.UI.AttachedProperties.ToolTipDefaults.GetSide(row));

            tree.EndDrag();
            Settle();
            Assert.Null(ToolTip.GetTip(row));
        }

        [Fact]
        public void MaskOwner_EachKind_IsSetForACompositionAndALayerOnly()
        {
            var provider = TestServiceProviderFactory.Create();
            var project = provider.GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)project.CompositionManager.ManagerData.Items[0];
            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.ManagerData.Items[0];
            layer.ModelEntityManager.AddItem(typeof(Entity));
            var tree = TreeFixtures.CreateTree(provider);
            var compositionNode = tree.CompositionNodes()[0];
            var layerNode = compositionNode.Children[0].Children[0];
            var entities = layerNode.Children.Single(child => child.Group is EntitiesGroup);

            Assert.Same(composition, compositionNode.MaskOwner);
            Assert.Same(layer, layerNode.MaskOwner);
            Assert.Null(entities.MaskOwner);
            Assert.Null(entities.Children[0].MaskOwner);
        }
    }
}

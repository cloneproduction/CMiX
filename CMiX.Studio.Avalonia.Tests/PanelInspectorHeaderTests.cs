// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.Headless.XUnit;
using Avalonia.VisualTree;
using CMiX.Core;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.Studio.Avalonia.Views.Managers;
using CMiX.UI.Controls;
using CMiX.UI.Controls.String;
using CMiX.UI.Panels;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using CameraModel = CMiX.Core.Rendering.Cameras.Camera;
using CameraView = CMiX.Studio.Avalonia.Views.Camera;
using Composition = CMiX.Core.Compositing.Composition;
using CompositionView = CMiX.Studio.Avalonia.Views.Composition;
using FullscreenQuad = CMiX.Core.Compositing.FullscreenQuad;
using FullscreenQuadView = CMiX.Studio.Avalonia.Views.FullscreenQuad;
using Layer = CMiX.Core.Compositing.Layer;
using LightModel = CMiX.Core.Rendering.Lights.LightEntity;
using LightView = CMiX.Studio.Avalonia.Views.LightEntity;
using TextEntity = CMiX.Core.Compositing.TextEntity;
using TextEntityView = CMiX.Studio.Avalonia.Views.TextEntity;

namespace CMiX.Studio.Avalonia.Tests
{
    // The title bar of the inspector: its height, the kind color, the badge, the name and the toggles of the item.
    public class PanelInspectorHeaderTests
    {
        // The title bar has the height of the headers of the panels under it (the headers of the expanders).
        [AvaloniaFact]
        public void Show_TheHeader_HasTheHeightOfThePanelHeaders()
        {
            var screen = PanelInspectorScreen.Show();
            screen.Select(screen.CompositionNode);
            var expander = screen.Editor.GetVisualDescendants().OfType<ExpanderIndented>().First();
            var expanderHeader = expander.GetVisualDescendants().OfType<ToggleButton>().First();

            var height = Convert.ToDouble(Application.Current!.FindResource("ControlHeightXL"));
            Assert.Equal(height, screen.Header.Bounds.Height);
            Assert.Equal(expanderHeader.Bounds.Height, screen.Header.Bounds.Height);

            // The icon and the name are in the middle of the bar.
            var icon = screen.HeaderIcon;
            var iconMiddle = icon.TranslatePoint(new Point(0, icon.Bounds.Height / 2), screen.Header)!.Value.Y;
            Assert.Equal(height / 2, iconMiddle, 1);
        }

        // The whole title bar has the color of the kind of the selected row. The badge has no fill of its own.
        [AvaloniaFact]
        public void Select_AnItem_ColorsTheWholeHeaderWithTheKindColor()
        {
            var screen = PanelInspectorScreen.Show();
            var palette = CMiX.Studio.Avalonia.Themes.CategoryPalette.FromApplication();
            foreach (var node in new[] { screen.CompositionNode, screen.LayerNode, screen.Group<EntitiesGroup>(), screen.Group<LightsGroup>() })
            {
                screen.Select(node);
                var square = screen.Header.GetVisualDescendants().OfType<BadgeCategory>().Single().GetVisualDescendants().OfType<Border>().First();

                var expected = palette.For(node.Category!.Value, node.ComposableDepth, node.KindDepth).FillSelected;
                Assert.Equal(expected, ((global::Avalonia.Media.ISolidColorBrush)screen.Header.Background!).Color);
                Assert.Null(square.Background);
                Assert.Equal(node.IconKey, screen.HeaderIcon.IconKey);
            }
        }

        [AvaloniaFact]
        public void Show_TheBadgeOfTheHeader_IsARowHighSquareWithAWhiteIconInTheBar()
        {
            var screen = PanelInspectorScreen.Show();
            screen.Select(screen.CompositionNode);
            var square = screen.Header.GetVisualDescendants().OfType<BadgeCategory>().Single().GetVisualDescendants().OfType<Border>().First();

            var row = Convert.ToDouble(Application.Current!.FindResource("ControlHeightMD"));
            Assert.Equal(row, square.Bounds.Width);
            Assert.Equal(row, square.Bounds.Height);
            Assert.Equal((CornerRadius)Application.Current!.FindResource("CornerRadiusAll")!, square.CornerRadius);

            var top = square.TranslatePoint(new Point(0, 0), screen.Header)!.Value.Y;
            Assert.Equal(top, screen.Header.Bounds.Height - (top + square.Bounds.Height), 1);

            var white = ((global::Avalonia.Media.ISolidColorBrush)Application.Current!.FindResource("ActiveForegroundBrush")!).Color;
            Assert.Equal(white, ((global::Avalonia.Media.ISolidColorBrush)screen.HeaderIcon.Foreground!).Color);
        }

        [AvaloniaFact]
        public void Show_TheHeaderOfAnItem_HasTheEditableNameAndTheTogglesAtTheRight()
        {
            var screen = PanelInspectorScreen.Show();
            screen.Select(screen.LayerNode);

            var visibility = screen.VisibilityToggle!;
            var mask = screen.MaskToggle!;
            Assert.True(visibility.IsEffectivelyVisible);
            Assert.True(mask.IsEffectivelyVisible);
            Assert.True(screen.NameEditor.IsEffectivelyVisible);

            var headerWidth = screen.Header.Bounds.Width;
            // The mask is first and the visibility is on the far right.
            var visibilitySlot = (Control)visibility.Parent!;
            var visibilityRight = visibilitySlot.TranslatePoint(new Point(visibilitySlot.Bounds.Width, 0), screen.Header)!.Value.X;
            var maskLeft = mask.TranslatePoint(new Point(0, 0), screen.Header)!.Value.X;
            var visibilityLeft = visibility.TranslatePoint(new Point(0, 0), screen.Header)!.Value.X;
            var nameRight = screen.NameEditor.TranslatePoint(new Point(screen.NameEditor.Bounds.Width, 0), screen.Header)!.Value.X;
            Assert.Equal(headerWidth - 8, visibilityRight, 1);
            Assert.True(maskLeft < visibilityLeft);
            Assert.True(nameRight <= maskLeft + 0.5, "The name goes under the toggles.");
        }

        [AvaloniaFact]
        public void Show_TheToggles_HaveTheSlotsOfTheToggleColumnOfTheTree()
        {
            var screen = PanelInspectorScreen.Show();
            screen.Select(screen.LayerNode);

            var tree = screen.Window.GetVisualDescendants().OfType<ListTreeProject>().Single();
            var treeToggles = tree.Rows.RowOf(screen.LayerNode)!.Toggles;
            var treeSlots = treeToggles.GetVisualDescendants().OfType<Border>().Where(border => border.Classes.Contains("slot") && border.IsEffectivelyVisible).ToList();
            var treeButtons = treeToggles.GetVisualDescendants().OfType<ToggleButton>().Where(button => button.IsEffectivelyVisible).ToList();

            var slots = screen.Header.GetVisualDescendants().OfType<Border>().Where(border => border.Classes.Contains("slot")).ToList();
            var buttons = screen.Header.GetVisualDescendants().OfType<ToggleButton>().ToList();

            // The header has one slot and one button for each of the tree. The tree row is 24 high, the header keeps its own size.
            Assert.Equal(treeSlots.Count, slots.Count);
            Assert.Equal(treeButtons.Count, buttons.Count);
            Assert.Equal(treeButtons.Select(button => button.Name), buttons.Select(button => button.Name));
            Assert.Equal(2, slots.Count);
        }

        [AvaloniaFact]
        public void Edit_TheTogglesAndTheName_ChangeTheItem()
        {
            var screen = PanelInspectorScreen.Show();
            screen.Select(screen.LayerNode);
            var layer = (Layer)screen.LayerNode.Item!;

            var visible = layer.PrefabService.Visibility.Value;
            screen.VisibilityToggle!.IsChecked = !visible;
            Assert.Equal(!visible, layer.PrefabService.Visibility.Value);

            screen.MaskToggle!.IsChecked = true;
            Assert.True(layer.Mask.IsMask.Value);

            screen.NameEditor.Text = "Renamed";
            Assert.Equal("Renamed", layer.PrefabService.Name.Value);
        }

        [AvaloniaFact]
        public void Show_EachKindOfItem_GivesTheMaskToggleToACompositionAndALayerOnly()
        {
            var screen = PanelInspectorScreen.Show();
            var entity = screen.Group<EntitiesGroup>().Children[0];

            screen.Select(screen.CompositionNode);
            Assert.True(screen.MaskToggle!.IsEffectivelyVisible);

            screen.Select(entity);
            Assert.True(screen.VisibilityToggle!.IsEffectivelyVisible);
            Assert.True(screen.MaskToggle == null || !screen.MaskToggle.IsEffectivelyVisible);

            screen.Select(screen.Group<EntitiesGroup>());
            Assert.False(screen.VisibilityToggle!.IsEffectivelyVisible);
            Assert.False(screen.NameEditor.IsEffectivelyVisible);
            Assert.Equal("Entities", screen.Title.Text);
        }
    }
}

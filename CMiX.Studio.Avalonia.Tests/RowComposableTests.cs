// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Controls.String;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Color = Avalonia.Media.Color;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // A row of the layer and composition list has the same colors as every other row of a list.
    // A double click on the text of the name renames it. A double click on the rest of the row does nothing.
    // A right click on a row selects it and opens its menu, as in the outliner of Blender. The menu acts on the row that owns it.
    public class RowComposableTests : IDisposable
    {
        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

        private static Color Brush(string key) => ((ISolidColorBrush)Application.Current!.FindResource(key)!).Color;

        private ListBoxItem FirstRow()
        {
            var list = new ListBox
            {
                Theme = (ControlTheme)Application.Current!.FindResource("ListBoxComposable")!,
                ItemsSource = new[] { "One", "Two" },
                ItemTemplate = new FuncDataTemplate<string>((text, _) => new TextBlock { Text = text })
            };
            windows.Add(new Window { Content = list, Width = 300, Height = 300 }).Show();
            Dispatcher.UIThread.RunJobs();
            return list.GetVisualDescendants().OfType<ListBoxItem>().First();
        }

        private static Color? Background(ListBoxItem row) =>
            (row.GetVisualDescendants().OfType<Border>().First(border => border.Name == "Bd").Background as ISolidColorBrush)?.Color;

        private static void Pointer(ListBoxItem row, bool over)
        {
            ((IPseudoClasses)row.Classes).Set(":pointerover", over);
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void Pointer_OverARow_MakesItGrey()
        {
            var row = FirstRow();

            Pointer(row, true);

            Assert.Equal(Brush("RowHoverBrush"), Background(row));
        }

        [AvaloniaFact]
        public void Select_ARow_MakesItInactiveBlue()
        {
            var row = FirstRow();

            row.IsSelected = true;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(Brush("AccentInactiveBrush"), Background(row));
        }

        [AvaloniaFact]
        public void Pointer_OverASelectedRow_MakesItBlue()
        {
            var row = FirstRow();

            row.IsSelected = true;
            Pointer(row, true);

            Assert.Equal(Brush("StateOnHoverBrush"), Background(row));
        }

        private (Window Window, TextBoxEditable Name) ShowName()
        {
            var layer = TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(Layer));
            var row = new Views.RowComposable { DataContext = layer };
            var window = windows.Add(new Window { Content = row, Width = 400, Height = 60 });
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (window, row.GetVisualDescendants().OfType<TextBoxEditable>().First());
        }

        private static bool IsRenaming(TextBoxEditable name) => name.GetVisualDescendants().OfType<TextBox>().Any(input => input.IsVisible);

        private static void DoubleClick(Window window, Point point)
        {
            window.UpdateLayout();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            Dispatcher.UIThread.RunJobs();
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void DoubleClick_OnTheTextOfTheName_StartsTheRename()
        {
            var (window, name) = ShowName();
            var text = name.GetVisualDescendants().OfType<TextBlock>().First(block => block.Name == "textDisplay");

            DoubleClick(window, text.TranslatePoint(new Point(text.Bounds.Width / 2, text.Bounds.Height / 2), window)!.Value);

            Assert.True(IsRenaming(name));
        }

        [AvaloniaFact]
        public void DoubleClick_AwayFromTheText_DoesNothing()
        {
            var (window, name) = ShowName();

            DoubleClick(window, name.TranslatePoint(new Point(name.Bounds.Width - 12, name.Bounds.Height / 2), window)!.Value);

            Assert.False(IsRenaming(name));
        }

        private sealed class Screen
        {
            public Window Window { get; init; } = null!;
            public PrefabManager Layers { get; init; } = null!;
            public ListBox List { get; init; } = null!;
            public List<RowComposable> Rows { get; init; } = null!;
            public UndoManager Undo { get; init; } = null!;

            public int UndoSteps => UndoCounts.StepsOf(Undo);

            public bool AMenuIsOpen => Rows.Any(row => row.ContextMenu?.IsOpen == true);

            public void RightDown(int row)
            {
                var point = Window.CenterOf(Rows[row]);
                Window.MouseMove(point);
                Dispatcher.UIThread.RunJobs();
                Window.MouseDown(point, MouseButton.Right);
                Dispatcher.UIThread.RunJobs();
            }

            public void RightUp(int row)
            {
                Window.MouseUp(Window.CenterOf(Rows[row]), MouseButton.Right);
                Dispatcher.UIThread.RunJobs();
            }
        }

        private Screen ShowForRightClick()
        {
            var provider = TestServiceProviderFactory.Create();
            var compositions = provider.GetRequiredService<PrefabManager>();
            compositions.AddItem(typeof(Composition));
            var layers = ((Composition)compositions.SelectedItem).LayerManager;
            for (var i = 0; i < 3; i++)
                layers.AddItem(typeof(Layer));

            var view = new ListComposable { ItemType = typeof(Layer), DataContext = layers };
            var window = windows.Add(new Window { Content = view, Width = 400, Height = 300 });
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var undo = provider.GetRequiredService<UndoManager>();
            undo.Clear();
            return new Screen
            {
                Window = window,
                Layers = layers,
                List = view.GetVisualDescendants().OfType<ListBox>().First(),
                Rows = view.GetVisualDescendants().OfType<RowComposable>().ToList(),
                Undo = undo
            };
        }

        [AvaloniaFact]
        public void RightClick_AnUnselectedRow_SelectsItOnThePressAndOpensTheMenuOnTheRelease()
        {
            var screen = ShowForRightClick();
            Assert.Equal(2, screen.Layers.ManagerData.SelectedIndex);

            screen.RightDown(0);

            Assert.Equal(0, screen.Layers.ManagerData.SelectedIndex);
            Assert.Equal(0, screen.List.SelectedIndex);
            Assert.False(screen.AMenuIsOpen);

            screen.RightUp(0);

            Assert.True(screen.Rows[0].ContextMenu!.IsOpen);
            Assert.Equal(0, screen.Layers.ManagerData.SelectedIndex);
        }

        [AvaloniaFact]
        public void RightClick_AnUnselectedRow_IsOneUndoStepLikeALeftClick()
        {
            var screen = ShowForRightClick();

            screen.RightDown(0);
            screen.RightUp(0);

            Assert.Equal(1, screen.UndoSteps);
        }

        [AvaloniaFact]
        public void RightClick_TheSelectedRow_KeepsTheSelectionAndAddsNoUndoStep()
        {
            var screen = ShowForRightClick();

            screen.RightDown(2);
            screen.RightUp(2);

            Assert.Equal(2, screen.Layers.ManagerData.SelectedIndex);
            Assert.Equal(0, screen.UndoSteps);
            Assert.True(screen.Rows[2].ContextMenu!.IsOpen);
        }

        private (PrefabManager Manager, List<RowComposable> Rows, Window Window) ShowRows(int layerCount = 3)
        {
            var provider = TestServiceProviderFactory.Create();
            var compositions = provider.GetRequiredService<PrefabManager>();
            compositions.AddItem(typeof(Composition));
            var manager = ((Composition)compositions.SelectedItem).LayerManager;
            for (var i = 0; i < layerCount; i++)
                manager.AddItem(typeof(Layer));

            var view = new ListComposable { ItemType = typeof(Layer), DataContext = manager };
            var window = windows.Add(new Window { Content = view, Width = 500, Height = 800 });
            window.Show();
            Dispatcher.UIThread.RunJobs();

            return (manager, view.GetVisualDescendants().OfType<RowComposable>().ToList(), window);
        }

        private static MenuItem Row(RowComposable item, string header) =>
            item.ContextMenu!.Items.OfType<MenuItem>().Single(row => Equals(row.Header, header));

        // The right click of a user raises ContextRequested on the row.
        private static void OpenMenu(RowComposable item)
        {
            item.RaiseEvent(new ContextRequestedEventArgs { Source = item });
            Dispatcher.UIThread.RunJobs();
        }

        private static void Click(RowComposable item, string header)
        {
            OpenMenu(item);
            Row(item, header).RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void Menu_StartsWithTheNameOfTheItemAndALine_ThenListsTheRows()
        {
            var (manager, rows, _) = ShowRows(1);
            var name = ((Layer)manager.ManagerData.Items[0]).PrefabService.Name.Value;

            OpenMenu(rows[0]);
            var headers = rows[0].ContextMenu!.Items.OfType<MenuItem>().Select(row => row.Header).ToList();

            Assert.Equal(
                new object[] { name, "-", "Duplicate", "Rename", "-", "Move Up", "Move Down", "Move To Top", "Move To Bottom", "-", "Delete" },
                headers);
            Assert.False(Row(rows[0], name).IsEnabled);
        }

        [AvaloniaFact]
        public void Open_TheMenu_UsesTheSharedPopupTheme()
        {
            var (_, rows, _) = ShowRows(1);

            Assert.Same(Application.Current!.FindResource("ContextMenuDefault"), rows[0].ContextMenu!.Theme);
        }

        [AvaloniaFact]
        public void Rename_AnItem_ChangesTheTitleOfTheMenu()
        {
            var (manager, rows, _) = ShowRows(1);
            ((Layer)manager.ManagerData.Items[0]).PrefabService.Name.Value = "Hero";

            OpenMenu(rows[0]);

            Assert.Equal("Hero", rows[0].ContextMenu!.Items.OfType<MenuItem>().First().Header);
        }

        [AvaloniaFact]
        public void Open_TheMenu_PutsDuplicateThenTheRowUsedLastUnderThePointer()
        {
            var (_, rows, _) = ShowRows();
            OpenMenu(rows[0]);
            Assert.True(MenuAnchor.GetIsAnchor(Row(rows[0], "Duplicate")));

            Click(rows[0], "Move To Bottom");
            OpenMenu(rows[1]);

            Assert.True(MenuAnchor.GetIsAnchor(Row(rows[1], "Move To Bottom")));
            Assert.False(MenuAnchor.GetIsAnchor(Row(rows[1], "Duplicate")));
        }

        [AvaloniaFact]
        public void Duplicate_OnARowThatIsNotSelected_CopiesThatRow()
        {
            var (manager, rows, _) = ShowRows();
            var first = manager.ManagerData.Items[0];
            manager.SelectAt(2);

            Click(rows[0], "Duplicate");

            Assert.Equal(4, manager.ManagerData.Items.Count);
            Assert.Same(first, manager.ManagerData.Items[0]);
            Assert.NotEqual(first.ID, manager.ManagerData.Items[1].ID);
        }

        [AvaloniaFact]
        public void Remove_OnARow_RemovesThatRow()
        {
            var (manager, rows, _) = ShowRows();
            var items = manager.ManagerData.Items.ToList();

            Click(rows[1], "Delete");

            Assert.Equal(new[] { items[0], items[2] }, manager.ManagerData.Items);
        }

        [AvaloniaFact]
        public void Move_Rows_MoveTheItemAndStopAtTheEdges()
        {
            var (manager, rows, _) = ShowRows();
            var items = manager.ManagerData.Items.ToList();

            OpenMenu(rows[0]);
            Assert.False(Row(rows[0], "Move Up").IsEnabled);
            Assert.False(Row(rows[0], "Move To Top").IsEnabled);
            Assert.True(Row(rows[0], "Move Down").IsEnabled);
            rows[0].ContextMenu!.Close();

            OpenMenu(rows[2]);
            Assert.False(Row(rows[2], "Move Down").IsEnabled);
            Assert.False(Row(rows[2], "Move To Bottom").IsEnabled);
            rows[2].ContextMenu!.Close();

            Click(rows[0], "Move To Bottom");
            Assert.Equal(new[] { items[1], items[2], items[0] }, manager.ManagerData.Items);
        }

        [AvaloniaFact]
        public void Rename_FromTheMenu_StartsTheEditModeOfTheNameBox()
        {
            var (_, rows, _) = ShowRows(1);

            Click(rows[0], "Rename");

            Assert.True(rows[0].GetVisualDescendants().OfType<TextBoxEditable>().Single().IsEditing);
        }
    }
}

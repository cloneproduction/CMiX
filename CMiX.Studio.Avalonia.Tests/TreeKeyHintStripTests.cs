using System.IO;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Services;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls.Boolean;
using CMiX.UI.Controls.Parts;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // The hint strip under the tree: the keys of the mode of the tree (idle, search, menu), with the gestures of the shortcut map.
    // The setting InterfaceOptions.ShowKeyHints hides it. It takes no click and no focus.
    public class TreeKeyHintStripTests : IDisposable
    {
        private sealed record Screen(Window Window, TextBoxSearchTree Box, ListTreeProject View);

        private Window? window;
        private readonly TempFolder temp = new("interface", create: false);

        public void Dispose()
        {
            window?.Close();
            temp.Dispose();
        }

        // The search field over the tree, as in the bar of the panel. The width is the width of one side panel of the main window.
        private (Screen Screen, Scene Scene) Show()
        {
            var scene = CreateScene([3]);
            var (shown, box) = ListTreeTestScreen.ShowWithSearch(scene.Tree, PanelWidth, 500);
            window = shown.Window;
            return (new Screen(shown.Window, box, shown.View), scene);
        }

        // About one half of the main window (908 wide, two side panels).
        private const double PanelWidth = 440;

        private static void Pump(Screen screen)
        {
            Dispatcher.UIThread.RunJobs();
            ListTreeTestScreen.Settle(screen.Window);
        }

        private static string[] Words(Screen screen) => screen.View.HintStrip.Hints!.Select(hint => hint.Words).ToArray();

        private static string[] Keys(Screen screen) => screen.View.HintStrip.Hints!.SelectMany(hint => hint.Keys).ToArray();

        private static void FocusRow(Screen screen, CMiX.Studio.Avalonia.ViewModels.Tree.TreeNode node)
        {
            Assert.True(screen.View.Rows.RowOf(node)!.Focus(NavigationMethod.Tab));
            Pump(screen);
        }

        [AvaloniaFact]
        public void Show_TheTree_ShowsTheIdleHintsUnderTheRows()
        {
            var (screen, _) = Show();
            var strip = screen.View.HintStrip;

            Assert.Equal(TreeHintMode.Idle, screen.View.HintMode);
            Assert.Equal(new[] { "move", "select", "open", "menu", "add", "inspect", "undo" }, Words(screen));
            Assert.Equal(new[] { TreeKeyHints.Arrows, TreeKeyHints.Enter, "E", "W", "Shift+A", "Space", "Ctrl+Z" }, Keys(screen));
            Assert.True(strip.IsEffectivelyVisible);

            // The strip is at the bottom of the view, under the rows.
            var rowsArea = screen.View.Scroller;
            var stripTop = strip.TranslatePoint(default, screen.View)!.Value.Y;
            Assert.True(stripTop >= rowsArea.TranslatePoint(new Point(0, rowsArea.Bounds.Height), screen.View)!.Value.Y - 0.5);
            Assert.Equal(screen.View.Bounds.Height, stripTop + strip.Bounds.Height, 0.5);
        }

        [AvaloniaFact]
        public void Show_TheIdleHints_FitOneLineOfASidePanel()
        {
            var (screen, _) = Show();
            var strip = screen.View.HintStrip;

            Assert.True(strip.Child!.DesiredSize.Width + strip.Padding.Left + strip.Padding.Right <= PanelWidth,
                $"The idle hints are {strip.Child.DesiredSize.Width} wide.");
        }

        [AvaloniaFact]
        public void FocusSearch_TheFieldHasTheFocus_ShowsTheSearchHintsThenTheIdleHintsAgain()
        {
            var (screen, scene) = Show();

            screen.Box.Field.FocusInput();
            Pump(screen);

            Assert.Equal(TreeHintMode.Search, screen.View.HintMode);
            Assert.Equal(new[] { "next", "previous", "to tree", "clear" }, Words(screen));
            Assert.Equal(new[] { TreeKeyHints.Enter, TreeKeyHints.ShiftEnter, TreeKeyHints.Down, TreeKeyHints.Escape }, Keys(screen));

            FocusRow(screen, scene.NodeA);

            Assert.Equal(TreeHintMode.Idle, screen.View.HintMode);
            Assert.Equal("move", Words(screen)[0]);
        }

        [AvaloniaFact]
        public void OpenMenu_TheMenuOfARow_ShowsEscCloseThenTheIdleHintsAgain()
        {
            var (screen, scene) = Show();
            scene.Tree.SelectedNode = scene.NodeA;
            Pump(screen);
            FocusRow(screen, scene.NodeA);

            scene.Tree.OpenMenuAtTargetCommand.Execute(null);
            Pump(screen);

            var menu = screen.View.Menu!;
            Assert.True(menu.IsOpen);
            Assert.Equal(TreeHintMode.Menu, screen.View.HintMode);
            Assert.Equal(new[] { "close" }, Words(screen));
            Assert.Equal(new[] { TreeKeyHints.Escape }, Keys(screen));

            menu.Close();
            Pump(screen);

            Assert.Equal(new[] { "move", "select", "open", "menu", "add", "inspect", "undo" }, Words(screen));
        }

        [AvaloniaFact]
        public void OpenAddPanel_ByTheAddKey_ShowsEscCloseThenTheIdleHintsAgain()
        {
            var (screen, scene) = Show();
            FocusRow(screen, scene.NodeA);

            scene.Tree.AddAtTargetCommand.Execute(null);
            Pump(screen);

            Assert.True(screen.View.MenuAddPanel.IsOpen);
            Assert.Equal(new[] { "close" }, Words(screen));

            screen.View.MenuAddPanel.Close();
            Pump(screen);

            Assert.Equal("move", Words(screen)[0]);
        }

        [AvaloniaFact]
        public void Set_ARemappedUndoAndNoMenuKey_ShowsTheNewUndoAndNoMenuHint()
        {
            var (screen, scene) = Show();

            scene.Tree.Shortcuts.Set("Edit.Undo", new KeyGesture(Key.U, KeyModifiers.Control));
            scene.Tree.Shortcuts.Set("Tree.Menu", null);
            Pump(screen);

            Assert.Equal(new[] { "move", "select", "open", "add", "inspect", "undo" }, Words(screen));
            Assert.Equal("Ctrl+U", screen.View.HintStrip.Hints!.Single(hint => hint.Words == "undo").Keys.Single());
            var caps = screen.View.HintStrip.GetVisualDescendants().OfType<TextBlock>().Where(text => text.Name == "PART_KeyText");
            Assert.Contains(caps, cap => cap.Text == "Ctrl+U");
        }

        [AvaloniaFact]
        public void ShowKeyHints_Off_HidesTheStripAndOnShowsItAgain()
        {
            var (screen, scene) = Show();

            scene.Tree.Interface.ShowKeyHints = false;
            Pump(screen);

            // The rows take the room of the strip.
            Assert.False(screen.View.HintStrip.IsVisible);
            var rowsArea = screen.View.Scroller;
            Assert.Equal(screen.View.Bounds.Height, rowsArea.TranslatePoint(new Point(0, rowsArea.Bounds.Height), screen.View)!.Value.Y, 0.5);

            scene.Tree.Interface.ShowKeyHints = true;
            Pump(screen);

            Assert.True(screen.View.HintStrip.IsEffectivelyVisible);
        }

        [AvaloniaFact]
        public void Click_OnTheStrip_TakesNoFocusAndNoClick()
        {
            var (screen, scene) = Show();
            FocusRow(screen, scene.NodeA);
            var focused = screen.Window.FocusManager!.GetFocusedElement();
            var strip = screen.View.HintStrip;

            Assert.False(strip.IsHitTestVisible);
            Assert.All(strip.GetSelfAndVisualDescendants().OfType<InputElement>(), element => Assert.False(element.Focusable));

            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            var center = strip.TranslatePoint(new Point(strip.Bounds.Width / 2, strip.Bounds.Height / 2), screen.Window)!.Value;
            var hit = screen.Window.InputHitTest(center) as Visual;
            Assert.False(hit != null && (ReferenceEquals(hit, strip) || strip.IsVisualAncestorOf(hit)));
            screen.Window.MouseDown(center, MouseButton.Left);
            screen.Window.MouseUp(center, MouseButton.Left);
            Pump(screen);

            Assert.Same(focused, screen.Window.FocusManager!.GetFocusedElement());
        }

        [Fact]
        public void ShowKeyHints_Changed_IsSavedAndKeepsTheRedisSettings()
        {
            var path = temp.PathOf(StudioSettings.FileName);
            new StudioSettings { Redis = new RedisSettings { Ip = "10.0.0.2", Port = 6390 } }.Save(path);
            var options = new InterfaceOptions(path);
            options.Load();
            Assert.True(options.ShowKeyHints);

            options.ShowKeyHints = false;

            var saved = StudioSettings.Load(path);
            Assert.False(saved.Interface.ShowKeyHints);
            Assert.Equal("10.0.0.2", saved.Redis.Ip);
            Assert.Equal(6390, saved.Redis.Port);
            var loaded = new InterfaceOptions(path);
            loaded.Load();
            Assert.False(loaded.ShowKeyHints);
        }

        [Fact]
        public void Load_AFileWithNoInterface_ShowsTheKeyHints()
        {
            var path = temp.File(StudioSettings.FileName, JsonSerializer.Serialize(new { Redis = new { Ip = "127.0.0.1" } }));

            var options = new InterfaceOptions(path);
            options.Load();

            Assert.True(options.ShowKeyHints);
        }

        [AvaloniaFact]
        public void Show_TheTabSettings_HasTheKeyHintsToggleOnTheInterfaceOptions()
        {
            var (provider, mainWindow, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            window = mainWindow;
            TestServiceProviderFactory.SelectMainTab(mainWindow, "Settings");

            var tab = mainWindow.GetVisualDescendants().OfType<TabSettings>().Single();
            var toggle = tab.GetVisualDescendants().OfType<ToggleButtonCaptioned>().Single(t => t.Name == "showKeyHintsToggle");

            Assert.Same(provider.GetRequiredService<InterfaceOptions>(), viewModel.Interface);
            Assert.True(toggle.IsChecked);

            toggle.IsChecked = false;
            TestServiceProviderFactory.Pump();

            Assert.False(viewModel.Interface.ShowKeyHints);
            Assert.False(mainWindow.GetVisualDescendants().OfType<ListTreeProject>().First().HintStrip.IsVisible);
        }
    }
}

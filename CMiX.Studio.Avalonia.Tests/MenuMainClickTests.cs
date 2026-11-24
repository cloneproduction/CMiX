using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Studio.Avalonia.Tests.Support;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // A top item of the main menu opens its menu on every click: also after a click outside the menu closed it (see OutsideClick).
    public class MenuMainClickTests : IDisposable
    {
        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

        private static void Click(Window window, Control control) =>
            ClickAt(window, control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value);

        private static Popup PopupOf(MenuItem item) =>
            item.GetVisualDescendants().OfType<Popup>().First(popup => popup.Name == "PART_Popup");

        private (Window Window, MenuItem File) Show()
        {
            var (window, _) = ShowWithViewModel();
            return (window, MenuMainRows.TopItem(window, "File"));
        }

        private (Window Window, ViewModels.MainViewModel ViewModel) ShowWithViewModel()
        {
            var (_, window, viewModel) = windows.ShowMainWindow();
            TestServiceProviderFactory.Pump();
            return (window, viewModel);
        }

        // A row of an open menu is in the popup, its own top level.
        private static void ClickRow(MenuItem row)
        {
            var popupRoot = TopLevel.GetTopLevel(row)!;
            ClickAt(popupRoot, row.TranslatePoint(new Point(row.Bounds.Width / 2, row.Bounds.Height / 2), popupRoot)!.Value);
        }

        private static void ClickAt(TopLevel topLevel, Point point)
        {
            topLevel.MouseMove(point);
            topLevel.MouseDown(point, MouseButton.Left);
            topLevel.MouseUp(point, MouseButton.Left);
            TestServiceProviderFactory.Pump();
        }

        [AvaloniaFact]
        public void Click_TheFileItemThreeTimes_OpensClosesAndOpensItsMenu()
        {
            var (window, file) = Show();

            Click(window, file);
            Assert.True(PopupOf(file).IsOpen, "first click");

            Click(window, file);
            Assert.False(PopupOf(file).IsOpen, "second click");

            Click(window, file);
            Assert.True(PopupOf(file).IsOpen, "third click");

            file.Close();
        }

        // A click outside the menu closes it (OutsideClick). The next click on the item must open it again, every time.
        [AvaloniaFact]
        public void Click_TheFileItemAfterAClickOutside_OpensItsMenuAgain()
        {
            var (window, file) = Show();
            var outside = new Point(window.Bounds.Width / 2, window.Bounds.Height - 20);

            for (var round = 1; round <= 3; round++)
            {
                Click(window, file);
                Assert.True(file.IsSubMenuOpen, "open, round " + round);
                Assert.True(PopupOf(file).IsOpen, "popup open, round " + round);

                ClickAt(window, outside);
                Assert.False(PopupOf(file).IsOpen, "closed, round " + round);
                Assert.False(file.IsSubMenuOpen, "item closed, round " + round);
            }
        }

        [AvaloniaFact]
        public void Click_NewCompositionInTheCompoMenu_AddsTheCompositionAfterThePopupHasClosed()
        {
            var (window, viewModel) = ShowWithViewModel();
            var compositions = viewModel.Project.CompositionManager.ManagerData.Items;
            var before = compositions.Count;
            var compo = MenuMainRows.TopItem(window, "Compo");
            Click(window, compo);
            var row = compo.Items.OfType<MenuItem>().Single(item => (string?)item.Header == "New Composition");
            bool? popupOpenOnAdd = null;
            compositions.CollectionChanged += (_, _) => popupOpenOnAdd ??= PopupOf(compo).IsOpen;

            ClickRow(row);

            Assert.Equal(before + 1, compositions.Count);
            Assert.False(popupOpenOnAdd, "the popup was open when the composition was added");
            Assert.IsType<Composition>(viewModel.Project.CompositionManager.SelectedItem);
        }

        [AvaloniaFact]
        public void Enter_OnNewCompositionInTheCompoMenu_AddsTheCompositionAfterThePopupHasClosed()
        {
            var (window, viewModel) = ShowWithViewModel();
            var compositions = viewModel.Project.CompositionManager.ManagerData.Items;
            var before = compositions.Count;
            var compo = MenuMainRows.TopItem(window, "Compo");
            Click(window, compo);
            var row = compo.Items.OfType<MenuItem>().Single(item => (string?)item.Header == "New Composition");
            row.Focus();
            TestServiceProviderFactory.Pump();
            bool? popupOpenOnAdd = null;
            compositions.CollectionChanged += (_, _) => popupOpenOnAdd ??= PopupOf(compo).IsOpen;

            TopLevel.GetTopLevel(row)!.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);
            TestServiceProviderFactory.Pump();

            Assert.Equal(before + 1, compositions.Count);
            Assert.False(popupOpenOnAdd, "the popup was open when the composition was added");
        }
    }
}

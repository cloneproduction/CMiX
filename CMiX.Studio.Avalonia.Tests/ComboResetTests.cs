using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using CMiX.Studio.Avalonia.Views.Controls;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // A combo shows a Reset menu only when it has a reset command, and the menu restores the default.
    public class ComboResetTests
    {
        private static Window Host(Control content)
        {
            var window = new Window { Content = content, Width = 400, Height = 300 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return window;
        }

        private static CaptionedComboBox NewCombo() =>
            new() { Width = 200, Height = 30, ItemsSource = new[] { "A", "B" }, SelectedItem = "A" };

        [AvaloniaFact]
        public void Combo_WithoutResetCommand_HasNoMenu()
        {
            var combo = NewCombo();
            Host(combo);

            Assert.Null(combo.ContextMenu);
        }

        [AvaloniaFact]
        public void Combo_WithResetCommand_HasAResetMenuThatRunsIt()
        {
            var ran = false;
            var combo = NewCombo();
            combo.ResetCommand = new RelayCommand(() => ran = true);
            Host(combo);

            combo.ContextMenu!.Items.OfType<MenuItem>().Single().Command!.Execute(null);

            Assert.True(ran);
        }

        [AvaloniaFact]
        public void Combo_LosesTheMenu_WhenTheCommandIsRemoved()
        {
            var combo = NewCombo();
            combo.ResetCommand = new RelayCommand(() => { });
            Host(combo);
            Assert.NotNull(combo.ContextMenu);

            combo.ResetCommand = null;

            Assert.Null(combo.ContextMenu);
        }

        [AvaloniaFact]
        public void Combo_ContextRequestOnTheCombo_OpensTheResetMenu()
        {
            var combo = NewCombo();
            combo.ResetCommand = new RelayCommand(() => { });
            Host(combo);

            var inner = combo.GetVisualDescendants().OfType<ComboBox>().Single();
            inner.RaiseEvent(new global::Avalonia.Controls.ContextRequestedEventArgs { Source = inner });
            Dispatcher.UIThread.RunJobs();

            Assert.True(combo.ContextMenu!.IsOpen);
        }

        [AvaloniaFact]
        public void Combo_ContextRequestInsideTheDropDown_OpensNoMenu()
        {
            var combo = NewCombo();
            combo.ResetCommand = new RelayCommand(() => { });
            Host(combo);
            var inner = combo.GetVisualDescendants().OfType<ComboBox>().Single();
            inner.IsDropDownOpen = true;
            Dispatcher.UIThread.RunJobs();
            var popup = inner.GetVisualDescendants().OfType<global::Avalonia.Controls.Primitives.Popup>().First();
            var item = popup.Child!.GetVisualDescendants().OfType<ComboBoxItem>().First();

            item.RaiseEvent(new global::Avalonia.Controls.ContextRequestedEventArgs { Source = item });
            Dispatcher.UIThread.RunJobs();

            Assert.False(combo.ContextMenu!.IsOpen);
        }

        // The headless mouse does not reach the combo, so the test raises the pointer events on the toggle of the combo.
        private static void Click(Window window, Control target, MouseButton button)
        {
            var pointer = new Pointer(Pointer.GetNextFreeId(), PointerType.Mouse, true);
            var position = target.TranslatePoint(new Point(target.Bounds.Width / 2, target.Bounds.Height / 2), window)!.Value;
            var left = button == MouseButton.Left;

            target.RaiseEvent(new PointerPressedEventArgs(target, pointer, window, position, 1,
                new PointerPointProperties(left ? RawInputModifiers.LeftMouseButton : RawInputModifiers.RightMouseButton,
                    left ? PointerUpdateKind.LeftButtonPressed : PointerUpdateKind.RightButtonPressed), KeyModifiers.None));
            target.RaiseEvent(new PointerReleasedEventArgs(target, pointer, window, position, 2,
                new PointerPointProperties(RawInputModifiers.None,
                    left ? PointerUpdateKind.LeftButtonReleased : PointerUpdateKind.RightButtonReleased), KeyModifiers.None, button));
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void Combo_AfterARightClick_TheNextLeftClickKeepsTheDropDownOpen()
        {
            var combo = NewCombo();
            combo.ResetCommand = new RelayCommand(() => { });
            var window = Host(combo);
            var inner = combo.GetVisualDescendants().OfType<ComboBox>().Single();
            var toggle = inner.GetVisualDescendants().OfType<ToggleButton>().Single();

            Click(window, toggle, MouseButton.Right);
            Assert.True(combo.ContextMenu!.IsOpen);
            Assert.False(inner.IsDropDownOpen);
            combo.ContextMenu.Close();
            Click(window, toggle, MouseButton.Left);

            Assert.True(inner.IsDropDownOpen);
        }

        [AvaloniaFact]
        public void SetAlphaChannelCombo_ResetRestoresTheModelDefault()
        {
            var filter = (SetAlpha)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(SetAlpha));
            var view = new Views.SetAlpha { DataContext = filter };
            Host(view);
            var original = filter.AlphaChannel.Value;
            filter.AlphaChannel.Value = Enum.GetValues<AlphaChannel>().First(v => v != original);

            var combo = view.GetVisualDescendants().OfType<CaptionedComboBox>().Single();
            combo.ContextMenu!.Items.OfType<MenuItem>().Single().Command!.Execute(null);

            Assert.Equal(original, filter.AlphaChannel.Value);
        }
    }
}

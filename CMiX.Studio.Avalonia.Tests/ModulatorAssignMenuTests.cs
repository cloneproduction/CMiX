using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using CMiX.Studio.Avalonia.Views.Controls;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The assign popup of a real filter view: its rows, its assigned row, and what a click does.
    public class ModulatorAssignMenuTests
    {
        private static (Blur Blur, FFTModulator Fft, Window Window, ModulatorAssignButton Button) Show()
        {
            var provider = TestServiceProviderFactory.Create();
            var blur = (Blur)provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            blur.ModulatorManager.AddItem(typeof(RandomModulator));
            blur.ModulatorManager.AddItem(typeof(FFTModulator));

            var view = new Views.Blur { DataContext = blur };
            var window = new Window { Content = view, Width = 500, Height = 800 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var button = view.GetVisualDescendants().OfType<ModulatorAssignButton>().Single(b => b.DataContext == blur.Strength);
            return (blur, blur.ModulatorManager.ManagerData.Items.OfType<FFTModulator>().Single(), window, button);
        }

        private static FlyoutBase FlyoutOf(ModulatorAssignButton button) => button.FindControl<Button>("assignButton")!.Flyout!;

        private static List<MenuItem> Open(Window window, ModulatorAssignButton button)
        {
            FlyoutOf(button).ShowAt(button.FindControl<Button>("assignButton")!);
            Dispatcher.UIThread.RunJobs();
            return window.GetVisualDescendants().OfType<MenuFlyoutPresenter>().Single().GetVisualDescendants().OfType<MenuItem>().ToList();
        }

        private static MenuItem Row(List<MenuItem> rows, string header) => rows.Single(r => Equals(r.Header, header));

        [AvaloniaFact]
        public void AssignPopup_ListsTheTitleTheOutputsAndUnassign_InAPopupFrame()
        {
            var (_, _, window, button) = Show();

            var rows = Open(window, button);

            Assert.Equal(
                new object?[] { "Assign Modulator", "-", "Random Value", "FFT FFT", "FFT Bass", "FFT LowerMid", "FFT HigherMid", "FFT High", "Unassign" },
                rows.Select(r => r.Header).ToArray());
            Assert.All(rows, row => Assert.NotNull(row.FindAncestorOfType<PopupFrame>()));
        }

        [AvaloniaFact]
        public void AssignPopup_WithNothingBound_HasNoCheckedRow_AndADisabledUnassign()
        {
            var (_, _, window, button) = Show();

            var rows = Open(window, button);

            Assert.DoesNotContain(rows, r => r.Classes.Contains(":checked"));
            Assert.False(Row(rows, "Unassign").IsEffectivelyEnabled);
            Assert.False(Row(rows, "Assign Modulator").IsEffectivelyEnabled);
            Assert.True(Row(rows, "FFT Bass").IsEffectivelyEnabled);
        }

        [AvaloniaFact]
        public void AssignPopup_ClickOnAnOutput_BindsIt_AndClosesThePopup()
        {
            var (blur, fft, window, button) = Show();
            var rows = Open(window, button);

            PointerInput.Click(window, Row(rows, "FFT Bass"), MouseButton.Left);
            TestServiceProviderFactory.Pump();

            Assert.Same(fft, blur.Strength.BoundModulator);
            Assert.Equal("Bass", blur.Strength.BoundOutputName);
            Assert.False(FlyoutOf(button).IsOpen);
        }

        [AvaloniaFact]
        public void AssignPopup_OpenedAgain_MarksTheAssignedRow_AndUnassignClearsIt()
        {
            var (blur, _, window, button) = Show();
            PointerInput.Click(window, Row(Open(window, button), "FFT Bass"), MouseButton.Left);
            TestServiceProviderFactory.Pump();

            var rows = Open(window, button);

            Assert.True(Row(rows, "FFT Bass").Classes.Contains(":checked"));
            Assert.False(Row(rows, "Random Value").Classes.Contains(":checked"));
            Assert.True(Row(rows, "Unassign").IsEffectivelyEnabled);

            PointerInput.Click(window, Row(rows, "Unassign"), MouseButton.Left);
            TestServiceProviderFactory.Pump();

            Assert.Null(blur.Strength.BoundModulator);
            Assert.False(FlyoutOf(button).IsOpen);
        }

        [AvaloniaFact]
        public void AssignPopup_ShowsAModulatorThatWasAddedAfterTheFirstOpen()
        {
            var (blur, _, window, button) = Show();
            var before = Open(window, button).Count;
            FlyoutOf(button).Hide();
            Dispatcher.UIThread.RunJobs();

            blur.ModulatorManager.AddItem(typeof(RandomModulator));
            var rows = Open(window, button);

            Assert.Equal(before + 1, rows.Count);
        }
    }
}

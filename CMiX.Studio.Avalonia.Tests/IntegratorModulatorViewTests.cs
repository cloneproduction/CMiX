using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.UI.Controls.Boolean;
using CMiX.UI.Controls.Number;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ViewIntegratorModulator = CMiX.Studio.Avalonia.Views.IntegratorModulator;

namespace CMiX.Studio.Avalonia.Tests
{
    // The values and the Reset button of an IntegratorModulator in the window.
    public class IntegratorModulatorViewTests : IDisposable
    {
        private readonly IServiceProvider _provider = TestServiceProviderFactory.Create();

        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

        private IntegratorModulator NewModulator()
        {
            var blur = (Blur)_provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            blur.ModulatorManager.AddItem(typeof(IntegratorModulator));
            return (IntegratorModulator)blur.ModulatorManager.SelectedItem;
        }

        private ViewIntegratorModulator Show(IntegratorModulator modulator)
        {
            var view = new ViewIntegratorModulator { DataContext = modulator };
            windows.Add(new Window { Content = view, Width = 400, Height = 400 }).Show();
            Dispatcher.UIThread.RunJobs();
            return view;
        }

        private static List<DragValue> DragValues(Control view) =>
            view.GetVisualDescendants().OfType<DragValue>().ToList();

        private static ButtonCaptioned ResetButton(Control view) =>
            view.GetVisualDescendants().OfType<ButtonCaptioned>().Single();

        [AvaloniaFact]
        public void Show_AFreshModulator_ShowsTheDefaults()
        {
            var view = Show(NewModulator());

            var values = DragValues(view);
            Assert.Equal(2, values.Count);
            Assert.Equal(0.01f, (float)values[0].Value);
            Assert.Equal(0.0, values[1].Value);
            Assert.True(view.GetVisualDescendants().OfType<ToggleButtonCaptioned>().Single().IsChecked);
        }

        [AvaloniaFact]
        public void SetValue_OnTheOffsetDragValue_ChangesTheModulator()
        {
            var modulator = NewModulator();
            var view = Show(modulator);

            DragValues(view)[0].Value = 0.25;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(0.25f, modulator.Offset.Value);
        }

        [AvaloniaFact]
        public void Show_TheResetButton_HoldsTheClickCommand()
        {
            var modulator = NewModulator();
            var view = Show(modulator);

            Assert.Same(modulator.Reset.ClickCommand, ResetButton(view).Command);
        }

        [AvaloniaFact]
        public void Click_TheResetButton_RaisesTheClickEvent()
        {
            var modulator = NewModulator();
            var view = Show(modulator);
            var clicks = 0;
            modulator.Reset.Click += (source, args) => clicks++;

            ResetButton(view).Command!.Execute(null);

            Assert.Equal(1, clicks);
        }

        [AvaloniaFact]
        public void Show_TheResetButton_HasNoResetMenu()
        {
            var view = Show(NewModulator());

            Assert.Null(ResetButton(view).ContextMenu);
        }

        [AvaloniaFact]
        public void Show_TheResetButton_IsAsHighAsADragValueRow()
        {
            var view = Show(NewModulator());

            var button = ResetButton(view);
            Assert.True(button.Bounds.Height > 0);
            Assert.InRange(button.Bounds.Height, DragValues(view)[0].Bounds.Height - 1, DragValues(view)[0].Bounds.Height + 1);
        }
    }
}

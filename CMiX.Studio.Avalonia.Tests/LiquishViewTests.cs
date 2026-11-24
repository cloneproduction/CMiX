using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Sources;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Views.Modulation;
using CMiX.UI.Controls.ColorPicker;
using CMiX.UI.Controls.Number;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // Liquish view: the time row, the scale, three colors and the resolution.
    public class LiquishViewTests : IDisposable
    {
        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

        private (Liquish Source, Views.Liquish View) Show()
        {
            var provider = TestServiceProviderFactory.Create();
            var source = (Liquish)provider.GetRequiredService<ControlFactory>().Create(typeof(Liquish));

            var view = new Views.Liquish { DataContext = source };
            var window = windows.Add(new Window { Content = view, Width = 500, Height = 900 });
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (source, view);
        }

        private static DragValue TimeRow(Views.Liquish view, Liquish source) =>
            view.GetVisualDescendants().OfType<DragValue>().Single(d => d.DataContext == source.Time);

        [AvaloniaFact]
        public void Show_TimeRow_HasAModulatorAssignButtonAtItsRight()
        {
            var (source, view) = Show();
            var time = TimeRow(view, source);
            var mainBorder = time.GetVisualDescendants().OfType<Border>().First(b => b.Name == "mainBorder");

            var assign = time.GetVisualDescendants().OfType<ButtonModulatorAssign>().Single();

            Assert.Same(source.ModulatorManager, assign.ModulatorManager);
            Assert.Same(mainBorder.GetVisualParent()!.GetVisualParent(), assign.GetVisualParent()!.GetVisualParent());
            double Left(Control c) => c.TranslatePoint(new Point(0, 0), view)!.Value.X;
            Assert.True(Left(assign) >= Left(mainBorder) + mainBorder.Bounds.Width - 1);
        }

        [AvaloniaFact]
        public void Bind_TimeRow_IsReadOnlyWhileAModulatorIsBound()
        {
            var (source, view) = Show();
            var time = TimeRow(view, source);
            Assert.False(time.IsReadOnly);

            source.ModulatorManager.AddItem(typeof(RandomModulator));
            var modulator = (RandomModulator)source.ModulatorManager.ManagerData.Items[0];
            source.Time.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, modulator.Outputs[0]));
            TestServiceProviderFactory.Pump();

            Assert.True(time.IsReadOnly);
        }

        [AvaloniaFact]
        public void Show_ScaleRow_BindsXAndYOfTheScale()
        {
            var (source, view) = Show();

            var scale = view.GetVisualDescendants().OfType<Vector2Modulatable>().Single();

            Assert.Same(source.Scale, scale.DataContext);
            Assert.Contains(view.GetVisualDescendants().OfType<Control>(), c => c.DataContext == source.Scale.X);
            Assert.Contains(view.GetVisualDescendants().OfType<Control>(), c => c.DataContext == source.Scale.Y);
        }

        [AvaloniaFact]
        public void Show_View_HasThreeSelectorColorsAndTheResolution()
        {
            var (source, view) = Show();

            var colors = view.GetVisualDescendants().OfType<SelectorColor>().ToList();

            Assert.Equal(3, colors.Count);
            Assert.Equal(new object[] { source.ColorA, source.ColorB, source.ColorC }, colors.Select(c => c.DataContext));
            Assert.Single(view.GetVisualDescendants().OfType<Views.TextureResolution>());
        }
    }
}

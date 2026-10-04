using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using CMiX.Studio.Avalonia.Views.Controls;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // Hosts real filter views and checks that a bound modulator makes the value editor read only.
    public class ModulatedReadOnlyTests
    {
        private static (TModel Model, TView View) Show<TModel, TView>()
            where TModel : TextureFilterBase
            where TView : Control, new()
        {
            var provider = TestServiceProviderFactory.Create();
            var model = (TModel)provider.GetRequiredService<ControlFactory>().Create(typeof(TModel));

            var view = new TView { DataContext = model };
            var window = new Window { Content = view, Width = 500, Height = 800 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (model, view);
        }

        private static void Bind(TextureFilterBase filter, ModulatableValue<float> target)
        {
            filter.ModulatorManager.AddItem(typeof(RandomModulator));
            var modulator = (RandomModulator)filter.ModulatorManager.ManagerData.Items[0];
            target.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, modulator.Outputs[0]));
            TestServiceProviderFactory.Pump();
        }

        private static void Unbind(ModulatableValue<float> target)
        {
            target.SetModulatorCommand.Execute(null);
            TestServiceProviderFactory.Pump();
        }

        private static T Find<T>(Control view, object dataContext) where T : Control =>
            view.GetVisualDescendants().OfType<T>().Single(c => c.DataContext == dataContext);

        private static T TemplatePart<T>(TemplatedControl owner, string name) where T : Control =>
            owner.GetVisualDescendants().OfType<T>().Single(c => c.Name == name && c.TemplatedParent == owner);

        private static Button AssignButton(Control view, object dataContext) =>
            Find<ModulatorAssignButton>(view, dataContext).FindControl<Button>("assignButton")!;

        [AvaloniaFact]
        public void Slider_Unbound_IsEditable()
        {
            var (blur, view) = Show<Blur, Views.Blur>();
            var slider = Find<CMiXSlider>(view, blur.Strength);

            Assert.False(slider.IsReadOnly);
            Assert.True(TemplatePart<Border>(slider, "mainBorder").IsEnabled);
            Assert.True(TemplatePart<Border>(slider, "mainBorder").IsEffectivelyEnabled);
        }

        [AvaloniaFact]
        public void Slider_Bound_DisablesTheTrack()
        {
            var (blur, view) = Show<Blur, Views.Blur>();
            var slider = Find<CMiXSlider>(view, blur.Strength);

            Bind(blur, blur.Strength);

            Assert.True(slider.IsReadOnly);
            Assert.False(TemplatePart<Border>(slider, "mainBorder").IsEffectivelyEnabled);
        }

        [AvaloniaFact]
        public void Slider_Bound_KeepsTheAssignButtonEnabled()
        {
            var (blur, view) = Show<Blur, Views.Blur>();

            Bind(blur, blur.Strength);

            Assert.True(Find<ModulatorAssignButton>(view, blur.Strength).IsEffectivelyEnabled);
            Assert.True(AssignButton(view, blur.Strength).IsEffectivelyEnabled);
        }

        [AvaloniaFact]
        public void Slider_UnboundAfterBinding_EnablesTheTrackAgain()
        {
            var (blur, view) = Show<Blur, Views.Blur>();
            var slider = Find<CMiXSlider>(view, blur.Strength);
            Bind(blur, blur.Strength);

            Unbind(blur.Strength);

            Assert.False(slider.IsReadOnly);
            Assert.True(TemplatePart<Border>(slider, "mainBorder").IsEffectivelyEnabled);
        }

        [AvaloniaFact]
        public void FloatValue_IsReadOnlyFollowsTheModulatorBinding()
        {
            var (kaleidoscope, view) = Show<Kaleidoscope, Views.Kaleidoscope>();
            var dragValue = Find<DragValue>(view, kaleidoscope.Rotation);
            Assert.False(dragValue.IsReadOnly);

            Bind(kaleidoscope, kaleidoscope.Rotation);
            Assert.True(dragValue.IsReadOnly);
            Assert.False(TemplatePart<Border>(dragValue, "mainBorder").IsEffectivelyEnabled);

            Unbind(kaleidoscope.Rotation);
            Assert.False(dragValue.IsReadOnly);
            Assert.True(TemplatePart<Border>(dragValue, "mainBorder").IsEffectivelyEnabled);
        }

        [AvaloniaFact]
        public void FloatValue_Bound_KeepsTheAssignButtonEnabled()
        {
            var (kaleidoscope, view) = Show<Kaleidoscope, Views.Kaleidoscope>();

            Bind(kaleidoscope, kaleidoscope.Rotation);

            Assert.True(Find<ModulatorAssignButton>(view, kaleidoscope.Rotation).IsEffectivelyEnabled);
            Assert.True(AssignButton(view, kaleidoscope.Rotation).IsEffectivelyEnabled);
        }

        // Halftone sets Minimum="1" on Tile Count and Minimum="0" on the other three values.
        [AvaloniaFact]
        public void FloatValue_PassesItsMinimumToTheDragValue()
        {
            var (halftone, view) = Show<Halftone, Views.Halftone>();

            foreach (var bindable in halftone.Bindables)
            {
                var floatValue = Find<ModulatableFloatValue>(view, bindable);
                var dragValue = Find<DragValue>(floatValue, bindable);
                Assert.Equal(floatValue.Minimum, dragValue.Minimum);
                Assert.Equal(floatValue.Maximum, dragValue.Maximum);
            }

            Assert.Equal(1.0, Find<DragValue>(view, halftone.NumberOfTiles).Minimum);
        }
    }
}

using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class IsModulatedTests
    {
        private static (Blur blur, RandomModulator modulator) CreateBlurWithModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var blur = (Blur)provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            blur.ModulatorManager.AddItem(typeof(RandomModulator));
            var modulator = (RandomModulator)blur.ModulatorManager.ManagerData.Items[0];
            return (blur, modulator);
        }

        [Fact]
        public void IsModulated_IsFalseBeforeAnyBinding()
        {
            var (blur, _) = CreateBlurWithModulator();

            Assert.False(blur.Strength.IsModulated);
        }

        [Fact]
        public void IsModulated_IsTrueAfterBindingAModulatorOutput()
        {
            var (blur, modulator) = CreateBlurWithModulator();

            blur.Strength.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, modulator.Outputs[0]));

            Assert.True(blur.Strength.IsModulated);
        }

        [Fact]
        public void IsModulated_IsFalseAfterUnbinding()
        {
            var (blur, modulator) = CreateBlurWithModulator();
            blur.Strength.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, modulator.Outputs[0]));

            blur.Strength.SetModulatorCommand.Execute(null);

            Assert.False(blur.Strength.IsModulated);
        }

        [Fact]
        public void IsModulated_RaisesPropertyChangedOnBindAndUnbind()
        {
            var (blur, modulator) = CreateBlurWithModulator();
            var raised = 0;
            blur.Strength.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(IModulatorBindable.IsModulated)) raised++; };

            blur.Strength.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, modulator.Outputs[0]));
            Assert.True(raised > 0);

            raised = 0;
            blur.Strength.SetModulatorCommand.Execute(null);
            Assert.True(raised > 0);
        }
    }
}

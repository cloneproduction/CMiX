using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class KaleidoscopeTests
    {
        [Fact]
        public void ModulatorManager_CanAddRandomModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var kaleidoscope = provider.GetRequiredService<Kaleidoscope>();

            kaleidoscope.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Single(kaleidoscope.ModulatorManager.ManagerData.Items);
            Assert.IsType<RandomModulator>(kaleidoscope.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void ReorderingModulators_DoesNotUnassignTheMovedOnesBindings()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var kaleidoscope = (Kaleidoscope)factory.Create(typeof(Kaleidoscope));

            kaleidoscope.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod1 = (BeatRandomModulator)kaleidoscope.ModulatorManager.ManagerData.Items[0];
            kaleidoscope.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod2 = (BeatRandomModulator)kaleidoscope.ModulatorManager.ManagerData.Items[1];

            kaleidoscope.IterationZoom.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod1, mod1.Outputs[0]));
            kaleidoscope.Center.X.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod2, mod2.Outputs[0]));

            kaleidoscope.ModulatorManager.ManagerData.Items.Move(0, 1);

            Assert.Equal(mod1.ID, kaleidoscope.IterationZoom.ModulatorID);
            Assert.Same(mod1, kaleidoscope.IterationZoom.BoundModulator);
            Assert.Equal(mod2.ID, kaleidoscope.Center.X.ModulatorID);
            Assert.Same(mod2, kaleidoscope.Center.X.BoundModulator);
        }

        [Fact]
        public void DeletingAssignedModulator_UnassignsItFromANestedVectorBindable()
        {
            var provider = TestServiceProviderFactory.Create();
            var kaleidoscope = provider.GetRequiredService<Kaleidoscope>();
            kaleidoscope.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)kaleidoscope.ModulatorManager.ManagerData.Items[0];

            kaleidoscope.CellScale.Y.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            kaleidoscope.ModulatorManager.DeleteItem(randomModulator);

            Assert.Null(kaleidoscope.CellScale.Y.ModulatorID);
            Assert.Null(kaleidoscope.CellScale.Y.BoundModulator);
        }

        [Fact]
        public void Kaleidoscope_ToModel_FromModel_RoundTripsNestedVectorValueAndLiveBoundModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var kaleidoscope = provider.GetRequiredService<Kaleidoscope>();

            kaleidoscope.Center.X.Value = 4f;
            kaleidoscope.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)kaleidoscope.ModulatorManager.ManagerData.Items[0];
            kaleidoscope.Center.X.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            var model = kaleidoscope.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<Kaleidoscope>();
            reloaded.FromModel(model);

            Assert.Equal(4f, reloaded.Center.X.Value);
            var reloadedRandomModulator = Assert.IsType<RandomModulator>(reloaded.ModulatorManager.ManagerData.Items[0]);
            Assert.Same(reloadedRandomModulator, reloaded.Center.X.BoundModulator);
        }
    }
}

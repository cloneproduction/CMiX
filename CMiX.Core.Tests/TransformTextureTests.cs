using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class TransformTextureTests
    {
        [Fact]
        public void ModulatorManager_CanAddRandomModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var transformTexture = provider.GetRequiredService<TransformTexture>();

            transformTexture.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Single(transformTexture.ModulatorManager.ManagerData.Items);
            Assert.IsType<RandomModulator>(transformTexture.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void ReorderingModulators_DoesNotUnassignTheMovedOnesBindings()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var transformTexture = (TransformTexture)factory.Create(typeof(TransformTexture));

            transformTexture.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod1 = (BeatRandomModulator)transformTexture.ModulatorManager.ManagerData.Items[0];
            transformTexture.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod2 = (BeatRandomModulator)transformTexture.ModulatorManager.ManagerData.Items[1];

            transformTexture.Rotation.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod1, mod1.Outputs[0]));
            transformTexture.Location.X.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod2, mod2.Outputs[0]));

            transformTexture.ModulatorManager.ManagerData.Items.Move(0, 1);

            Assert.Equal(mod1.ID, transformTexture.Rotation.ModulatorID);
            Assert.Same(mod1, transformTexture.Rotation.BoundModulator);
            Assert.Equal(mod2.ID, transformTexture.Location.X.ModulatorID);
            Assert.Same(mod2, transformTexture.Location.X.BoundModulator);
        }

        [Fact]
        public void DeletingAssignedModulator_UnassignsItFromANestedVectorBindable()
        {
            var provider = TestServiceProviderFactory.Create();
            var transformTexture = provider.GetRequiredService<TransformTexture>();
            transformTexture.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)transformTexture.ModulatorManager.ManagerData.Items[0];

            transformTexture.Scale.Y.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            transformTexture.ModulatorManager.DeleteItem(randomModulator);

            Assert.Null(transformTexture.Scale.Y.ModulatorID);
            Assert.Null(transformTexture.Scale.Y.BoundModulator);
        }

        [Fact]
        public void TransformTexture_ToModel_FromModel_RoundTripsNestedVectorValueAndLiveBoundModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var transformTexture = provider.GetRequiredService<TransformTexture>();

            transformTexture.Location.X.Value = 4f;
            transformTexture.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)transformTexture.ModulatorManager.ManagerData.Items[0];
            transformTexture.Location.X.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            var model = transformTexture.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<TransformTexture>();
            reloaded.FromModel(model);

            Assert.Equal(4f, reloaded.Location.X.Value);
            var reloadedRandomModulator = Assert.IsType<RandomModulator>(reloaded.ModulatorManager.ManagerData.Items[0]);
            Assert.Same(reloadedRandomModulator, reloaded.Location.X.BoundModulator);
        }
    }
}

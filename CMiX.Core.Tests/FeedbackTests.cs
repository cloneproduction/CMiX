using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class FeedbackTests
    {
        [Fact]
        public void ModulatorManager_CanAddRandomModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var feedback = provider.GetRequiredService<Feedback>();

            feedback.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Single(feedback.ModulatorManager.ManagerData.Items);
            Assert.IsType<RandomModulator>(feedback.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void ReorderingModulators_DoesNotUnassignTheMovedOnesBindings()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var feedback = (Feedback)factory.Create(typeof(Feedback));

            feedback.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod1 = (BeatRandomModulator)feedback.ModulatorManager.ManagerData.Items[0];
            feedback.ModulatorManager.AddItem(typeof(BeatRandomModulator));

            feedback.Factor.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod1, mod1.Outputs[0]));

            feedback.ModulatorManager.ManagerData.Items.Move(0, 1);

            Assert.Equal(mod1.ID, feedback.Factor.ModulatorID);
            Assert.Same(mod1, feedback.Factor.BoundModulator);
        }

        [Fact]
        public void DeletingAssignedModulator_UnassignsItFromEveryBindableUsingIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var feedback = provider.GetRequiredService<Feedback>();
            feedback.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)feedback.ModulatorManager.ManagerData.Items[0];

            feedback.Factor.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            feedback.ModulatorManager.DeleteItem(randomModulator);

            Assert.Null(feedback.Factor.ModulatorID);
            Assert.Null(feedback.Factor.BoundModulator);
        }

        [Fact]
        public void Feedback_ToModel_FromModel_RoundTripsBindableValuesAndLiveBoundModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var feedback = provider.GetRequiredService<Feedback>();

            feedback.Factor.Value = 0.6f;
            feedback.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)feedback.ModulatorManager.ManagerData.Items[0];
            feedback.Factor.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            var model = feedback.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<Feedback>();
            reloaded.FromModel(model);

            Assert.Equal(0.6f, reloaded.Factor.Value);
            var reloadedRandomModulator = Assert.IsType<RandomModulator>(reloaded.ModulatorManager.ManagerData.Items[0]);
            Assert.Same(reloadedRandomModulator, reloaded.Factor.BoundModulator);
        }
    }
}

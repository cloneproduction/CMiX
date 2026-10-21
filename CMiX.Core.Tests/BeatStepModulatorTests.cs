using System.Text.Json;
using CMiX.Core.Animations;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // The list of a BeatStepModulator: the user adds, removes and saves its steps.
    public class BeatStepModulatorTests
    {
        private static BeatStepModulator NewModulator() =>
            (BeatStepModulator)TestServiceProviderFactory.Create()
                .GetRequiredService<ControlFactory>().Create(typeof(BeatStepModulator));

        private static IEnumerable<float> Values(BeatStepModulator modulator) =>
            modulator.StepManager.ManagerData.Items.Cast<BeatStep>().Select(step => step.Value);

        [Fact]
        public void ItStartsWithNoStep()
        {
            Assert.Empty(NewModulator().StepManager.ManagerData.Items);
        }

        [Fact]
        public void AddingSteps_ListsThemInOrder()
        {
            var modulator = NewModulator();

            modulator.StepManager.AddItem(typeof(BeatStep));
            ((BeatStep)modulator.StepManager.SelectedItem).Value = 0.25f;
            modulator.StepManager.AddItem(typeof(BeatStep));
            ((BeatStep)modulator.StepManager.SelectedItem).Value = 0.75f;

            Assert.Equal(new[] { 0.25f, 0.75f }, Values(modulator));
        }

        [Fact]
        public void DeletingAStep_TakesItOutOfTheList()
        {
            var modulator = NewModulator();
            modulator.StepManager.AddItem(typeof(BeatStep));
            modulator.StepManager.AddItem(typeof(BeatStep));
            var first = modulator.StepManager.ManagerData.Items[0];

            modulator.StepManager.RemoveItemCommand.Execute(first);

            Assert.Single(modulator.StepManager.ManagerData.Items);
            Assert.DoesNotContain(first, modulator.StepManager.ManagerData.Items);
        }

        [Fact]
        public void TheSteps_SurviveSavingAndLoading()
        {
            var modulator = NewModulator();
            foreach (var value in new[] { 0.1f, 0.5f, 0.9f })
            {
                modulator.StepManager.AddItem(typeof(BeatStep));
                ((BeatStep)modulator.StepManager.SelectedItem).Value = value;
            }

            var json = JsonSerializer.Serialize<IControlModel>(modulator.ToModel(), ProjectSerializer.Options);
            var model = (BeatStepModulatorModel)JsonSerializer.Deserialize<IControlModel>(json, ProjectSerializer.Options)!;

            Assert.Equal(new[] { 0.1f, 0.5f, 0.9f },
                model.StepManager.ManagerData.Items.Cast<BeatStepModel>().Select(step => step.Value));
        }
    }
}

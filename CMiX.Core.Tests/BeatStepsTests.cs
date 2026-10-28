using CMiX.Core.Animations;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class BeatStepsTests
    {
        private static BeatSteps CreateBeatSteps() =>
            ((BeatRandomModulator)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>()
                .Create(typeof(BeatRandomModulator))).BeatSteps;

        [Fact]
        public void ToModel_CalledTwice_GivesTheSameID()
        {
            var steps = CreateBeatSteps();

            Assert.Equal(((BeatStepsModel)steps.ToModel()).ID, ((BeatStepsModel)steps.ToModel()).ID);
        }

        [Fact]
        public void FromModel_KeepsTheIDOfTheModel()
        {
            var steps = CreateBeatSteps();
            var model = new BeatStepsModel();

            steps.FromModel(model);

            Assert.Equal(model.ID, steps.ID);
        }
    }
}

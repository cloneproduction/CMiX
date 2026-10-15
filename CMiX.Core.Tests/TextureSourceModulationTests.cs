using System.Linq;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class TextureSourceModulationTests
    {
        public static IEnumerable<object[]> SourceTypes => new[]
        {
            typeof(BubbleNoise), typeof(CheckerBoard), typeof(Gradient), typeof(Image),
            typeof(SequencePlayer), typeof(TouchBlob), typeof(TypeWriter), typeof(VideoIn),
            typeof(VideoPlayer),
        }.Select(type => new object[] { type });

        private static TextureSourceBase Create(Type type) =>
            (TextureSourceBase)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(type);

        [Theory]
        [MemberData(nameof(SourceTypes))]
        public void EverySource_HasAModulatorManager(Type type)
        {
            var source = Create(type);

            Assert.NotNull(source.ModulatorManager);
            Assert.Same(source.ModulatorManager, source.Modulators.Manager);
        }

        [Theory]
        [MemberData(nameof(SourceTypes))]
        public void EverySource_KeepsItsModulatorsThroughTheModel(Type type)
        {
            var source = Create(type);
            source.ModulatorManager.AddItem(typeof(LFOModulator));

            var copy = Create(type);
            copy.FromModel(source.ToModel());

            Assert.Single(copy.ModulatorManager.ManagerData.Items.OfType<LFOModulator>());
        }

        [Theory]
        [MemberData(nameof(SourceTypes))]
        public void EverySource_GivesItsCompositionIDToItsModulators(Type type)
        {
            var source = Create(type);
            var id = Guid.NewGuid();
            source.CompositionID = id;

            source.ModulatorManager.AddItem(typeof(LFOModulator));

            var modulator = (IHasCompositionID)source.ModulatorManager.SelectedItem;
            Assert.Equal(id, modulator.CompositionID);
        }

        [Fact]
        public void Host_UnbindsAValue_WhenItsModulatorIsRemoved()
        {
            var provider = TestServiceProviderFactory.Create();
            var blur = (Blur)provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            using var host = new ModulatorHost(provider.GetRequiredService<PrefabManager>());
            host.Add(blur.Strength);
            host.Manager.AddItem(typeof(RandomModulator));
            var modulator = (RandomModulator)host.Manager.ManagerData.Items[0];
            blur.Strength.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, modulator.Outputs[0]));
            Assert.Equal(modulator.ID, blur.Strength.ModulatorID);

            host.Manager.DeleteItem(modulator);

            Assert.Null(blur.Strength.ModulatorID);
        }

        [Fact]
        public void Host_KeepsTheBindingOfAMovedModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var blur = (Blur)provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            using var host = new ModulatorHost(provider.GetRequiredService<PrefabManager>());
            host.Add(blur.Strength);
            host.Manager.AddItem(typeof(RandomModulator));
            var modulator = (RandomModulator)host.Manager.ManagerData.Items[0];
            host.Manager.AddItem(typeof(RandomModulator));
            blur.Strength.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, modulator.Outputs[0]));

            host.Manager.ManagerData.Items.Move(0, 1);

            Assert.Equal(modulator.ID, blur.Strength.ModulatorID);
        }
    }
}

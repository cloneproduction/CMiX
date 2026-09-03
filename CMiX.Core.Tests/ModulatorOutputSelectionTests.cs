using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ModulatorOutputSelectionTests
    {
        private sealed class TestMultiOutputModulator : IModulator
        {
            public Guid ID { get; set; } = Guid.NewGuid();
            public PrefabService PrefabService { get; set; }
            public bool IsHovered { get; set; }
            public bool IsExpanded { get; set; }
            public IReadOnlyList<ModulatorOutput> Outputs { get; } = new[]
            {
                new ModulatorOutput("X", ModulatorKind.Set, typeof(float)),
                new ModulatorOutput("Y", ModulatorKind.Set, typeof(float))
            };

            public IControlModel ToModel() => throw new NotSupportedException();
            public void FromModel(IControlModel model) => throw new NotSupportedException();
        }

        [Fact]
        public void Modulatable_BoundToSpecificOutput_RoundTripsThroughToModelFromModel()
        {
            var provider = TestServiceProviderFactory.Create();
            var modulatable = provider.GetRequiredService<ModulatableFloat>();
            var modulator = new TestMultiOutputModulator();

            modulatable.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, modulator.Outputs[1]));

            Assert.Same(modulator, modulatable.BoundModulator);
            Assert.Equal(modulator.ID, modulatable.ModulatorID.Value);
            Assert.Equal("Y", modulatable.BoundOutputName.Value);

            var model = modulatable.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<ModulatableFloat>();
            reloaded.FromModel(model);

            Assert.Equal(modulator.ID, reloaded.ModulatorID.Value);
            Assert.Equal("Y", reloaded.BoundOutputName.Value);
        }

        [Fact]
        public void SetModulatorCommand_Null_ClearsBoundOutputNameAlongsideModulatorAndID()
        {
            var provider = TestServiceProviderFactory.Create();
            var modulatable = provider.GetRequiredService<ModulatableFloat>();
            var modulator = new TestMultiOutputModulator();

            modulatable.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, modulator.Outputs[0]));
            modulatable.SetModulatorCommand.Execute(null);

            Assert.Null(modulatable.BoundModulator);
            Assert.Null(modulatable.ModulatorID.Value);
            Assert.Null(modulatable.BoundOutputName.Value);
        }
    }
}

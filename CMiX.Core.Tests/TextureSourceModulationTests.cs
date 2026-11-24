using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class TextureSourceModulationTests
    {
        public static IEnumerable<object[]> SourceTypes => new[]
        {
            typeof(BubbleNoise), typeof(CheckerBoard), typeof(Gradient), typeof(GradientMesh),
            typeof(Image), typeof(Liquish), typeof(SequencePlayer), typeof(TouchBlob),
            typeof(TypeWriter), typeof(VideoIn), typeof(VideoPlayer),
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
        [InlineData(typeof(VideoPlayer), 3)]
        [InlineData(typeof(SequencePlayer), 4)]
        public void Players_ModulateTheirPlayLoopSeekAndDoSeekValues(Type type, int count)
        {
            var player = (TextureSourceBase)Create(type);

            Assert.Equal(count, player.Modulators.Bindables.Count);
            Assert.Contains(player.Modulators.Bindables, b => b is ModulatableValue<int>);
            Assert.Contains(player.Modulators.Bindables, b => b is ModulatableValue<bool>);
        }

        [Fact]
        public void Gradient_Create_ModulatesGamma()
        {
            var gradient = (Gradient)Create(typeof(Gradient));

            Assert.Same(gradient.Gamma, Assert.Single(gradient.Modulators.Bindables));
        }

        [Fact]
        public void BubbleNoise_Create_ModulatesTimeFrequencyAndContrast()
        {
            var bubbleNoise = (BubbleNoise)Create(typeof(BubbleNoise));

            Assert.Equal(3, bubbleNoise.Modulators.Bindables.Count);
            Assert.Contains(bubbleNoise.Modulators.Bindables, b => ReferenceEquals(b, bubbleNoise.Time));
            Assert.Contains(bubbleNoise.Modulators.Bindables, b => ReferenceEquals(b, bubbleNoise.Frequency));
            Assert.Contains(bubbleNoise.Modulators.Bindables, b => ReferenceEquals(b, bubbleNoise.Contrast));
        }

        [Fact]
        public void CheckerBoard_Create_ModulatesCellCountXAndY()
        {
            var checkerBoard = (CheckerBoard)Create(typeof(CheckerBoard));

            Assert.Equal(2, checkerBoard.Modulators.Bindables.Count);
            Assert.Contains(checkerBoard.Modulators.Bindables, b => ReferenceEquals(b, checkerBoard.CellCount.X));
            Assert.Contains(checkerBoard.Modulators.Bindables, b => ReferenceEquals(b, checkerBoard.CellCount.Y));
        }

        [Fact]
        public void VideoPlayer_KeepsTheDoSeekValueThroughTheModel()
        {
            var player = (VideoPlayer)Create(typeof(VideoPlayer));
            player.DoSeek.Value = true;

            var copy = (VideoPlayer)Create(typeof(VideoPlayer));
            copy.FromModel(player.ToModel());

            Assert.True(copy.DoSeek.Value);
        }

        [Fact]
        public void DoSeek_PressAndReleaseThenUndo_LeavesDoSeekFalse()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var player = (VideoPlayer)provider.GetRequiredService<ControlFactory>().Create(typeof(VideoPlayer));
            provider.GetRequiredService<ControlActivationService>().ActivateAll();

            Assert.True(player.DoSeek.IsMomentary);

            player.DoSeek.Value = true;
            player.DoSeek.Value = false;
            var error = Record.Exception(() => undoManager.Undo());

            Assert.Null(error);
            Assert.False(player.DoSeek.Value);
        }

        [Fact]
        public void SequencePlayer_KeepsTheDoSeekValueThroughTheModel()
        {
            var player = (SequencePlayer)Create(typeof(SequencePlayer));
            player.DoSeek.Value = true;

            var copy = (SequencePlayer)Create(typeof(SequencePlayer));
            copy.FromModel(player.ToModel());

            Assert.True(copy.DoSeek.Value);
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

            host.Manager.RemoveItem(modulator);

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

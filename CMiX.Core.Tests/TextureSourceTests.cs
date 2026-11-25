// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Text.Json;
using System.Text.Json.Nodes;
using CMiX.Core.Assets;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Every texture source has a modulator host and keeps its modulators. The values that each source modulates, and the players.
    public class TextureSourceTests
    {
        public static IEnumerable<object[]> SourceTypes => new[]
        {
            typeof(BubbleNoise), typeof(CheckerBoard), typeof(Gradient), typeof(GradientMesh),
            typeof(Image), typeof(Liquish), typeof(SequencePlayer), typeof(TouchBlob),
            typeof(TypeWriter), typeof(VideoIn), typeof(VideoPlayer),
        }.Select(type => new object[] { type });

        // Runs over every concrete texture source, so a new source cannot skip UseCompositionResolution.
        public static IEnumerable<object[]> SourceTypeNames() =>
            typeof(TextureSourceBase).Assembly.GetTypes()
                .Where(t => !t.IsAbstract && typeof(TextureSourceBase).IsAssignableFrom(t))
                .Select(t => new object[] { t.Name });

        private static Type SourceType(string name) =>
            typeof(TextureSourceBase).Assembly.GetTypes().Single(t => t.Name == name && typeof(TextureSourceBase).IsAssignableFrom(t));

        private static TextureSourceBase Create(Type type) =>
            (TextureSourceBase)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(type);

        private static T Create<T>() where T : TextureSourceBase => (T)Create(typeof(T));

        private static IControlModel ThroughJson(IControlModel model) =>
            JsonSerializer.Deserialize<IControlModel>(JsonSerializer.Serialize(model, ProjectSerializer.Options), ProjectSerializer.Options)!;

        [Theory]
        [MemberData(nameof(SourceTypes))]
        public void ModulatorManager_OfEverySource_IsTheManagerOfItsHost(Type type)
        {
            var source = Create(type);

            Assert.NotNull(source.ModulatorManager);
            Assert.Same(source.ModulatorManager, source.Modulators.Manager);
        }

        [Theory]
        [MemberData(nameof(SourceTypes))]
        public void FromModel_OfASourceWithAModulator_KeepsTheModulator(Type type)
        {
            var source = Create(type);
            source.ModulatorManager.AddItem(typeof(LFOModulator));

            var copy = Create(type);
            copy.FromModel(source.ToModel());

            Assert.Single(copy.ModulatorManager.ManagerData.Items.OfType<LFOModulator>());
        }

        [Theory]
        [MemberData(nameof(SourceTypes))]
        public void Deserialize_OfASerializedSource_KeepsEveryValue(Type type)
        {
            var source = Create(type);
            var before = ValueWalker.Collect(source);
            var n = 0;
            foreach (var value in before.Values)
                value.Change(n++);

            var loaded = Create(type);
            loaded.FromModel(ThroughJson(source.ToModel()));
            var after = ValueWalker.Collect(loaded);

            foreach (var (path, value) in before)
                Assert.True(Equals(value.Value, after[path].Value), $"{path} was {value.Value} before the save and {after[path].Value} after the load.");
        }

        [Theory]
        [MemberData(nameof(SourceTypeNames))]
        public void FromModel_OfASourceWithCompositionResolution_KeepsIt(string name)
        {
            var type = SourceType(name);
            var factory = TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>();

            var source = (TextureSourceBase)factory.Create(type);
            Assert.False(source.UseCompositionResolution.Value);
            source.UseCompositionResolution.Value = true;
            var model = source.ToModel();
            Assert.True(((ITextureSourceModel)model).UseCompositionResolution.Value);

            var loaded = (TextureSourceBase)factory.Create(type);
            loaded.FromModel(model);

            Assert.True(loaded.UseCompositionResolution.Value);
        }

        [Theory]
        [InlineData(typeof(VideoPlayer), 3)]
        [InlineData(typeof(SequencePlayer), 4)]
        public void Bindables_OfAPlayer_HoldTheIntAndBoolValues(Type type, int count)
        {
            var player = Create(type);

            Assert.Equal(count, player.Modulators.Bindables.Count);
            Assert.Contains(player.Modulators.Bindables, b => b is ModulatableValue<int>);
            Assert.Contains(player.Modulators.Bindables, b => b is ModulatableValue<bool>);
        }

        [Fact]
        public void Bindables_OfAGradient_AreTheGamma()
        {
            var gradient = Create<Gradient>();

            Assert.Same(gradient.Gamma, Assert.Single(gradient.Modulators.Bindables));
        }

        [Fact]
        public void Bindables_OfABubbleNoise_AreTimeFrequencyAndContrast()
        {
            var bubbleNoise = Create<BubbleNoise>();

            Assert.Equal(3, bubbleNoise.Modulators.Bindables.Count);
            Assert.Contains(bubbleNoise.Modulators.Bindables, b => ReferenceEquals(b, bubbleNoise.Time));
            Assert.Contains(bubbleNoise.Modulators.Bindables, b => ReferenceEquals(b, bubbleNoise.Frequency));
            Assert.Contains(bubbleNoise.Modulators.Bindables, b => ReferenceEquals(b, bubbleNoise.Contrast));
        }

        [Fact]
        public void Bindables_OfACheckerBoard_AreTheCellCountXAndY()
        {
            var checkerBoard = Create<CheckerBoard>();

            Assert.Equal(2, checkerBoard.Modulators.Bindables.Count);
            Assert.Contains(checkerBoard.Modulators.Bindables, b => ReferenceEquals(b, checkerBoard.CellCount.X));
            Assert.Contains(checkerBoard.Modulators.Bindables, b => ReferenceEquals(b, checkerBoard.CellCount.Y));
        }

        [Theory]
        [InlineData(typeof(GradientMesh))]
        [InlineData(typeof(Liquish))]
        public void RemoveItem_OfTheModulatorOfTime_UnbindsTime(Type type)
        {
            var source = Create(type);
            source.ModulatorManager.AddItem(typeof(LFOModulator));
            var modulator = (LFOModulator)source.ModulatorManager.ManagerData.Items[0];
            var time = (ModulatableValue<float>)type.GetProperty("Time")!.GetValue(source)!;
            time.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, modulator.Outputs[0]));
            Assert.Equal(modulator.ID, time.ModulatorID);

            source.ModulatorManager.RemoveItem(modulator);

            Assert.Null(time.ModulatorID);
        }

        [Fact]
        public void FromModel_OfALiquishWithABoundScale_KeepsTheBinding()
        {
            var liquish = Create<Liquish>();
            liquish.ModulatorManager.AddItem(typeof(LFOModulator));
            var modulator = (LFOModulator)liquish.ModulatorManager.ManagerData.Items[0];
            liquish.Scale.X.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, modulator.Outputs[0]));

            liquish.FromModel(liquish.ToModel());

            Assert.Equal(modulator.ID, liquish.Scale.X.ModulatorID);
        }

        [Theory]
        [InlineData(typeof(VideoPlayer))]
        [InlineData(typeof(SequencePlayer))]
        public void FromModel_OfAPlayerWithDoSeek_KeepsDoSeek(Type type)
        {
            var player = Create(type);
            var doSeek = (ModulatableValue<bool>)type.GetProperty("DoSeek")!.GetValue(player)!;
            doSeek.Value = true;

            var copy = Create(type);
            copy.FromModel(player.ToModel());

            Assert.True(((ModulatableValue<bool>)type.GetProperty("DoSeek")!.GetValue(copy)!).Value);
        }

        [Theory]
        [InlineData(typeof(VideoPlayer))]
        [InlineData(typeof(SequencePlayer))]
        public void Undo_AfterADoSeekPressAndRelease_LeavesDoSeekFalse(Type type)
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var player = provider.GetRequiredService<ControlFactory>().Create(type);
            provider.GetRequiredService<ControlRegistry>().ActivateAll();
            var doSeek = (ModulatableValue<bool>)type.GetProperty("DoSeek")!.GetValue(player)!;

            Assert.True(doSeek.IsMomentary);

            doSeek.Value = true;
            doSeek.Value = false;
            var error = Record.Exception(() => undoManager.Undo());

            Assert.Null(error);
            Assert.False(doSeek.Value);
        }

        [Fact]
        public void Create_OfASequencePlayer_GivesAMomentaryDoSeekAndAnAssetSource()
        {
            var player = Create<SequencePlayer>();

            Assert.True(player.DoSeek.IsMomentary);
            Assert.IsAssignableFrom<IAssetTextureSource>(player);
        }

        [Fact]
        public void FromModel_OfAFolderAsset_LoadsAnImageSequence()
        {
            using var temp = new TempDirectoryFixture();
            var factory = TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>();

            var player = (SequencePlayer)factory.Create(typeof(SequencePlayer));
            player.AssetSelector.SetAssetFromPath(temp.Path);
            var model = player.ToModel();

            var loaded = (SequencePlayer)factory.Create(typeof(SequencePlayer));
            loaded.FromModel(model);

            var sequence = Assert.IsType<ImageSequence>(loaded.AssetSelector.Asset);
            Assert.Equal(temp.Path, sequence.FilePath);
        }

        [Fact]
        public void FromModel_OfAnOldFileWithAButtonDoSeek_LoadsAFalseValueWithTheID()
        {
            var id = new Guid("5a1d3c2e-7b4f-4e8a-9c61-2f0d8e4b7a93");
            var node = JsonSerializer.SerializeToNode(new SequencePlayerModel(), ProjectSerializer.Options)!;
            node["DoSeek"] = new JsonObject { ["ID"] = id.ToString() };

            var loaded = JsonSerializer.Deserialize<SequencePlayerModel>(node, ProjectSerializer.Options)!;

            Assert.Equal(id, loaded.DoSeek.ID);
            Assert.False(loaded.DoSeek.Value.Value);
            Assert.Null(loaded.DoSeek.ModulatorID.Value);
        }

        [Fact]
        public void RemoveItem_OfAModulatorOfAHost_UnbindsTheValue()
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
        public void Move_OfAModulatorOfAHost_KeepsTheBinding()
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

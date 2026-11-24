// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Text.Json;
using CMiX.Core.Animations;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Every modulator type with its category and its outputs. A new modulator type needs a row, so its category is a decision.
    public class ModulatorTests
    {
        private static readonly (Type Type, ModulatorCategory Category, string[] Names, Type[] OutputTypes)[] Table =
        {
            (typeof(BeatLFOModulator), ModulatorCategory.Internal, new[] { "Value" }, new[] { typeof(ModulatorOutput<float>) }),
            (typeof(BeatRandomModulator), ModulatorCategory.Internal, new[] { "Value" }, new[] { typeof(ModulatorOutput<float>) }),
            (typeof(BeatStepModulator), ModulatorCategory.Internal, new[] { "Value", "Gate" }, new[] { typeof(ModulatorOutput<float>), typeof(ModulatorOutput<bool>) }),
            (
                typeof(FFTModulator), ModulatorCategory.Internal, new[] { "Bass", "LowerMid", "HigherMid", "High" },
                new[] { typeof(ModulatorOutput<float>), typeof(ModulatorOutput<float>), typeof(ModulatorOutput<float>), typeof(ModulatorOutput<float>) }
            ),
            (typeof(IntegratorModulator), ModulatorCategory.Internal, new[] { "Value" }, new[] { typeof(ModulatorOutput<float>) }),
            (
                typeof(LFOModulator), ModulatorCategory.Internal, new[] { "Phase", "Cycles", "Gate" },
                new[] { typeof(ModulatorOutput<float>), typeof(ModulatorOutput<int>), typeof(ModulatorOutput<bool>) }
            ),
            (
                typeof(LISAModulator), ModulatorCategory.External, new[] { "Count", "X", "Y" },
                new[] { typeof(ModulatorOutput<int>), typeof(ModulatorOutput<float>), typeof(ModulatorOutput<float>) }
            ),
            (typeof(RandomModulator), ModulatorCategory.Internal, new[] { "Value" }, new[] { typeof(ModulatorOutput<float>) }),
            (typeof(RedisBoolModulator), ModulatorCategory.External, new[] { "Value" }, new[] { typeof(AddressedOutput<bool>) }),
            (typeof(RedisFloatModulator), ModulatorCategory.External, new[] { "Value" }, new[] { typeof(AddressedOutput<float>) }),
            (typeof(RedisIntModulator), ModulatorCategory.External, new[] { "Value" }, new[] { typeof(AddressedOutput<int>) }),
            (
                typeof(TrackingModulator), ModulatorCategory.External, new[] { "Count", "X", "Y" },
                new[] { typeof(ModulatorOutput<int>), typeof(ModulatorOutput<float>), typeof(ModulatorOutput<float>) }
            ),
            (
                typeof(VisitorDataModulator), ModulatorCategory.External,
                new[] { "Count", "HeartRate", "Temperature", "Conductivity", "ETemp", "BlueIOTPosX", "BlueIOTPosY" },
                new[]
                {
                    typeof(ModulatorOutput<int>), typeof(ModulatorOutput<float>), typeof(ModulatorOutput<float>), typeof(ModulatorOutput<float>),
                    typeof(ModulatorOutput<float>), typeof(ModulatorOutput<float>), typeof(ModulatorOutput<float>)
                }
            ),
        };

        public static IEnumerable<object[]> Categories() =>
            Table.Select(row => new object[] { row.Type, row.Category });

        public static IEnumerable<object[]> Outputs() =>
            Table.Select(row => new object[] { row.Type, row.Names, row.OutputTypes });

        public static IEnumerable<object[]> ModulatorTypes() =>
            ConcreteModulatorTypes().Select(t => new object[] { t });

        private static IEnumerable<Type> ConcreteModulatorTypes() =>
            typeof(IModulator).Assembly.GetTypes()
                .Where(type => !type.IsAbstract && type.IsClass && typeof(IModulator).IsAssignableFrom(type))
                .OrderBy(type => type.Name, StringComparer.Ordinal);

        private static ControlFactory CreateFactory() =>
            TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>();

        private static T Create<T>() where T : IModulator => (T)CreateFactory().Create(typeof(T));

        private static IControlModel ThroughJson(IControlModel model) =>
            JsonSerializer.Deserialize<IControlModel>(JsonSerializer.Serialize(model, ProjectSerializer.Options), ProjectSerializer.Options)!;

        [Fact]
        public void Table_OfTheModulators_HoldsEveryConcreteModulatorType()
        {
            var inTable = Table.Select(row => row.Type).ToHashSet();
            var types = ConcreteModulatorTypes().ToList();

            Assert.True(types.Count >= 13, $"Only {types.Count} modulator types were found.");
            Assert.Empty(types.Where(type => !inTable.Contains(type)).Select(type => type.Name));
        }

        [Theory]
        [MemberData(nameof(Categories))]
        public void Category_OfAModulatorType_IsTheCategoryOfTheTable(Type type, ModulatorCategory category)
        {
            Assert.Equal(category, ModulatorCategoryAttribute.Of(type));
            Assert.Equal(category, ((IModulator)CreateFactory().Create(type)).Category);
        }

        [Theory]
        [MemberData(nameof(Outputs))]
        public void Outputs_OfANewModulator_AreTheNamedOutputsOfTheTable(Type type, string[] names, Type[] outputTypes)
        {
            var modulator = (IModulator)CreateFactory().Create(type);

            Assert.Equal(names, modulator.Outputs.Select(o => o.Name));
            for (var i = 0; i < outputTypes.Length; i++)
                Assert.IsType(outputTypes[i], modulator.Outputs[i]);
        }

        [Theory]
        [MemberData(nameof(ModulatorTypes))]
        public void Deserialize_OfASerializedModulator_KeepsEveryValue(Type type)
        {
            var modulator = CreateFactory().Create(type);
            var before = ValueWalker.Collect(modulator);
            var n = 0;
            foreach (var value in before.Values)
                value.Change(n++);

            var loaded = CreateFactory().Create(ThroughJson(modulator.ToModel()));
            var after = ValueWalker.Collect(loaded);

            Assert.Equal(modulator.ID, loaded.ID);
            foreach (var (path, value) in before.Where(entry => !ControlRoundTripTests.UnsavedPaths.Contains(entry.Key)))
                Assert.True(Equals(value.Value, after[path].Value), $"{path} was {value.Value} before the save and {after[path].Value} after the load.");
        }

        [Theory]
        [InlineData(typeof(LISAModulator), "X")]
        [InlineData(typeof(IntegratorModulator), "Value")]
        public void SetModulatorCommand_OfAnOutputOnAFilterValue_ModulatesTheValue(Type type, string outputName)
        {
            var blur = (Blur)CreateFactory().Create(typeof(Blur));
            blur.ModulatorManager.AddItem(type);
            var modulator = (IModulator)blur.ModulatorManager.SelectedItem;

            blur.Strength.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, modulator.Outputs.Single(o => o.Name == outputName)));

            Assert.True(blur.Strength.IsModulated);
        }

        [Fact]
        public void ClickCommand_OfTheIntegratorReset_RaisesTheClickEvent()
        {
            var modulator = Create<IntegratorModulator>();
            var count = 0;
            modulator.Reset.Click += (s, e) => count++;

            modulator.Reset.ClickCommand.Execute(null);

            Assert.Equal(1, count);
        }

        [Fact]
        public void Deserialize_OfAnIntegratorModel_KeepsTheIDOfTheResetButton()
        {
            var source = (IntegratorModulatorModel)Create<IntegratorModulator>().ToModel();

            var model = Assert.IsType<IntegratorModulatorModel>(ThroughJson(source));

            Assert.Equal(source.Reset.ID, model.Reset.ID);
        }

        [Fact]
        public void Deserialize_OfABeatStepModulatorWithSteps_KeepsTheStepsInOrder()
        {
            var modulator = Create<BeatStepModulator>();
            foreach (var value in new[] { 0.1f, 0.5f, 0.9f })
            {
                modulator.StepManager.AddItem(typeof(BeatStep));
                ((BeatStep)modulator.StepManager.SelectedItem).Value = value;
            }

            var model = (BeatStepModulatorModel)ThroughJson(modulator.ToModel());

            Assert.Equal(new[] { 0.1f, 0.5f, 0.9f },
                model.StepManager.ManagerData.Items.Cast<BeatStepModel>().Select(step => step.Value));
        }

        [Fact]
        public void ToModel_OfBeatStepsCalledTwice_GivesTheSameID()
        {
            var steps = Create<BeatRandomModulator>().BeatSteps;

            Assert.Equal(((BeatStepsModel)steps.ToModel()).ID, ((BeatStepsModel)steps.ToModel()).ID);
        }

        [Fact]
        public void FromModel_OfBeatSteps_KeepsTheIDOfTheModel()
        {
            var steps = Create<BeatRandomModulator>().BeatSteps;
            var model = new BeatStepsModel();

            steps.FromModel(model);

            Assert.Equal(model.ID, steps.ID);
        }
    }
}

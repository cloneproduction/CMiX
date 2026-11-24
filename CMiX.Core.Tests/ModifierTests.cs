// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Reflection;
using System.Text.Json;
using CMiX.Core.Colors.Modifiers;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Text.Modifiers;
using CMiX.Core.Transformation;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Every modifier that a panel offers. A new modifier is found by reflection and needs a row in the owner table.
    public class ModifierTests
    {
        public static IEnumerable<object[]> AddableModifierTypes() =>
            AddableTypes().Select(t => new object[] { t });

        public static IEnumerable<object[]> ModulatedModifierTypes() =>
            ModulatedTypes().Select(t => new object[] { t });

        public static IEnumerable<object[]> ModifierTypesWithTwoValues() =>
            ModulatedTypes().Where(t => Create(t).Bindables.Count >= 2).Select(t => new object[] { t });

        public static IEnumerable<object[]> ModifierTypesWithAValue() =>
            ModulatedTypes().Where(t => Create(t).Bindables.Count >= 1).Select(t => new object[] { t });

        public static IEnumerable<object[]> ModifierTypesWithAModeSelector() =>
            AddableTypes()
                .Where(t => t.GetProperty(nameof(ScaleModifier.ModifierModeSelector))?.PropertyType == typeof(ModifierModeSelector))
                .Select(t => new object[] { t });

        public static TheoryData<Type, Type[]> Owners => new TheoryData<Type, Type[]>
        {
            { typeof(AlphaModifier), new[] { typeof(Layer), typeof(Composition) } },
            { typeof(BillboardModifier), new[] { typeof(Entity) } },
            { typeof(CharWriterModifier), new[] { typeof(TextEntity) } },
            { typeof(CircularSpreadModifier), new[] { typeof(Entity), typeof(LightEntity) } },
            { typeof(ColorModifier), new[] { typeof(Entity), typeof(LightEntity) } },
            { typeof(ColorPaletteModifier), new[] { typeof(Entity), typeof(LightEntity) } },
            { typeof(ComposableVisibilityModifier), new[] { typeof(Layer), typeof(Composition) } },
            { typeof(FlipModifier), new[] { typeof(Entity) } },
            { typeof(GridModifier), new[] { typeof(Entity), typeof(LightEntity) } },
            { typeof(LinearModifier), new[] { typeof(Entity) } },
            { typeof(OrbitModifier), new[] { typeof(Camera) } },
            { typeof(RandomModifier), new[] { typeof(Entity) } },
            { typeof(RenderRandomEntityModifier), new[] { typeof(Layer) } },
            { typeof(RenderSequenceEntityModifier), new[] { typeof(Layer) } },
            { typeof(RotationModifier), new[] { typeof(Entity), typeof(LightEntity) } },
            { typeof(ScaleModifier), new[] { typeof(Entity) } },
            { typeof(SplitModifier), new[] { typeof(TextEntity) } },
            { typeof(TargetModifier), new[] { typeof(Camera) } },
            { typeof(TexCoordModifier), new[] { typeof(Entity) } },
            { typeof(TransformSRTModifier), new[] { typeof(Entity) } },
            { typeof(TranslateModifier), new[] { typeof(Entity), typeof(LightEntity) } },
            { typeof(VisibilityModifier), new[] { typeof(Entity), typeof(LightEntity) } },
            { typeof(ZoomModifier), new[] { typeof(Camera) } },
        };

        public static TheoryData<Type, string[]> ValuePaths => new TheoryData<Type, string[]>
        {
            { typeof(AlphaModifier), new[] { "Value" } },
            { typeof(CharWriterModifier), new string[0] },
            { typeof(CircularSpreadModifier), new[] { "X", "Y", "Phase", "Factor" } },
            { typeof(ColorModifier), new[] { "Hue", "Saturation", "Value", "Alpha" } },
            { typeof(FlipModifier), new string[0] },
            { typeof(GridModifier), new[] { "Width.X", "Width.Y", "Width.Z", "Phase.X", "Phase.Y", "Phase.Z" } },
            { typeof(LinearModifier), new[] { "Width", "Phase" } },
            { typeof(RenderRandomEntityModifier), new[] { "Control" } },
            { typeof(RenderSequenceEntityModifier), new[] { "Control" } },
            { typeof(RotationModifier), new[] { "X", "Y", "Z" } },
            { typeof(ScaleModifier), new[] { "X", "Y", "Z", "Uniform" } },
            { typeof(TexCoordModifier), new[] { "Location.X", "Location.Y", "Scale.X", "Scale.Y", "Rotation", "Uniform" } },
            { typeof(TranslateModifier), new[] { "X", "Y", "Z" } },
        };

        public static TheoryData<Type, string[]> ValueLabels => new TheoryData<Type, string[]>
        {
            { typeof(CircularSpreadModifier), new[] { "X", "Y", "Phase", "Factor" } },
            { typeof(ColorModifier), new[] { "Hue", "Saturation", "Value", "Alpha" } },
            { typeof(LinearModifier), new[] { "Width", "Phase" } },
            { typeof(RenderRandomEntityModifier), new[] { "Control" } },
            { typeof(RenderSequenceEntityModifier), new[] { "Control" } },
            { typeof(RotationModifier), new[] { "X", "Y", "Z" } },
            { typeof(ScaleModifier), new[] { "X", "Y", "Z", "Uniform" } },
            { typeof(TranslateModifier), new[] { "X", "Y", "Z" } },
        };

        public static TheoryData<Type, string, object> Defaults => new TheoryData<Type, string, object>
        {
            { typeof(AlphaModifier), "Value", 1f },
            { typeof(ColorModifier), "Hue", 0f },
            { typeof(ColorModifier), "Saturation", 0f },
            { typeof(ColorModifier), "Value", 1f },
            { typeof(ColorModifier), "Alpha", 1f },
            { typeof(ComposableVisibilityModifier), "Visible", true },
            { typeof(GridModifier), "Count.X", 1 },
            { typeof(GridModifier), "Count.Y", 1 },
            { typeof(GridModifier), "Count.Z", 1 },
            { typeof(ScaleModifier), "X", 1f },
            { typeof(ScaleModifier), "Y", 1f },
            { typeof(ScaleModifier), "Z", 1f },
            { typeof(ScaleModifier), "Uniform", 1f },
            { typeof(VisibilityModifier), "Visible", true },
        };

        public static TheoryData<Type> VisibilityModifierTypes => new TheoryData<Type>
        {
            typeof(VisibilityModifier),
            typeof(ComposableVisibilityModifier),
        };

        private static IEnumerable<Type> AddableTypes() =>
            typeof(ModifierPanelAttribute).Assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Where(t => t.GetCustomAttributes(typeof(ModifierPanelAttribute), false).Length > 0)
                .OrderBy(t => t.Name, StringComparer.Ordinal);

        // The modifiers that derive from Modifier hold a modulator list and bind their values to it.
        private static IEnumerable<Type> ModulatedTypes() =>
            AddableTypes().Where(t => typeof(Modifier).IsAssignableFrom(t));

        private static Modifier Create(Type type) =>
            (Modifier)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(type);

        private static T Create<T>() where T : Modifier => (T)Create(typeof(T));

        private static T AddModulator<T>(Modifier modifier) where T : IModulator
        {
            modifier.ModulatorManager.AddItem(typeof(T));
            return (T)modifier.ModulatorManager.ManagerData.Items[^1];
        }

        private static void Bind(IModulatorBindable value, IModulator modulator) =>
            value.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, modulator.Outputs[0]));

        private static object Resolve(object root, string path) =>
            path.Split('.').Aggregate(root, (o, name) => o.GetType().GetProperty(name)!.GetValue(o));

        private static ModulatableValue<bool> VisibleOf(Modifier modifier) =>
            (ModulatableValue<bool>)Resolve(modifier, "Visible");

        [Fact]
        public void AddableModifierTypes_OfTheCoreAssembly_HoldEveryModifier()
        {
            var types = AddableTypes().ToList();

            Assert.True(types.Count >= 23, $"Only {types.Count} modifier types were found.");
            Assert.Contains(typeof(ScaleModifier), types);
            Assert.Contains(typeof(OrbitModifier), types);
        }

        [Fact]
        public void Owners_OfTheTable_HoldEveryAddableModifier()
        {
            var inTable = Owners.Select(row => (Type)row[0]).ToHashSet();

            Assert.All(AddableTypes(), type => Assert.Contains(type, inTable));
        }

        [Theory]
        [MemberData(nameof(Owners))]
        public void ModifierPanel_OfAModifier_NamesItsOwners(Type type, Type[] owners)
        {
            var actual = type.GetCustomAttributes<ModifierPanelAttribute>().Select(a => a.PanelOwner).OrderBy(t => t.Name);

            Assert.Equal(owners.OrderBy(t => t.Name), actual);
        }

        [Theory]
        [MemberData(nameof(AddableModifierTypes))]
        public void Create_OfAnAddableModifier_GivesTheModifier(Type type)
        {
            var control = TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(type);

            Assert.IsType(type, control);
        }

        [Theory]
        [MemberData(nameof(ModifierTypesWithAModeSelector))]
        public void Create_OfAModifierWithAModeSelector_SetsTheCountToOne(Type type)
        {
            var selector = (ModifierModeSelector)Resolve(Create(type), nameof(ScaleModifier.ModifierModeSelector));

            Assert.Equal(1, selector.Count.Value);
        }

        [Theory]
        [MemberData(nameof(Defaults))]
        public void Create_OfAModifier_GivesTheModelDefault(Type type, string path, object expected)
        {
            var value = Resolve(Resolve(Create(type), path), "Value");

            Assert.Equal(expected, value);
        }

        [Theory]
        [MemberData(nameof(ValuePaths))]
        public void Bindables_OfAModifier_AreTheNamedValuesInOrder(Type type, string[] paths)
        {
            var modifier = Create(type);

            Assert.Equal(paths.Length, modifier.Bindables.Count);
            for (var i = 0; i < paths.Length; i++)
                Assert.Same(modifier.Bindables[i], Resolve(modifier, paths[i]));
        }

        [Theory]
        [MemberData(nameof(ValueLabels))]
        public void Bindables_OfAModifier_CarryTheLabelsInOrder(Type type, string[] labels)
        {
            Assert.Equal(labels, Create(type).Bindables.Select(b => b.Label));
        }

        [Theory]
        [MemberData(nameof(ModifierTypesWithTwoValues))]
        public void SetModulatorCommand_OnTheFirstAndTheLastValue_BindsOnlyThoseTwo(Type type)
        {
            var modifier = Create(type);
            var modulator = AddModulator<RandomModulator>(modifier);
            var last = modifier.Bindables.Count - 1;

            Bind(modifier.Bindables[0], modulator);
            Bind(modifier.Bindables[last], modulator);

            for (var i = 0; i <= last; i++)
            {
                if (i == 0 || i == last)
                    Assert.Equal(modulator.ID, modifier.Bindables[i].ModulatorID);
                else
                    Assert.Null(modifier.Bindables[i].ModulatorID);
            }
        }

        [Theory]
        [MemberData(nameof(ModulatedModifierTypes))]
        public void FromModel_OfABoundModifier_BindsEveryValueToTheLoadedModulator(Type type)
        {
            var modifier = Create(type);
            var modulator = AddModulator<RandomModulator>(modifier);
            foreach (var value in modifier.Bindables)
                Bind(value, modulator);

            var loaded = Create(type);
            loaded.FromModel(modifier.ToModel());

            var loadedModulator = Assert.IsType<RandomModulator>(Assert.Single(loaded.ModulatorManager.ManagerData.Items));
            Assert.Equal(modifier.Bindables.Count, loaded.Bindables.Count);
            Assert.All(loaded.Bindables, value => Assert.Same(loadedModulator, value.BoundModulator));
        }

        [Theory]
        [MemberData(nameof(ModifierTypesWithAValue))]
        public void RemoveItem_OfABoundModulator_UnbindsEveryValue(Type type)
        {
            var modifier = Create(type);
            var modulator = AddModulator<RandomModulator>(modifier);
            foreach (var value in modifier.Bindables)
                Bind(value, modulator);

            modifier.ModulatorManager.RemoveItem(modulator);

            Assert.All(modifier.Bindables, value =>
            {
                Assert.Null(value.ModulatorID);
                Assert.Null(value.BoundModulator);
            });
        }

        [Theory]
        [MemberData(nameof(ModifierTypesWithTwoValues))]
        public void Move_OfTwoBoundModulators_KeepsEveryBinding(Type type)
        {
            var modifier = Create(type);
            var first = AddModulator<BeatRandomModulator>(modifier);
            var second = AddModulator<BeatRandomModulator>(modifier);
            for (var i = 0; i < modifier.Bindables.Count; i++)
                Bind(modifier.Bindables[i], i % 2 == 0 ? first : second);

            modifier.ModulatorManager.ManagerData.Items.Move(0, 1);

            for (var i = 0; i < modifier.Bindables.Count; i++)
            {
                var expected = i % 2 == 0 ? first : second;
                Assert.Equal(expected.ID, modifier.Bindables[i].ModulatorID);
                Assert.Same(expected, modifier.Bindables[i].BoundModulator);
            }
        }

        [Fact]
        public void FromModel_OfAGridWithTheFirstCountAxisBound_BindsOnlyThatAxis()
        {
            var grid = Create<GridModifier>();
            var modulator = AddModulator<RandomModulator>(grid);
            Bind(grid.Count.X, modulator);

            var loaded = Create<GridModifier>();
            loaded.FromModel(grid.ToModel());

            var loadedModulator = Assert.IsType<RandomModulator>(loaded.ModulatorManager.ManagerData.Items[0]);
            Assert.Same(loadedModulator, loaded.Count.X.BoundModulator);
            Assert.Null(loaded.Count.Y.BoundModulator);
            Assert.Null(loaded.Count.Z.BoundModulator);
        }

        [Fact]
        public void FromModel_OfAGridWithTheLastCountAxisBound_KeepsTheOutputName()
        {
            var grid = Create<GridModifier>();
            var lfo = AddModulator<LFOModulator>(grid);
            grid.Count.Z.SetModulatorCommand.Execute(new ModulatorOutputSelection(lfo, lfo.Outputs.First(o => o.Name == "Cycles")));

            var loaded = Create<GridModifier>();
            loaded.FromModel(grid.ToModel());

            Assert.Null(loaded.Count.X.BoundModulator);
            Assert.NotNull(loaded.Count.Z.BoundModulator);
            Assert.Equal(lfo.ID, loaded.Count.Z.ModulatorID);
            Assert.Equal("Cycles", loaded.Count.Z.BoundOutputName);
        }

        [Fact]
        public void RemoveItem_OfAModulatorOnTwoCountAxes_UnbindsBoth()
        {
            var grid = Create<GridModifier>();
            var modulator = AddModulator<RandomModulator>(grid);
            Bind(grid.Count.X, modulator);
            Bind(grid.Count.Z, modulator);

            grid.ModulatorManager.RemoveItem(modulator);

            Assert.Null(grid.Count.X.ModulatorID);
            Assert.Null(grid.Count.X.BoundModulator);
            Assert.Null(grid.Count.Z.ModulatorID);
            Assert.Null(grid.Count.Z.BoundModulator);
            Assert.Null(grid.Count.Y.ModulatorID);
        }

        [Fact]
        public void RemoveItem_OfAModulatorOnTheModeCount_UnbindsTheCount()
        {
            var scale = Create<ScaleModifier>();
            var modulator = AddModulator<RandomModulator>(scale);
            Bind(scale.ModifierModeSelector.Count, modulator);

            scale.ModulatorManager.RemoveItem(modulator);

            Assert.Null(scale.ModifierModeSelector.Count.ModulatorID);
            Assert.Null(scale.ModifierModeSelector.Count.BoundModulator);
        }

        [Fact]
        public void FromModel_OfABoundModeCount_BindsTheLoadedModulator()
        {
            var scale = Create<ScaleModifier>();
            var modulator = AddModulator<RandomModulator>(scale);
            Bind(scale.ModifierModeSelector.Count, modulator);

            var loaded = Create<ScaleModifier>();
            loaded.FromModel(scale.ToModel());

            var loadedModulator = Assert.IsType<RandomModulator>(loaded.ModulatorManager.ManagerData.Items[0]);
            Assert.Same(loadedModulator, loaded.ModifierModeSelector.Count.BoundModulator);
        }

        [Fact]
        public void Create_OfAModelWithABinding_BindsTheValueToTheLoadedModulator()
        {
            var factory = TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>();
            var scale = (ScaleModifier)factory.Create(typeof(ScaleModifier));
            var modulator = AddModulator<RandomModulator>(scale);
            Bind(scale.X, modulator);

            var loaded = (ScaleModifier)factory.Create(scale.ToModel());

            Assert.Equal(modulator.ID, loaded.X.ModulatorID);
            Assert.NotNull(loaded.X.BoundModulator);
            Assert.Equal(modulator.ID, loaded.X.BoundModulator.ID);
        }

        [Fact]
        public void Dispose_OfAModifier_KeepsTheModulatorListUsable()
        {
            var scale = Create<ScaleModifier>();
            Assert.IsAssignableFrom<IDisposable>(scale);

            scale.Dispose();
            scale.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Single(scale.ModulatorManager.ManagerData.Items);
        }

        [Fact]
        public void Undo_AfterSetModulator_ClearsTheIDTheNameAndTheModulatorInOneStep()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var scale = (ScaleModifier)provider.GetRequiredService<ControlFactory>().Create(typeof(ScaleModifier));
            var modulator = AddModulator<RandomModulator>(scale);

            Bind(scale.X, modulator);

            Assert.Equal(modulator.ID, scale.X.ModulatorID);
            Assert.Equal("Value", scale.X.BoundOutputName);
            Assert.NotNull(scale.X.BoundModulator);

            undoManager.Undo();

            Assert.Null(scale.X.ModulatorID);
            Assert.Null(scale.X.BoundOutputName);
            Assert.Null(scale.X.BoundModulator);
        }

        [Theory]
        [MemberData(nameof(VisibilityModifierTypes))]
        public void FromModel_OfAVisibilityModifier_KeepsTheValueAndTheGateBinding(Type type)
        {
            var modifier = Create(type);
            var lfo = AddModulator<LFOModulator>(modifier);
            VisibleOf(modifier).Value = false;
            VisibleOf(modifier).SetModulatorCommand.Execute(new ModulatorOutputSelection(lfo, lfo.Outputs.Single(o => o is ModulatorOutput<bool>)));

            var copy = Create(type);
            copy.FromModel(modifier.ToModel());

            Assert.False(VisibleOf(copy).Value);
            Assert.Equal(lfo.ID, VisibleOf(copy).ModulatorID);
            Assert.Equal("Gate", VisibleOf(copy).BoundOutputName);
            Assert.NotNull(VisibleOf(copy).BoundModulator);
        }

        [Theory]
        [MemberData(nameof(VisibilityModifierTypes))]
        public void RemoveItem_OfTheGateModulator_UnbindsVisible(Type type)
        {
            var modifier = Create(type);
            var lfo = AddModulator<LFOModulator>(modifier);
            VisibleOf(modifier).SetModulatorCommand.Execute(new ModulatorOutputSelection(lfo, lfo.Outputs.Single(o => o is ModulatorOutput<bool>)));

            modifier.ModulatorManager.RemoveItem(lfo);

            Assert.Null(VisibleOf(modifier).ModulatorID);
        }

        // Before the toggle, the modifier saved a float named Value. A project file with that key must open.
        [Fact]
        public void FromModel_OfAnOldFloatVisibilityFile_LeavesTheModifierVisible()
        {
            const string json = "{\"$type\":\"CMiX.Core.Modulation.Modifiers.VisibilityModifierModel\",\"Value\":{\"Value\":{\"Value\":0.25}}}";
            var model = JsonSerializer.Deserialize<IControlModel>(json, ProjectSerializer.Options)!;

            var modifier = Create<VisibilityModifier>();
            modifier.FromModel(model);

            Assert.True(modifier.Visible.Value);
            Assert.Null(modifier.Visible.ModulatorID);
        }

        [Fact]
        public void LabelOf_TheComposableVisibilityModifier_IsVisibility()
        {
            Assert.Equal("Visibility", ControlFactory.StringHelper.LabelOf(typeof(ComposableVisibilityModifier)));
        }

        [Fact]
        public void Create_OfAComposableVisibilityModifier_NamesItVisibilityModifier()
        {
            Assert.Equal("Visibility Modifier", Create<ComposableVisibilityModifier>().PrefabService.Name.Value);
        }

        [Fact]
        public void IComposable_AsAnIModifiable_IsAssignable()
        {
            Assert.True(typeof(IModifiable).IsAssignableFrom(typeof(IComposable)));
        }

        [Theory]
        [InlineData(typeof(Composition), typeof(AlphaModifier))]
        [InlineData(typeof(Composition), typeof(ComposableVisibilityModifier))]
        [InlineData(typeof(Layer), typeof(ComposableVisibilityModifier))]
        public void AddItemCommand_OfAComposableModifier_PutsItInTheList(Type owner, Type modifierType)
        {
            var compositionManager = TestServiceProviderFactory.Create().GetRequiredService<PrefabManager>();
            compositionManager.AddItem(typeof(Composition));
            var composition = (Composition)compositionManager.SelectedItem;
            composition.LayerManager.AddItem(typeof(Layer));
            var modifiers = owner == typeof(Layer) ? ((Layer)composition.LayerManager.SelectedItem).ModifierManager : composition.ModifierManager;

            modifiers.AddItemCommand.Execute(modifierType);

            Assert.Single(modifiers.ManagerData.Items, item => item.GetType() == modifierType);
        }
    }
}

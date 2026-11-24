// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // A ModulatableValue of every value type binds to a modulator output, keeps the binding in its model and blocks Reset while bound.
    public class ModulatableValueTests
    {
        public static TheoryData<Type> ValueTypes => new TheoryData<Type> { typeof(float), typeof(int), typeof(bool) };

        public static TheoryData<Type, object> SavedValues => new TheoryData<Type, object>
        {
            { typeof(float), 2.5f },
            { typeof(int), 5 },
            { typeof(bool), true },
        };

        // The value type, a default and a changed value.
        public static TheoryData<Type, object, object> ChangedValues => new TheoryData<Type, object, object>
        {
            { typeof(float), 1.5f, 4f },
            { typeof(int), 3, 9 },
            { typeof(bool), true, false },
        };

        private sealed class TestMultiOutputModulator : IModulator
        {
            public Guid ID { get; set; } = Guid.NewGuid();
            public PrefabService PrefabService { get; set; }
            public Guid CompositionID { get; set; }
            public bool IsHovered { get; set; }
            public bool IsExpanded { get; set; }
            public IReadOnlyList<IModulatorOutput> Outputs { get; } = new IModulatorOutput[]
            {
                new ModulatorOutput<float>("X"),
                new ModulatorOutput<float>("Y")
            };

            public IControlModel ToModel() => throw new NotSupportedException();
            public void FromModel(IControlModel model) => throw new NotSupportedException();
        }

        private static dynamic Create(IServiceProvider provider, Type valueType) =>
            provider.GetRequiredService(typeof(ModulatableValue<>).MakeGenericType(valueType));

        // A value with a random modulator that its lookup finds.
        private static (dynamic Value, IModulatorBindable Bindable, RandomModulator Modulator) ValueWithAModulator(Type valueType)
        {
            var provider = TestServiceProviderFactory.Create();
            var value = Create(provider, valueType);
            var modulator = provider.GetRequiredService<RandomModulator>();
            var bindable = (IModulatorBindable)value;
            bindable.ModulatorLookup = id => id == modulator.ID ? modulator : null;
            return (value, bindable, modulator);
        }

        private static IModulatorOutput OutputOf(Type outputType) =>
            (IModulatorOutput)Activator.CreateInstance(typeof(ModulatorOutput<>).MakeGenericType(outputType), "Output")!;

        [Theory]
        [MemberData(nameof(ValueTypes))]
        public void Create_OfAModulatableValue_IsUnbound(Type valueType)
        {
            IModulatorBindable value = Create(TestServiceProviderFactory.Create(), valueType);

            Assert.Null(value.ModulatorID);
            Assert.Null(value.BoundModulator);
            Assert.False(value.IsModulated);
        }

        [Theory]
        [MemberData(nameof(ValueTypes))]
        public void SetModulatorCommand_AnOutputThenNull_SetsThenClearsTheBinding(Type valueType)
        {
            var (_, bindable, modulator) = ValueWithAModulator(valueType);

            bindable.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, modulator.Outputs[0]));

            Assert.Equal(modulator.ID, bindable.ModulatorID);
            Assert.Same(modulator, bindable.BoundModulator);
            Assert.Equal("Value", bindable.BoundOutputName);
            Assert.True(bindable.IsModulated);

            bindable.SetModulatorCommand.Execute(null);

            Assert.Null(bindable.ModulatorID);
            Assert.Null(bindable.BoundModulator);
            Assert.Null(bindable.BoundOutputName);
            Assert.False(bindable.IsModulated);
        }

        [Theory]
        [MemberData(nameof(ValueTypes))]
        public void SetModulatorCommand_AnOutputThenNull_RaisesIsModulatedBothTimes(Type valueType)
        {
            var (value, bindable, modulator) = ValueWithAModulator(valueType);
            var raised = 0;
            ((System.ComponentModel.INotifyPropertyChanged)value).PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(IModulatorBindable.IsModulated)) raised++;
            };

            bindable.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, modulator.Outputs[0]));
            Assert.True(raised > 0);

            raised = 0;
            bindable.SetModulatorCommand.Execute(null);
            Assert.True(raised > 0);
        }

        [Theory]
        [MemberData(nameof(SavedValues))]
        public void FromModel_OfToModel_KeepsTheLabelTheValueAndTheBinding(Type valueType, object saved)
        {
            var provider = TestServiceProviderFactory.Create();
            var value = Create(provider, valueType);
            var id = Guid.NewGuid();
            value.Label = "X";
            value.Value = (dynamic)saved;
            value.ModulatorID = id;
            value.BoundOutputName = "Gate";

            var copy = Create(TestServiceProviderFactory.Create(), valueType);
            copy.FromModel(value.ToModel());

            Assert.Equal("X", (string)copy.Label);
            Assert.Equal(saved, (object)copy.Value);
            Assert.Equal(id, (Guid?)copy.ModulatorID);
            Assert.Equal("Gate", (string)copy.BoundOutputName);
        }

        [Theory]
        [MemberData(nameof(ChangedValues))]
        public void ResetCommand_WhileModulated_IsDisabledAndResetsOnceUnbound(Type valueType, object defaultValue, object changed)
        {
            var (value, bindable, modulator) = ValueWithAModulator(valueType);
            value.Value = (dynamic)defaultValue;
            value.ValueSource.SetDefaults();
            value.Value = (dynamic)changed;
            System.Windows.Input.ICommand reset = value.ResetCommand;
            Assert.True(reset.CanExecute(null));

            bindable.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, modulator.Outputs[0]));

            Assert.False(reset.CanExecute(null));

            bindable.SetModulatorCommand.Execute(null);

            Assert.True(reset.CanExecute(null));

            reset.Execute(null);

            Assert.Equal(defaultValue, (object)value.Value);
        }

        [Theory]
        [InlineData(typeof(bool), typeof(bool), true)]
        [InlineData(typeof(bool), typeof(float), false)]
        [InlineData(typeof(float), typeof(bool), false)]
        [InlineData(typeof(float), typeof(float), true)]
        public void CanBind_AnOutputType_IsTrueOnlyForTheValueType(Type valueType, Type outputType, bool expected)
        {
            IModulatorBindable value = Create(TestServiceProviderFactory.Create(), valueType);

            Assert.Equal(expected, value.CanBind(OutputOf(outputType)));
        }

        [Fact]
        public void SetModulatorCommand_OfTheSecondOutput_KeepsItsNameThroughTheModel()
        {
            var provider = TestServiceProviderFactory.Create();
            var value = provider.GetRequiredService<ModulatableValue<float>>();
            var modulator = new TestMultiOutputModulator();
            value.ModulatorLookup = id => id == modulator.ID ? modulator : null;

            value.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, modulator.Outputs[1]));

            Assert.Same(modulator, value.BoundModulator);
            Assert.Equal(modulator.ID, value.ModulatorID);
            Assert.Equal("Y", value.BoundOutputName);

            var copy = TestServiceProviderFactory.Create().GetRequiredService<ModulatableValue<float>>();
            copy.FromModel(value.ToModel());

            Assert.Equal(modulator.ID, copy.ModulatorID);
            Assert.Equal("Y", copy.BoundOutputName);
        }

        [Fact]
        public void SetModulatorCommand_NullWithoutALookup_ClearsTheNameTheModulatorAndTheID()
        {
            var value = TestServiceProviderFactory.Create().GetRequiredService<ModulatableValue<float>>();
            var modulator = new TestMultiOutputModulator();

            value.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, modulator.Outputs[0]));
            value.SetModulatorCommand.Execute(null);

            Assert.Null(value.BoundModulator);
            Assert.Null(value.ModulatorID);
            Assert.Null(value.BoundOutputName);
        }
    }
}

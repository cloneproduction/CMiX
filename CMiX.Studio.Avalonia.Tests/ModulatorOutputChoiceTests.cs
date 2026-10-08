using System.Globalization;
using Avalonia.Headless.XUnit;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using CMiX.Studio.Avalonia.Converters;
using CMiX.Studio.Avalonia.Views.Controls;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The rows of the modulator assign menu, built on a real filter value and real modulators.
    public class ModulatorOutputChoiceTests
    {
        // A Blur filter with a Random modulator (one output) and an FFT modulator (five outputs).
        private static (Blur Blur, IModulator Random, IModulator Fft) CreateBlurWithModulators()
        {
            var provider = TestServiceProviderFactory.Create();
            var blur = (Blur)provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            blur.ModulatorManager.AddItem(typeof(RandomModulator));
            blur.ModulatorManager.AddItem(typeof(FFTModulator));

            var items = blur.ModulatorManager.ManagerData.Items;
            return (blur, items.OfType<RandomModulator>().Single(), items.OfType<FFTModulator>().Single());
        }

        private static List<ModulatorOutputSelection> Selections(params IModulator[] modulators) =>
            modulators.SelectMany(m => m.Outputs.Select(o => new ModulatorOutputSelection(m, o))).ToList();

        private static ModulatorOutputChoice Row(IReadOnlyList<ModulatorOutputChoice> rows, IModulator modulator, string outputName) =>
            rows.Single(r => r.Selection != null && r.Selection.Modulator == modulator && r.Selection.Output.Name == outputName);

        private static void Bind(IModulatorBindable bindable, IModulator modulator, string outputName) =>
            bindable.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, modulator.Outputs.Single(o => o.Name == outputName)));

        [AvaloniaFact]
        public void Build_ListsTitleSeparatorOneRowPerOutputAndUnassignLast()
        {
            var (blur, random, fft) = CreateBlurWithModulators();
            var selections = Selections(random, fft);

            var rows = ModulatorOutputChoice.Build(selections, blur.Strength);

            Assert.Equal(
                new[] { "Assign Modulator", "-", "Random Value", "FFT FFT", "FFT Bass", "FFT LowerMid", "FFT HigherMid", "FFT High", "Unassign" },
                rows.Select(r => r.Label).ToArray());
            Assert.Equal<ModulatorOutputSelection?>(selections, rows.Skip(2).Take(selections.Count).Select(r => r.Selection));
            Assert.Null(rows[0].Selection);
            Assert.Null(rows[1].Selection);
            Assert.Null(rows[^1].Selection);
        }

        [AvaloniaFact]
        public void Label_IsTheSameTextAsTheOldConverter()
        {
            var (_, random, fft) = CreateBlurWithModulators();
            var converter = new ModulatorOutputSelectionToLabelConverter();

            foreach (var selection in Selections(random, fft))
                Assert.Equal(
                    converter.Convert(selection, typeof(string), null, CultureInfo.InvariantCulture),
                    ModulatorOutputChoice.For(selection, null).Label);
        }

        [AvaloniaFact]
        public void IsAssigned_IsTrueOnlyForTheBoundOutput()
        {
            var (blur, random, fft) = CreateBlurWithModulators();
            Bind(blur.Strength, fft, "Bass");

            var rows = ModulatorOutputChoice.Build(Selections(random, fft), blur.Strength);

            Assert.True(Row(rows, fft, "Bass").IsAssigned);
            Assert.Single(rows, r => r.IsAssigned);
        }

        [AvaloniaFact]
        public void OutputRowCommand_BindsTheModulatorOutput()
        {
            var (blur, random, fft) = CreateBlurWithModulators();
            var rows = ModulatorOutputChoice.Build(Selections(random, fft), blur.Strength);
            var command = Row(rows, fft, "Bass").Command!;

            Assert.True(command.CanExecute(null));
            command.Execute(null);

            Assert.Same(fft, blur.Strength.BoundModulator);
            Assert.Equal("Bass", blur.Strength.BoundOutputName);
        }

        [AvaloniaFact]
        public void Unassign_CannotExecuteWhenNothingIsBound_AndClearsTheBinding()
        {
            var (blur, random, fft) = CreateBlurWithModulators();
            var unassign = ModulatorOutputChoice.Build(Selections(random, fft), blur.Strength)[^1];

            Assert.Equal("Unassign", unassign.Label);
            Assert.False(unassign.IsAssigned);
            Assert.False(unassign.Command!.CanExecute(null));

            Bind(blur.Strength, random, "Value");
            Assert.True(unassign.Command.CanExecute(null));

            unassign.Command.Execute(null);

            Assert.Null(blur.Strength.BoundModulator);
            Assert.Null(blur.Strength.BoundOutputName);
            Assert.False(unassign.Command.CanExecute(null));
        }

        [Fact]
        public void Title_CannotExecute_AndSeparatorHasADashAndNoCommand()
        {
            var title = ModulatorOutputChoice.Title("Assign Modulator");
            var separator = ModulatorOutputChoice.Separator();

            Assert.Equal("Assign Modulator", title.Label);
            Assert.False(title.IsAssigned);
            Assert.False(title.Command!.CanExecute(null));
            Assert.Equal("-", separator.Label);
            Assert.Null(separator.Command);
        }

        [AvaloniaFact]
        public void NullBindable_NoRowIsAssigned_OutputRowsHaveNoCommand_AndUnassignCannotExecute()
        {
            var (_, random, fft) = CreateBlurWithModulators();

            var rows = ModulatorOutputChoice.Build(Selections(random, fft), null);

            Assert.DoesNotContain(rows, r => r.IsAssigned);
            Assert.All(rows.Where(r => r.Selection != null), r => Assert.Null(r.Command));
            Assert.False(rows[^1].Command!.CanExecute(null));
            rows[^1].Command!.Execute(null);
        }
    }
}

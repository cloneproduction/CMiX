using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using CMiX.Studio.Avalonia.Converters;
using CMiX.Studio.Avalonia.Views.Modulation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The tooltip of the assign button names what is assigned: the modulator and the output, as the menu names them.
    public class ModulatorAssignToolTipTests
    {
        private static (Blur Blur, ButtonModulatorAssign Button, Button Inner) Show()
        {
            var provider = TestServiceProviderFactory.Create();
            var blur = (Blur)provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            blur.ModulatorManager.AddItem(typeof(RandomModulator));
            blur.ModulatorManager.AddItem(typeof(FFTModulator));

            var view = new Views.Blur { DataContext = blur };
            new Window { Content = view, Width = 500, Height = 800 }.Show();
            Dispatcher.UIThread.RunJobs();

            var button = view.GetVisualDescendants().OfType<ButtonModulatorAssign>().Single(b => b.DataContext == blur.Strength);
            return (blur, button, button.GetVisualDescendants().OfType<Button>().First());
        }

        private static void Bind(Blur blur, IModulator modulator, string outputName)
        {
            blur.Strength.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, modulator.Outputs.Single(o => o.Name == outputName)));
            TestServiceProviderFactory.Pump();
        }

        [AvaloniaFact]
        public void Tooltip_WithNothingAssigned_AsksToAssign()
        {
            var (_, _, inner) = Show();

            Assert.Equal("Assign Modulator", ToolTip.GetTip(inner));
        }

        [AvaloniaFact]
        public void Tooltip_WithAnAssignedOutput_NamesTheModulatorAndTheOutput()
        {
            var (blur, _, inner) = Show();

            Bind(blur, blur.ModulatorManager.ManagerData.Items.OfType<RandomModulator>().Single(), "Value");
            Assert.Equal("Random Value", ToolTip.GetTip(inner));

            var fft = blur.ModulatorManager.ManagerData.Items.OfType<FFTModulator>().Single();
            Bind(blur, fft, "Bass");
            Assert.Equal("FFT Bass", ToolTip.GetTip(inner));
        }

        [AvaloniaFact]
        public void Tooltip_AfterUnassignOrRename_ShowsTheNewText()
        {
            var (blur, _, inner) = Show();
            var random = blur.ModulatorManager.ManagerData.Items.OfType<RandomModulator>().Single();
            Bind(blur, random, "Value");

            random.PrefabService.Name.Value = "Wobble Modulator";
            TestServiceProviderFactory.Pump();
            Assert.Equal("Wobble Value", ToolTip.GetTip(inner));

            blur.Strength.SetModulatorCommand.Execute(null);
            TestServiceProviderFactory.Pump();
            Assert.Equal("Assign Modulator", ToolTip.GetTip(inner));
        }

        [AvaloniaFact]
        public void Tooltip_Open_IsBelowTheButton()
        {
            var (_, _, inner) = Show();

            Assert.Equal(PlacementMode.Custom, ToolTip.GetPlacement(inner));
            Assert.Equal(CMiX.UI.Popups.PopupSide.Bottom, CMiX.UI.AttachedProperties.ToolTipDefaults.GetSide(inner));
        }

        [Fact]
        public void Convert_NoModulatorOrNoOutput_GivesTheTitle()
        {
            var converter = new ModulatorToToolTipConverter();
            var culture = System.Globalization.CultureInfo.InvariantCulture;

            Assert.Equal("Assign Modulator", converter.Convert(new object?[] { null, null, null }, typeof(string), null, culture));
            Assert.Equal("Assign Modulator", converter.Convert(new object?[] { null, "Value", null }, typeof(string), null, culture));
            Assert.Equal("Assign Modulator", converter.Convert(new object?[0], typeof(string), null, culture));
        }
    }
}

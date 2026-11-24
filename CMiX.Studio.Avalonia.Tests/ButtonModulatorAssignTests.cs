// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using CMiX.Studio.Avalonia.Converters;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Views.Modulation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The dot of the assign button has one color for each state and each modulator category. The tooltip names what is assigned:
    // the modulator and the output, as the menu names them. The menu closes when the pointer goes far away.
    public class ButtonModulatorAssignTests : IDisposable
    {
        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

        private static Color Token(string key) => ((ISolidColorBrush)Application.Current!.FindResource(key)!).Color;

        private (Blur Blur, ButtonModulatorAssign Button) Show(params Type[] modulatorTypes)
        {
            var provider = TestServiceProviderFactory.Create();
            var blur = (Blur)provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            foreach (var type in modulatorTypes)
                blur.ModulatorManager.AddItem(type);

            var view = new Views.Blur { DataContext = blur };
            windows.Add(new Window { Content = view, Width = 500, Height = 800 }).Show();
            Dispatcher.UIThread.RunJobs();

            var button = view.GetVisualDescendants().OfType<ButtonModulatorAssign>().Single(b => b.DataContext == blur.Strength);
            return (blur, button);
        }

        private static IModulator Bind(Blur blur, Type modulatorType)
        {
            var modulator = blur.ModulatorManager.ManagerData.Items.OfType<IModulator>().Single(m => m.GetType() == modulatorType);
            var output = modulator.Outputs.First(((IModulatorBindable)blur.Strength).CanBind);
            blur.Strength.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, output));
            TestServiceProviderFactory.Pump();
            return modulator;
        }

        private static Color? DotColor(ButtonModulatorAssign button) =>
            (button.GetVisualDescendants().OfType<global::Avalonia.Controls.Shapes.Ellipse>().Single().Fill as ISolidColorBrush)?.Color;

        [AvaloniaFact]
        public void Dot_WithNothingAssigned_IsUnassigned()
        {
            var (_, button) = Show(typeof(RandomModulator));

            Assert.Equal(Token("ModulatorUnassignedBrush"), DotColor(button));
        }

        [AvaloniaFact]
        public void Dot_WithAnInternalModulator_IsInternalAndLightsOnHover()
        {
            var (blur, button) = Show(typeof(RandomModulator));
            var modulator = Bind(blur, typeof(RandomModulator));

            Assert.Equal(Token("ModulatorInternalBrush"), DotColor(button));

            modulator.IsHovered = true;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(Token("ModulatorInternalHighlightBrush"), DotColor(button));
        }

        [AvaloniaFact]
        public void Dot_WithAnExternalModulator_IsExternalAndLightsOnHover()
        {
            var (blur, button) = Show(typeof(RedisFloatModulator));
            var modulator = Bind(blur, typeof(RedisFloatModulator));

            Assert.Equal(Token("ModulatorExternalBrush"), DotColor(button));

            modulator.IsHovered = true;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(Token("ModulatorExternalHighlightBrush"), DotColor(button));
        }

        [AvaloniaFact]
        public void Dot_OfTheTwoCategories_HaveDifferentColors()
        {
            Assert.NotEqual(Token("ModulatorInternalBrush"), Token("ModulatorExternalBrush"));
            Assert.NotEqual(Token("ModulatorInternalHighlightBrush"), Token("ModulatorExternalHighlightBrush"));
        }

        [AvaloniaFact]
        public void Dot_AfterUnassign_GoesBackToUnassigned()
        {
            var (blur, button) = Show(typeof(RedisFloatModulator));
            Bind(blur, typeof(RedisFloatModulator));

            blur.Strength.SetModulatorCommand.Execute(null);
            TestServiceProviderFactory.Pump();

            Assert.Equal(Token("ModulatorUnassignedBrush"), DotColor(button));
        }

        private (Blur Blur, ButtonModulatorAssign Button, Button Inner) ShowForTooltip()
        {
            var provider = TestServiceProviderFactory.Create();
            var blur = (Blur)provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            blur.ModulatorManager.AddItem(typeof(RandomModulator));
            blur.ModulatorManager.AddItem(typeof(FFTModulator));

            var view = new Views.Blur { DataContext = blur };
            windows.Add(new Window { Content = view, Width = 500, Height = 800 }).Show();
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
            var (_, _, inner) = ShowForTooltip();

            Assert.Equal("Assign Modulator", ToolTip.GetTip(inner));
        }

        [AvaloniaFact]
        public void Tooltip_WithAnAssignedOutput_NamesTheModulatorAndTheOutput()
        {
            var (blur, _, inner) = ShowForTooltip();

            Bind(blur, blur.ModulatorManager.ManagerData.Items.OfType<RandomModulator>().Single(), "Value");
            Assert.Equal("Random Value", ToolTip.GetTip(inner));

            var fft = blur.ModulatorManager.ManagerData.Items.OfType<FFTModulator>().Single();
            Bind(blur, fft, "Bass");
            Assert.Equal("FFT Bass", ToolTip.GetTip(inner));
        }

        [AvaloniaFact]
        public void Tooltip_AfterUnassignOrRename_ShowsTheNewText()
        {
            var (blur, _, inner) = ShowForTooltip();
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
            var (_, _, inner) = ShowForTooltip();

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

        [AvaloniaFact]
        public void MouseMove_FarFromTheOpenMenu_ClosesTheMenu()
        {
            var provider = TestServiceProviderFactory.Create();
            var model = (CMiX.Core.Texturing.Filters.Blur)provider.GetRequiredService<ControlFactory>().Create(typeof(CMiX.Core.Texturing.Filters.Blur));
            model.ModulatorManager.AddItem(typeof(CMiX.Core.Modulation.Modulators.RandomModulator));
            var view = new Views.Blur { DataContext = model };
            var window = windows.Add(new Window { Content = view, Width = 900, Height = 900 });
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var assign = view.GetVisualDescendants().OfType<ButtonModulatorAssign>().Single(b => b.DataContext == model.Strength);
            var button = assign.FindControl<Button>("assignButton")!;
            button.Flyout!.ShowAt(button);
            Dispatcher.UIThread.RunJobs();
            Assert.True(button.Flyout.IsOpen);

            window.MouseMove(new Point(window.Width - 10, window.Height - 10));
            Dispatcher.UIThread.RunJobs();

            Assert.False(button.Flyout.IsOpen);
        }
    }
}

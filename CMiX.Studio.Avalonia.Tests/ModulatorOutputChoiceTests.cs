// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Animations;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Views.Modulation;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Tests.Support;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The rows of the modulator assign menu, built on a real filter value and real modulators, and the assign popup of a real
    // filter view: its rows, its assigned row, and what a click does.
    public class ModulatorOutputChoiceTests : IDisposable
    {
        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

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
        public void Build_TwoModulators_ListsTitleSeparatorOneRowPerOutputAndUnassignLast()
        {
            var (blur, random, fft) = CreateBlurWithModulators();
            var selections = Selections(random, fft);

            var rows = ModulatorOutputChoice.Build(selections, blur.Strength);

            Assert.Equal(
                new[] { "Assign Modulator", "-", "Random Value", "FFT Bass", "FFT LowerMid", "FFT HigherMid", "FFT High", "Unassign" },
                rows.Select(r => r.Label).ToArray());
            Assert.Equal<ModulatorOutputSelection?>(selections, rows.Skip(2).Take(selections.Count).Select(r => r.Selection));
            Assert.Null(rows[0].Selection);
            Assert.Null(rows[1].Selection);
            Assert.Null(rows[^1].Selection);
        }

        [AvaloniaFact]
        public void ForBindable_ABindableThatRefusesAnOutput_LeavesItOut()
        {
            var (blur, _, _) = CreateBlurWithModulators();
            blur.ModulatorManager.AddItem(typeof(RedisBoolModulator));
            var refused = blur.ModulatorManager.ManagerData.Items.OfType<RedisBoolModulator>().Single();

            var rows = ModulatorOutputChoice.ForBindable(blur.ModulatorManager.ManagerData.Items, blur.Strength);

            Assert.Equal(
                new[] { "Assign Modulator", "-", "Random Value", "FFT Bass", "FFT LowerMid", "FFT HigherMid", "FFT High", "Unassign" },
                rows.Select(r => r.Label).ToArray());
            Assert.DoesNotContain(rows, r => r.Selection != null && r.Selection.Modulator == refused);
        }

        [AvaloniaFact]
        public void ForBindable_NoBindable_GivesEveryOutput()
        {
            var (blur, random, fft) = CreateBlurWithModulators();
            blur.ModulatorManager.AddItem(typeof(RedisBoolModulator));
            var redisBool = blur.ModulatorManager.ManagerData.Items.OfType<RedisBoolModulator>().Single();

            var rows = ModulatorOutputChoice.ForBindable(blur.ModulatorManager.ManagerData.Items, null);

            Assert.Equal<ModulatorOutputSelection?>(
                Selections(random, fft, redisBool),
                rows.Skip(2).Take(rows.Count - 3).Select(r => r.Selection));
        }

        [AvaloniaFact]
        public void Build_AnAddressedOutput_ShowsItsAddressInTheLabel()
        {
            var (blur, _, _) = CreateBlurWithModulators();
            blur.ModulatorManager.AddItem(typeof(RedisFloatModulator));
            var modulator = blur.ModulatorManager.ManagerData.Items.OfType<RedisFloatModulator>().Single();
            modulator.Registry.AddressManager.AddItem(typeof(RedisAddress));
            var address = (RedisAddress)modulator.Registry.AddressManager.SelectedItem;
            address.Value = "cmix:speed";
            modulator.SelectedAddress = address;

            var rows = ModulatorOutputChoice.Build(Selections(modulator), blur.Strength);

            Assert.EndsWith(" cmix:speed", rows[2].Label);
        }

        [AvaloniaFact]
        public void ABoolAddressedOutput_BoundToPlaying_ShowsItsAddressAndIsAssigned()
        {
            var provider = TestServiceProviderFactory.Create();
            var player = (SequencePlayer)provider.GetRequiredService<ControlFactory>().Create(typeof(SequencePlayer));
            player.ModulatorManager.AddItem(typeof(RedisBoolModulator));
            var modulator = player.ModulatorManager.ManagerData.Items.OfType<RedisBoolModulator>().Single();
            modulator.Registry.AddressManager.AddItem(typeof(RedisAddress));
            var address = (RedisAddress)modulator.Registry.AddressManager.SelectedItem;
            address.Value = "cmix:play";
            modulator.SelectedAddress = address;
            Bind(player.Playing, modulator, "Value");

            var rows = ModulatorOutputChoice.Build(Selections(modulator), player.Playing);

            Assert.EndsWith(" cmix:play", rows[2].Label);
            Assert.True(rows[2].IsAssigned);
        }

        [AvaloniaFact]
        public void AnIntAddressedOutput_BoundToSeekTarget_ShowsItsAddressAndIsAssigned()
        {
            var provider = TestServiceProviderFactory.Create();
            var player = (SequencePlayer)provider.GetRequiredService<ControlFactory>().Create(typeof(SequencePlayer));
            player.ModulatorManager.AddItem(typeof(RedisIntModulator));
            var modulator = player.ModulatorManager.ManagerData.Items.OfType<RedisIntModulator>().Single();
            modulator.Registry.AddressManager.AddItem(typeof(RedisAddress));
            var address = (RedisAddress)modulator.Registry.AddressManager.SelectedItem;
            address.Value = "cmix:seek";
            modulator.SelectedAddress = address;
            Bind(player.SeekTarget, modulator, "Value");

            var rows = ModulatorOutputChoice.Build(Selections(modulator), player.SeekTarget);

            Assert.EndsWith(" cmix:seek", rows[2].Label);
            Assert.True(rows[2].IsAssigned);
        }

        [AvaloniaFact]
        public void AnAddressedOutput_WithoutASelection_ShowsValueInTheLabel()
        {
            var (blur, _, _) = CreateBlurWithModulators();
            blur.ModulatorManager.AddItem(typeof(RedisFloatModulator));
            var modulator = blur.ModulatorManager.ManagerData.Items.OfType<RedisFloatModulator>().Single();

            var rows = ModulatorOutputChoice.Build(Selections(modulator), blur.Strength);

            Assert.EndsWith(" Value", rows[2].Label);
        }

        [AvaloniaFact]
        public void IsAssigned_ABoundOutput_IsTrueOnlyForThatOutput()
        {
            var (blur, random, fft) = CreateBlurWithModulators();
            Bind(blur.Strength, fft, "Bass");

            var rows = ModulatorOutputChoice.Build(Selections(random, fft), blur.Strength);

            Assert.True(Row(rows, fft, "Bass").IsAssigned);
            Assert.Single(rows, r => r.IsAssigned);
        }

        [AvaloniaFact]
        public void Command_OfAnOutputRow_BindsTheModulatorOutput()
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
        public void Build_NoBindable_AssignsNoRowAndGivesNoCommand()
        {
            var (_, random, fft) = CreateBlurWithModulators();

            var rows = ModulatorOutputChoice.Build(Selections(random, fft), null);

            Assert.DoesNotContain(rows, r => r.IsAssigned);
            Assert.All(rows.Where(r => r.Selection != null), r => Assert.Null(r.Command));
            Assert.False(rows[^1].Command!.CanExecute(null));
            rows[^1].Command!.Execute(null);
        }

        private (Blur Blur, FFTModulator Fft, Window Window, ButtonModulatorAssign Button) Show()
        {
            var provider = TestServiceProviderFactory.Create();
            var blur = (Blur)provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            blur.ModulatorManager.AddItem(typeof(RandomModulator));
            blur.ModulatorManager.AddItem(typeof(FFTModulator));

            var view = new Views.Blur { DataContext = blur };
            var window = windows.Add(new Window { Content = view, Width = 500, Height = 800 });
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var button = view.GetVisualDescendants().OfType<ButtonModulatorAssign>().Single(b => b.DataContext == blur.Strength);
            return (blur, blur.ModulatorManager.ManagerData.Items.OfType<FFTModulator>().Single(), window, button);
        }

        private static FlyoutBase FlyoutOf(ButtonModulatorAssign button) => button.FindControl<Button>("assignButton")!.Flyout!;

        private static List<MenuItem> Open(Window window, ButtonModulatorAssign button)
        {
            FlyoutOf(button).ShowAt(button.FindControl<Button>("assignButton")!);
            Dispatcher.UIThread.RunJobs();
            return window.GetVisualDescendants().OfType<MenuFlyoutPresenter>().Single().GetVisualDescendants().OfType<MenuItem>().ToList();
        }

        private static MenuItem Row(List<MenuItem> rows, string header) => rows.Single(r => Equals(r.Header, header));

        [AvaloniaFact]
        public void AssignPopup_ListsTheTitleTheOutputsAndUnassign_InAPopupFrame()
        {
            var (_, _, window, button) = Show();

            var rows = Open(window, button);

            Assert.Equal(
                new object?[] { "Assign Modulator", "-", "Random Value", "FFT Bass", "FFT LowerMid", "FFT HigherMid", "FFT High", "Unassign" },
                rows.Select(r => r.Header).ToArray());
            Assert.All(rows, row => Assert.NotNull(row.FindAncestorOfType<PopupFrame>()));
        }

        [AvaloniaFact]
        public void Open_ThePopupWithNothingBound_HasNoCheckedRowAndADisabledUnassign()
        {
            var (_, _, window, button) = Show();

            var rows = Open(window, button);

            Assert.DoesNotContain(rows, r => r.Classes.Contains(":checked"));
            Assert.False(Row(rows, "Unassign").IsEffectivelyEnabled);
            Assert.False(Row(rows, "Assign Modulator").IsEffectivelyEnabled);
            Assert.True(Row(rows, "FFT Bass").IsEffectivelyEnabled);
        }

        [AvaloniaFact]
        public void Click_AnOutputInThePopup_BindsItAndClosesThePopup()
        {
            var (blur, fft, window, button) = Show();
            var rows = Open(window, button);

            PointerInput.Click(window, Row(rows, "FFT Bass"), MouseButton.Left);
            TestServiceProviderFactory.Pump();

            Assert.Same(fft, blur.Strength.BoundModulator);
            Assert.Equal("Bass", blur.Strength.BoundOutputName);
            Assert.False(FlyoutOf(button).IsOpen);
        }

        [AvaloniaFact]
        public void Open_ThePopupAgain_MarksTheAssignedRowAndUnassignClearsIt()
        {
            var (blur, _, window, button) = Show();
            PointerInput.Click(window, Row(Open(window, button), "FFT Bass"), MouseButton.Left);
            TestServiceProviderFactory.Pump();

            var rows = Open(window, button);

            Assert.True(Row(rows, "FFT Bass").Classes.Contains(":checked"));
            Assert.False(Row(rows, "Random Value").Classes.Contains(":checked"));
            Assert.True(Row(rows, "Unassign").IsEffectivelyEnabled);

            PointerInput.Click(window, Row(rows, "Unassign"), MouseButton.Left);
            TestServiceProviderFactory.Pump();

            Assert.Null(blur.Strength.BoundModulator);
            Assert.False(FlyoutOf(button).IsOpen);
        }

        [AvaloniaFact]
        public void AssignPopup_OpensAtThePointer_LikeAContextMenu()
        {
            var (_, _, _, button) = Show();

            Assert.Equal(PlacementMode.Pointer, ((PopupFlyoutBase)FlyoutOf(button)).Placement);
        }

        [AvaloniaFact]
        public void AssignPopup_WithNothingBound_HasNoAnchorRow()
        {
            var (_, _, window, button) = Show();

            var rows = Open(window, button);

            Assert.DoesNotContain(rows, row => MenuAnchor.GetIsAnchor(row));
        }

        [AvaloniaFact]
        public void AssignPopup_WithABoundOutput_AnchorsOnlyTheAssignedRow()
        {
            var (_, _, window, button) = Show();
            PointerInput.Click(window, Row(Open(window, button), "FFT Bass"), MouseButton.Left);
            TestServiceProviderFactory.Pump();

            var rows = Open(window, button);

            Assert.True(MenuAnchor.GetIsAnchor(Row(rows, "FFT Bass")));
            Assert.Single(rows, row => MenuAnchor.GetIsAnchor(row));
        }

        [AvaloniaFact]
        public void AssignPopup_AfterUnassign_HasNoAnchorRowAgain()
        {
            var (_, _, window, button) = Show();
            PointerInput.Click(window, Row(Open(window, button), "FFT Bass"), MouseButton.Left);
            TestServiceProviderFactory.Pump();
            PointerInput.Click(window, Row(Open(window, button), "Unassign"), MouseButton.Left);
            TestServiceProviderFactory.Pump();

            var rows = Open(window, button);

            Assert.DoesNotContain(rows, row => MenuAnchor.GetIsAnchor(row));
        }

        [AvaloniaFact]
        public void Open_ThePopupAfterAnAdd_ShowsTheNewModulator()
        {
            var (blur, _, window, button) = Show();
            var before = Open(window, button).Count;
            FlyoutOf(button).Hide();
            Dispatcher.UIThread.RunJobs();

            blur.ModulatorManager.AddItem(typeof(RandomModulator));
            var rows = Open(window, button);

            Assert.Equal(before + 1, rows.Count);
        }
    }
}

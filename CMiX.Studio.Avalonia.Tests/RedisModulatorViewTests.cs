// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Animations;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.UI.Controls.Enum;
using CMiX.UI.Controls.Number;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ViewRedisBoolModulator = CMiX.Studio.Avalonia.Views.RedisBoolModulator;
using ViewRedisFloatModulator = CMiX.Studio.Avalonia.Views.RedisFloatModulator;
using ViewRedisIntModulator = CMiX.Studio.Avalonia.Views.RedisIntModulator;

namespace CMiX.Studio.Avalonia.Tests
{
    // The address combo box that the views of the three Redis modulators share, and the range of the float modulator.
    public class RedisModulatorViewTests : IDisposable
    {
        private readonly IServiceProvider _provider = TestServiceProviderFactory.Create();

        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

        private RedisAddressRegistry Registry => _provider.GetRequiredService<RedisAddressRegistry>();

        // Adds the modulator to a control that has a value of the output type.
        private RedisModulatorBase NewModulator(Type modulatorType)
        {
            var hostType = modulatorType == typeof(RedisFloatModulator) ? typeof(Blur) : typeof(SequencePlayer);
            var host = (IModulatable)_provider.GetRequiredService<ControlFactory>().Create(hostType);
            host.ModulatorManager.AddItem(modulatorType);
            return (RedisModulatorBase)host.ModulatorManager.SelectedItem;
        }

        private RedisAddress AddAddress(string text)
        {
            Registry.AddAddress();
            var address = (RedisAddress)Registry.AddressManager.SelectedItem;
            address.Value = text;
            return address;
        }

        private Control Show(Type viewType, RedisModulatorBase modulator)
        {
            var view = (Control)Activator.CreateInstance(viewType)!;
            view.DataContext = modulator;
            windows.Add(new Window { Content = view, Width = 400, Height = 400 }).Show();
            Dispatcher.UIThread.RunJobs();
            return view;
        }

        private static ComboBoxCaptioned AddressBox(Control view) =>
            view.GetVisualDescendants().OfType<ComboBoxCaptioned>().Single(combo => combo.Caption == "Address");

        [AvaloniaTheory]
        [InlineData(typeof(ViewRedisFloatModulator), typeof(RedisFloatModulator))]
        [InlineData(typeof(ViewRedisBoolModulator), typeof(RedisBoolModulator))]
        [InlineData(typeof(ViewRedisIntModulator), typeof(RedisIntModulator))]
        public void Show_TheAddressBox_ListsTheAddressesOfTheProject(Type viewType, Type modulatorType)
        {
            var modulator = NewModulator(modulatorType);
            var first = AddAddress("cmix:kick");
            var second = AddAddress("cmix:snare");

            var view = Show(viewType, modulator);

            Assert.Equal(new object[] { first, second }, AddressBox(view).ItemsSource!.Cast<object>());
        }

        [AvaloniaTheory]
        [InlineData(typeof(ViewRedisFloatModulator), typeof(RedisFloatModulator))]
        [InlineData(typeof(ViewRedisBoolModulator), typeof(RedisBoolModulator))]
        [InlineData(typeof(ViewRedisIntModulator), typeof(RedisIntModulator))]
        public void SelectedItem_PickingAnAddress_SetsTheSelectedAddressID(Type viewType, Type modulatorType)
        {
            var modulator = NewModulator(modulatorType);
            AddAddress("cmix:kick");
            var second = AddAddress("cmix:snare");
            var view = Show(viewType, modulator);

            AddressBox(view).SelectedItem = second;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(second.ID, modulator.SelectedAddressID.Value);
        }

        [AvaloniaTheory]
        [InlineData(typeof(ViewRedisFloatModulator), typeof(RedisFloatModulator))]
        [InlineData(typeof(ViewRedisBoolModulator), typeof(RedisBoolModulator))]
        [InlineData(typeof(ViewRedisIntModulator), typeof(RedisIntModulator))]
        public void Show_AModulatorWithASelection_ShowsItAsTheSelectedItem(Type viewType, Type modulatorType)
        {
            var modulator = NewModulator(modulatorType);
            AddAddress("cmix:kick");
            var second = AddAddress("cmix:snare");
            modulator.SelectedAddress = second;

            var view = Show(viewType, modulator);

            Assert.Same(second, AddressBox(view).SelectedItem);
        }

        [AvaloniaTheory]
        [InlineData(typeof(ViewRedisFloatModulator), typeof(RedisFloatModulator))]
        [InlineData(typeof(ViewRedisBoolModulator), typeof(RedisBoolModulator))]
        [InlineData(typeof(ViewRedisIntModulator), typeof(RedisIntModulator))]
        public void ResetCommand_OfTheAddressBox_ClearsTheSelection(Type viewType, Type modulatorType)
        {
            var modulator = NewModulator(modulatorType);
            AddAddress("cmix:kick");
            modulator.SelectedAddress = AddAddress("cmix:snare");
            var view = Show(viewType, modulator);
            var box = AddressBox(view);

            box.ResetCommand!.Execute(null);
            Dispatcher.UIThread.RunJobs();

            Assert.Null(modulator.SelectedAddressID.Value);
            Assert.Null(box.SelectedItem);
        }

        [AvaloniaTheory]
        [InlineData(typeof(ViewRedisFloatModulator), typeof(RedisFloatModulator))]
        [InlineData(typeof(ViewRedisBoolModulator), typeof(RedisBoolModulator))]
        [InlineData(typeof(ViewRedisIntModulator), typeof(RedisIntModulator))]
        public void Show_TheView_LeavesNothingOnTheUndoStack(Type viewType, Type modulatorType)
        {
            AddAddress("cmix:kick");
            var modulator = NewModulator(modulatorType);
            var undo = _provider.GetRequiredService<UndoManager>();
            undo.Clear();

            Show(viewType, modulator);

            Assert.False(undo.CanUndo);
        }

        [AvaloniaTheory]
        [InlineData(typeof(ViewRedisBoolModulator), typeof(RedisBoolModulator))]
        [InlineData(typeof(ViewRedisIntModulator), typeof(RedisIntModulator))]
        public void Show_AViewWithoutARange_HasNoDragValue(Type viewType, Type modulatorType)
        {
            var view = Show(viewType, NewModulator(modulatorType));

            Assert.Empty(view.GetVisualDescendants().OfType<DragValue>());
        }

        private static RangeValue RangeOf(Control view) => view.GetVisualDescendants().OfType<RangeValue>().Single();

        [AvaloniaFact]
        public void LeftValue_TheRangeValueOfAFloatModulator_ChangesTheMinimum()
        {
            var modulator = (RedisFloatModulator)NewModulator(typeof(RedisFloatModulator));
            var view = Show(typeof(ViewRedisFloatModulator), modulator);

            RangeOf(view).LeftValue = -2.0;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(-2.0f, modulator.Range.Minimum.Value);
        }

        [AvaloniaFact]
        public void RightValue_TheRangeValueOfAFloatModulator_ChangesTheMaximum()
        {
            var modulator = (RedisFloatModulator)NewModulator(typeof(RedisFloatModulator));
            var view = Show(typeof(ViewRedisFloatModulator), modulator);

            RangeOf(view).RightValue = 3.0;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(3.0f, modulator.Range.Maximum.Value);
        }
    }
}

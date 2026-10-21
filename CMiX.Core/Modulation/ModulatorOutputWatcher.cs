// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Specialized;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation
{
    // Unbinds a value when the output it is bound to is removed from a modulator of the list.
    public sealed class ModulatorOutputWatcher : IDisposable
    {
        private readonly PrefabManager _manager;
        private readonly Func<IEnumerable<IModulatorBindable>> _bindables;
        private readonly Dictionary<IModulator, Action<string>> _handlers = new();

        public ModulatorOutputWatcher(PrefabManager manager, Func<IEnumerable<IModulatorBindable>> bindables)
        {
            _manager = manager;
            _bindables = bindables;
            _manager.ManagerData.Items.CollectionChanged += OnItemsChanged;
        }

        private void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
                UnwatchAll();

            if (e.OldItems != null && e.Action != NotifyCollectionChangedAction.Move)
                foreach (var item in e.OldItems.OfType<IModulator>())
                    Unwatch(item);

            if (e.NewItems != null && e.Action != NotifyCollectionChangedAction.Move)
                foreach (var item in e.NewItems.OfType<IModulator>())
                    Watch(item);
        }

        private void Watch(IModulator modulator)
        {
            if (modulator is not INotifyOutputRemoved source || _handlers.ContainsKey(modulator)) return;

            Action<string> handler = outputName => Unbind(modulator, outputName);
            _handlers[modulator] = handler;
            source.OutputRemoved += handler;
        }

        private void Unwatch(IModulator modulator)
        {
            if (!_handlers.Remove(modulator, out var handler)) return;
            ((INotifyOutputRemoved)modulator).OutputRemoved -= handler;
        }

        private void UnwatchAll()
        {
            foreach (var modulator in _handlers.Keys.ToList())
                Unwatch(modulator);
        }

        private void Unbind(IModulator modulator, string outputName)
        {
            var bound = _bindables()
                .Where(bindable => bindable.ModulatorID == modulator.ID && bindable.BoundOutputName == outputName)
                .ToList();

            foreach (var bindable in bound)
                bindable.SetModulatorCommand.Execute(null);
        }

        public void Dispose()
        {
            _manager.ManagerData.Items.CollectionChanged -= OnItemsChanged;
            UnwatchAll();
        }
    }
}

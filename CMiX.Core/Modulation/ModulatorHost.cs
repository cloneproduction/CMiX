// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Specialized;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Modulation
{
    // Owns the modulator list of a control. It gives each bindable value a way to find its
    // modulator and unbinds a value when its modulator is removed.
    public sealed class ModulatorHost : IDisposable
    {
        private readonly List<IModulatorBindable> _bindables = new();

        public ModulatorHost(PrefabManager manager)
        {
            Manager = manager;
            Manager.ManagerData.Items.CollectionChanged += OnItemsChanged;
        }

        public PrefabManager Manager { get; }

        public IReadOnlyList<IModulatorBindable> Bindables => _bindables;

        public void Add(params IModulatorBindable[] bindables)
        {
            foreach (var bindable in bindables)
            {
                bindable.ModulatorLookup = Find;
                _bindables.Add(bindable);
            }
        }

        public void Load(PrefabManagerModel model) => LoadManager(Manager, model);

        private IModulator Find(Guid id) =>
            Manager.ManagerData.Items.OfType<IModulator>().FirstOrDefault(m => m.ID == id);

        private void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // A reorder is a Move that also fills OldItems. Only a real removal unbinds.
            if (e.Action != NotifyCollectionChangedAction.Remove || e.OldItems == null) return;

            foreach (IControl removed in e.OldItems)
                foreach (var bindable in _bindables.Where(b => b.ModulatorID == removed.ID))
                    bindable.SetModulatorCommand.Execute(null);
        }

        public void Dispose()
        {
            Manager.ManagerData.Items.CollectionChanged -= OnItemsChanged;
            Manager.Dispose();
        }
    }
}

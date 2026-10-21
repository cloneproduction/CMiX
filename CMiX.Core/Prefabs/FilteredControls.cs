// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CMiX.Core.Prefabs
{
    public sealed class FilteredControls<T> : ReadOnlyObservableCollection<T> where T : class, IControl
    {
        private readonly ObservableCollection<IControl> source;
        private readonly ObservableCollection<T> items;
        private readonly Func<T, bool> rule;

        public FilteredControls(ObservableCollection<IControl> source, Func<T, bool> rule = null)
            : this(source, rule ?? (_ => true), new ObservableCollection<T>())
        {
        }

        private FilteredControls(ObservableCollection<IControl> source, Func<T, bool> rule, ObservableCollection<T> items)
            : base(items)
        {
            this.source = source;
            this.rule = rule;
            this.items = items;
            Rebuild();
            source.CollectionChanged += OnSourceChanged;
        }

        private void OnSourceChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add when AddedAtTheEnd(e):
                    foreach (var item in e.NewItems.OfType<T>().Where(rule))
                        items.Add(item);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems.OfType<T>())
                        items.Remove(item);
                    break;
                default:
                    Rebuild();
                    break;
            }
        }

        private bool AddedAtTheEnd(NotifyCollectionChangedEventArgs e) =>
            e.NewStartingIndex + e.NewItems.Count == source.Count;

        private void Rebuild()
        {
            items.Clear();
            foreach (var item in source.OfType<T>().Where(rule))
                items.Add(item);
        }
    }
}

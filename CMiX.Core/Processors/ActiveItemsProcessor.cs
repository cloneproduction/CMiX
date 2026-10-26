// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Runtime.CompilerServices;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Processors
{
    // Gives the ids of the items of a list, in the order of the list. A hidden item is not in the result.
    // The result of a list is built again only after the list or a visibility changed.
    // Call it from the thread that changes the controls.
    public class ActiveItemsProcessor
    {
        private readonly VisibilityProcessor visibility;
        private readonly ConditionalWeakTable<ManagerData, Entry> entries = new();
        private int visibilityVersion;

        public ActiveItemsProcessor(VisibilityProcessor visibility)
        {
            this.visibility = visibility;
            visibility.Changed += () => visibilityVersion++;
        }

        public IReadOnlyList<Guid> GetActiveIds(PrefabManager manager) =>
            manager == null
                ? Array.Empty<Guid>()
                : entries.GetValue(manager.ManagerData, data => new Entry(data)).GetIds(visibility, visibilityVersion);

        private sealed class Entry
        {
            private readonly ManagerData data;
            private Guid[] ids;
            private int version;

            public Entry(ManagerData data)
            {
                this.data = data;
                data.Items.CollectionChanged += (_, _) => ids = null;
            }

            public IReadOnlyList<Guid> GetIds(VisibilityProcessor visibility, int visibilityVersion)
            {
                if (ids == null || version != visibilityVersion)
                {
                    ids = data.Items.Where(item => !visibility.IsHidden(item.ID)).Select(item => item.ID).ToArray();
                    version = visibilityVersion;
                }

                return ids;
            }
        }
    }
}

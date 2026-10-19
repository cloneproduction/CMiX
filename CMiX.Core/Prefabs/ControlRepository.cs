// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.ObjectModel;
using CMiX.Core.Colors.Modifiers;
using CMiX.Core.Compositing;
using CMiX.Core.Materials;
using CMiX.Core.Modifiers;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Prefabs
{
    public class ControlRepository : ObservableObject
    {
        private readonly Dictionary<Guid, HashSet<CollectionManager>> _collectionManagers = new();

        private readonly Dictionary<Guid, IControl> _controlsById = new();

        public ControlRepository()
        {
            Compositions = new FilteredControls<Composition>(Controls);
            Layers = new FilteredControls<Layer>(Controls);
            Textures = new FilteredControls<ITextureSource>(Controls);
            Cameras = new FilteredControls<Camera>(Controls);
            Lights = new FilteredControls<LightEntity>(Controls);
            ColorPalettes = new FilteredControls<ColorPaletteModifier>(Controls);
            Entities = new FilteredControls<Entity>(Controls);
            Texts = new FilteredControls<TextEntity>(Controls);
            ModelEntities = new FilteredControls<IPrefab>(Controls, prefab => prefab is Entity or TextEntity or FullscreenQuad);
        }

        public ObservableCollection<IControl> Controls { get; } = new();
        public FilteredControls<Composition> Compositions { get; }
        public FilteredControls<Layer> Layers { get; }
        public FilteredControls<ITextureSource> Textures { get; }
        public FilteredControls<Entity> Entities { get; }
        public FilteredControls<Camera> Cameras { get; }
        public FilteredControls<LightEntity> Lights { get; }
        public FilteredControls<TextEntity> Texts { get; }
        public FilteredControls<ColorPaletteModifier> ColorPalettes { get; }

        public FilteredControls<IPrefab> ModelEntities { get; }

        public void AddControl(IControl control, CollectionManager collectionManager)
        {
            if (control == null)
                return;

            if (!_collectionManagers.ContainsKey(control.ID))
                _collectionManagers[control.ID] = new HashSet<CollectionManager>();

            _collectionManagers[control.ID].Add(collectionManager);

            if (!_controlsById.ContainsKey(control.ID))
            {
                _controlsById[control.ID] = control;
                Controls.Add(control);
            }
        }

        public void RemoveControl(IControl control, CollectionManager collectionManager)
        {
            if (control == null || !_collectionManagers.ContainsKey(control.ID))
                return;

            _collectionManagers[control.ID].Remove(collectionManager);

            if (_collectionManagers[control.ID].Count == 0)
            {
                _collectionManagers.Remove(control.ID);
                if (_controlsById.TryGetValue(control.ID, out var indexed) && ReferenceEquals(indexed, control))
                    _controlsById.Remove(control.ID);
                Controls.Remove(control);
            }
        }

        public IControl GetControl(Guid id)
        {
            return _controlsById.TryGetValue(id, out var control) ? control : null;
        }

        public bool HasUsers(IControl control)
        {
            if (!_collectionManagers.ContainsKey(control.ID)) return false;
            return _collectionManagers[control.ID].Count > 0;
        }

        public IReadOnlyList<CollectionManager> GetCollectionManagers(IControl control) =>
            control != null && _collectionManagers.TryGetValue(control.ID, out var collectionManagers)
                ? collectionManagers.ToList()
                : new List<CollectionManager>();

        public static int GetRenderPriority(IControl c) => c switch
        {
            Project => 0,
            Composition => 1,
            Layer => 2,
            Entity or TextEntity or LightEntity or Camera => 3,
            Material => 4,
            ITextureSource => 5,
            IModifier => 6,
            _ => 99
        };

        public IEnumerable<IControl> GetOrderedControls() =>
            Controls.OrderBy(GetRenderPriority);
    }
}

// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CMiX.Core.Colors.Modifiers;
using CMiX.Core.Compositing;
using CMiX.Core.Materials;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Texturing.Sources;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Prefabs
{
    public class ControlRepository : ObservableObject
    {
        private readonly Dictionary<Guid, HashSet<Guid>> _referencers = new();

        // Mirrors Controls one to one so the network hot path resolves an id without scanning.
        // Controls is only ever mutated by AddControl and RemoveControl, and a control keeps the
        // id it was registered with, so the two can never drift apart.
        private readonly Dictionary<Guid, IControl> _controlsById = new();

        // A list, not a dictionary, because the probe below matches the first entry whose type is
        // assignable from the control and the entries are ordered on purpose (ITextureSource before
        // Entity); dictionary enumeration order is not a contract.
        private readonly List<(Type Type, Action<IControl> Action)> typeToAddAction;
        private readonly List<(Type Type, Action<IControl> Action)> typeToRemoveAction;
        private readonly Dictionary<Guid, Action<IControl>> _deleters = new();

        public ControlRepository()
        {
            typeToAddAction = new List<(Type, Action<IControl>)>
            {
                (typeof(Composition), c => Compositions.Add((Composition)c)),
                (typeof(Layer), c => Layers.Add((Layer)c)),
                (typeof(ITextureSource), c => Textures.Add((ITextureSource)c)),
                (typeof(Camera), c => Cameras.Add((Camera)c)),
                (typeof(LightEntity), c => Lights.Add((LightEntity)c)),
                (typeof(Entity), c => Entities.Add((Entity)c)),
                (typeof(Material), c => Materials.Add((Material)c)),
                (typeof(BeatModulator), c => BeatModulators.Add((BeatModulator)c)),
                (typeof(TextEntity), c => Texts.Add((TextEntity)c)),
                (typeof(ColorPaletteModifier), c => ColorPalettes.Add((ColorPaletteModifier)c))
            };

            typeToRemoveAction = new List<(Type, Action<IControl>)>
            {
                (typeof(Composition), c => Compositions.Remove((Composition)c)),
                (typeof(Layer), c => Layers.Remove((Layer)c)),
                (typeof(ITextureSource), c => Textures.Remove((ITextureSource)c)),
                (typeof(Camera), c => Cameras.Remove((Camera)c)),
                (typeof(LightEntity), c => Lights.Remove((LightEntity)c)),
                (typeof(Entity), c => Entities.Remove((Entity)c)),
                (typeof(Material), c => Materials.Remove((Material)c)),
                (typeof(BeatModulator), c => BeatModulators.Remove((BeatModulator)c)),
                (typeof(TextEntity), c => Texts.Remove((TextEntity)c)),
                (typeof(ColorPaletteModifier), c => ColorPalettes.Remove((ColorPaletteModifier)c))
            };

            Entities.CollectionChanged += OnEntitiesOrTextsChanged;
            Texts.CollectionChanged += OnEntitiesOrTextsChanged;
        }

        public ObservableCollection<IControl> Controls { get; } = new();
        public ObservableCollection<Composition> Compositions { get; } = new();
        public ObservableCollection<Layer> Layers { get; } = new();
        public ObservableCollection<Material> Materials { get; } = new();
        public ObservableCollection<ITextureSource> Textures { get; } = new();
        public ObservableCollection<Entity> Entities { get; } = new();
        public ObservableCollection<Camera> Cameras { get; } = new();
        public ObservableCollection<LightEntity> Lights { get; } = new();
        public ObservableCollection<BeatModulator> BeatModulators { get; } = new();
        public ObservableCollection<TextEntity> Texts { get; } = new();
        public ObservableCollection<ColorPaletteModifier> ColorPalettes { get; } = new();

        // Replaces the WPF CompositeCollection of the Entities and Texts CollectionViewSources so
        // the layer entity slot swap popup can list both entities and text entities together.
        public ObservableCollection<IControl> EntitiesAndTexts { get; } = new();

        private void OnEntitiesOrTextsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                        AddToEntitiesAndTexts((IControl)item, sender == Texts);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                        EntitiesAndTexts.Remove((IControl)item);
                    break;
                default:
                    ResyncEntitiesAndTexts();
                    break;
            }
        }

        private void AddToEntitiesAndTexts(IControl item, bool isText)
        {
            if (!isText)
            {
                // Entities keep their relative order ahead of the texts block.
                var insertIndex = EntitiesAndTexts.Count(x => x is Entity);
                EntitiesAndTexts.Insert(insertIndex, item);
            }
            else
            {
                EntitiesAndTexts.Add(item);
            }
        }

        private void ResyncEntitiesAndTexts()
        {
            EntitiesAndTexts.Clear();
            foreach (var entity in Entities)
                EntitiesAndTexts.Add(entity);
            foreach (var text in Texts)
                EntitiesAndTexts.Add(text);
        }

        public void AddControl(IControl control, Guid referencerId)
        {
            if (control == null)
                return;

            if (!_referencers.ContainsKey(control.ID))
                _referencers[control.ID] = new HashSet<Guid>();

            _referencers[control.ID].Add(referencerId);

            if (!_controlsById.ContainsKey(control.ID))
            {
                _controlsById[control.ID] = control;
                Controls.Add(control);
                AddToSpecificRepo(control);
            }
        }

        public void RemoveControl(IControl control, Guid referencerId)
        {
            if (control == null || !_referencers.ContainsKey(control.ID))
                return;

            _referencers[control.ID].Remove(referencerId);

            if (_referencers[control.ID].Count == 0)
            {
                _referencers.Remove(control.ID);
                // Only drops the index entry when it points at this very instance, so a control
                // that shares an id with the registered one leaves the registered one reachable
                // exactly as the previous scan over Controls did.
                if (_controlsById.TryGetValue(control.ID, out var indexed) && ReferenceEquals(indexed, control))
                    _controlsById.Remove(control.ID);
                Controls.Remove(control);
                RemoveFromSpecificRepo(control);
            }
        }

        public void RemoveControl(Guid id, Guid referencerId)
        {
            var control = GetControl(id);
            if (control != null)
                RemoveControl(control, referencerId);
        }

        public IControl GetControl(Guid id)
        {
            return _controlsById.TryGetValue(id, out var control) ? control : null;
        }

        public bool HasUsers(IControl control)
        {
            if (!_referencers.ContainsKey(control.ID)) return false;
            return _referencers[control.ID].Count > 0;
        }

        // Last registration for a given manager id wins, so a manager whose id changes after
        // construction (see PrefabManager.RegisterDeleter) simply overwrites its earlier entry.
        public void RegisterDeleter(Guid managerId, Action<IControl> deleter)
        {
            _deleters[managerId] = deleter;
        }

        // Only drops the entry when it still holds this very manager's delegate, so a manager that
        // retires after another one already claimed the same id cannot unregister the live one.
        public void UnregisterDeleter(Guid managerId, Action<IControl> deleter)
        {
            if (deleter == null) return;
            if (_deleters.TryGetValue(managerId, out var registered) && registered.Equals(deleter))
                _deleters.Remove(managerId);
        }

        internal int DeleterCount => _deleters.Count;

        // Deletes control from every manager that currently references it. RemoveControl already
        // drops the control from the repository collections once its last referencer lets go, so
        // this only has to fan the delete out to the referencing managers, not touch the collections.
        public void DeleteEverywhere(IControl control)
        {
            if (control == null) return;
            if (!_referencers.TryGetValue(control.ID, out var referencerIds)) return;

            foreach (var managerId in referencerIds.ToList())
            {
                if (_deleters.TryGetValue(managerId, out var deleter))
                    deleter(control);
            }
        }

        private void AddToSpecificRepo(IControl control)
        {
            var match = typeToAddAction.FirstOrDefault(entry => entry.Type.IsInstanceOfType(control));
            match.Action?.Invoke(control);
        }

        private void RemoveFromSpecificRepo(IControl control)
        {
            var match = typeToRemoveAction.FirstOrDefault(entry => entry.Type.IsInstanceOfType(control));
            match.Action?.Invoke(control);
        }

        private static int GetRenderPriority(IControl c) => c switch
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

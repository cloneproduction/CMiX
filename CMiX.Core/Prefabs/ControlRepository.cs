// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Prefabs
{
    public class ControlRepository : ObservableObject
    {
        private readonly Dictionary<Guid, HashSet<CollectionManager>> _collectionManagers = new();

        private readonly Dictionary<Guid, IControl> _controlsById = new();

        private readonly List<(Type Type, Action<IControl> Action)> typeToAddAction;
        private readonly List<(Type Type, Action<IControl> Action)> typeToRemoveAction;

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
                //(typeof(BeatModulator), c => BeatModulators.Add((BeatModulator)c)),
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
                //(typeof(BeatModulator), c => BeatModulators.Remove((BeatModulator)c)),
                (typeof(TextEntity), c => Texts.Remove((TextEntity)c)),
                (typeof(ColorPaletteModifier), c => ColorPalettes.Remove((ColorPaletteModifier)c))
            };

            Entities.CollectionChanged += OnEntitiesOrTextsChanged;
            Texts.CollectionChanged += OnEntitiesOrTextsChanged;
        }

        public ObservableCollection<IControl> Controls { get; } = new();
        public ObservableCollection<Composition> Compositions { get; } = new();
        public ObservableCollection<Layer> Layers { get; } = new();
        public ObservableCollection<ITextureSource> Textures { get; } = new();
        public ObservableCollection<Entity> Entities { get; } = new();
        public ObservableCollection<Camera> Cameras { get; } = new();
        public ObservableCollection<LightEntity> Lights { get; } = new();
        //public ObservableCollection<BeatModulator> BeatModulators { get; } = new();
        public ObservableCollection<TextEntity> Texts { get; } = new();
        public ObservableCollection<ColorPaletteModifier> ColorPalettes { get; } = new();

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
                AddToSpecificRepo(control);
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
                RemoveFromSpecificRepo(control);
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

// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.Animations;
using CMiX.Core.Colors.Modifiers;
using CMiX.Core.Compositing;
using CMiX.Core.Materials;
using CMiX.Core.Modifiers;
using CMiX.Core.Networking.Servers;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Texturing.Sources;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Prefabs
{
    public class ControlRepository : ObservableObject
    {
        private readonly Dictionary<Guid, HashSet<Guid>> _referencers = new();
        private readonly Dictionary<Type, Action<IControl>> typeToAddAction;
        private readonly Dictionary<Type, Action<IControl>> typeToRemoveAction;

        public ControlRepository()
        {
            typeToAddAction = new Dictionary<Type, Action<IControl>>
            {
                { typeof(Composition), c => Compositions.Add((Composition)c) },
                { typeof(Layer), c => Layers.Add((Layer)c) },
                { typeof(ITextureSource), c => Textures.Add((ITextureSource)c) },
                { typeof(Camera), c => Cameras.Add((Camera)c) },
                { typeof(LightEntity), c => Lights.Add((LightEntity)c) },
                { typeof(Entity), c => Entities.Add((Entity)c) },
                { typeof(Material), c => Materials.Add((Material)c) },
                { typeof(Server), c => Servers.Add((Server)c) },
                { typeof(BeatModifier), c => BeatModifiers.Add((BeatModifier)c) },
                { typeof(TextEntity), c => Texts.Add((TextEntity)c) },
                { typeof(ColorPalette), c => ColorPalettes.Add((ColorPalette)c) }
            };

            typeToRemoveAction = new Dictionary<Type, Action<IControl>>
            {
                { typeof(Composition), c => Compositions.Remove((Composition)c) },
                { typeof(Layer), c => Layers.Remove((Layer)c) },
                { typeof(ITextureSource), c => Textures.Remove((ITextureSource)c) },
                { typeof(Camera), c => Cameras.Remove((Camera)c) },
                { typeof(LightEntity), c => Lights.Remove((LightEntity)c) },
                { typeof(Entity), c => Entities.Remove((Entity)c) },
                { typeof(Material), c => Materials.Remove((Material)c) },
                { typeof(Server), c => Servers.Remove((Server)c) },
                { typeof(BeatModifier), c => BeatModifiers.Remove((BeatModifier)c) },
                { typeof(TextEntity), c => Texts.Remove((TextEntity)c) },
                { typeof(ColorPalette), c => ColorPalettes.Remove((ColorPalette)c) }
            };
        }

        public ObservableCollection<IControl> Controls { get; } = new();
        public ObservableCollection<Composition> Compositions { get; } = new();
        public ObservableCollection<Layer> Layers { get; } = new();
        public ObservableCollection<Material> Materials { get; } = new();
        public ObservableCollection<ITextureSource> Textures { get; } = new();
        public ObservableCollection<Entity> Entities { get; } = new();
        public ObservableCollection<Camera> Cameras { get; } = new();
        public ObservableCollection<LightEntity> Lights { get; } = new();
        public ObservableCollection<Server> Servers { get; } = new();
        public ObservableCollection<BeatModifier> BeatModifiers { get; } = new();
        public ObservableCollection<TextEntity> Texts { get; } = new();
        public ObservableCollection<ColorPalette> ColorPalettes { get; } = new();

        public void AddControl(IControl control, Guid referencerId)
        {
            if (control == null)
                return;

            if (!_referencers.ContainsKey(control.ID))
                _referencers[control.ID] = new HashSet<Guid>();

            var added = _referencers[control.ID].Add(referencerId);

            if (!Controls.Any(x => x.ID == control.ID))
            {
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
            return Controls.FirstOrDefault(x => x.ID == id);
        }

        public bool HasUsers(IControl control)
        {
            if (!_referencers.ContainsKey(control.ID)) return false;
            return _referencers[control.ID].Count > 0;
        }

        private void AddToSpecificRepo(IControl control)
        {
            var match = typeToAddAction.FirstOrDefault(kvp => kvp.Key.IsInstanceOfType(control));
            match.Value?.Invoke(control);
        }

        private void RemoveFromSpecificRepo(IControl control)
        {
            var match = typeToRemoveAction.FirstOrDefault(kvp => kvp.Key.IsInstanceOfType(control));
            match.Value?.Invoke(control);
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

// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.Animations;
using CMiX.Core.Colors.Modifiers;
using CMiX.Core.Compositing;
using CMiX.Core.Materials;
using CMiX.Core.Networking.Servers;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Prefabs
{
    public class ControlRepository : ObservableObject
    {
        private int nameCount = 1;
        private readonly Dictionary<Type, Action<IControl>> typeToAddAction;

        public ControlRepository()
        {
            typeToAddAction = new Dictionary<Type, Action<IControl>>
            {
                { typeof(Composition), c => Compositions.Add((Composition)c) },
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
        }

        public ObservableCollection<IControl> Controls { get; } = new();
        public ObservableCollection<Composition> Compositions { get; } = new();
        public ObservableCollection<Material> Materials { get; } = new();
        public ObservableCollection<ITextureSource> Textures { get; } = new();
        public ObservableCollection<Entity> Entities { get; } = new();
        public ObservableCollection<Camera> Cameras { get; } = new();
        public ObservableCollection<LightEntity> Lights { get; } = new();
        public ObservableCollection<Server> Servers { get; } = new();
        public ObservableCollection<BeatModifier> BeatModifiers { get; } = new();
        public ObservableCollection<TextEntity> Texts { get; } = new();
        public ObservableCollection<ColorPalette> ColorPalettes { get; } = new();

        public void AddControl(IControl control)
        {
            if (control == null || control is EmptyPrefab || Controls.Any(x => x.ID == control.ID))
                return;

            Controls.Add(control);
            NamePrefab(control);
            AddToSpecificRepo(control);
        }

        public IControl GetControl(Guid id)
        {
            return Controls.FirstOrDefault(x => x.ID == id);
        } 


        void NamePrefab(IControl control)
        {
            if(control is IPrefab prefab)
            {
                var prefabs = Controls.OfType<IPrefab>().ToList();

                if(prefabs.FirstOrDefault(x => x.PrefabService.Name.Value == prefab.PrefabService.Name.Value) != null)
                {
                    prefab.PrefabService.Name.Value = control.GetType().Name + "." + string.Format("{0:000}", nameCount);
                    nameCount++;
                }
            }
        }

        private void AddToSpecificRepo(IControl control)
        {
            typeToAddAction.FirstOrDefault(kvp => kvp.Key.IsInstanceOfType(control)).Value?.Invoke(control);
        }
    }
}

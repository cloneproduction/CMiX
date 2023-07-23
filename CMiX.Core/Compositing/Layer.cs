// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Media;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefab;
using CMiX.Core.Services;
using CMiX.Core.Texturing;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Compositing
{
    public partial class Layer : ObservableObject, IPrefab, IModifiable
    {
        public Layer(CompositionService compositionService)
        {
            CompositionService = compositionService;
            Name = new StringValue();
            IsRenaming = new BooleanValue();
            IsSelected = new BooleanValue();

            Visibility = new BooleanValue();
            IsMask = new BooleanValue(false);
            Opacity = new FloatValue(1.0f);
            BackgroundColor = new ColorSelector(Color.FromArgb(255, 128, 128, 128));

            MaskChannel = new GenericValue<MaskChannel>();
            BlendMode = new GenericValue<BlendModeEnum>();
            MaskMode = new GenericValue<MaskMode>();
            Invert = new BooleanValue(false);

            AmbientOcclusion = new AmbientOcclusion();
            ModifierManager = new ModifierManager(new TextureFilterFactory());

            ModelEntityManager = new PrefabManager<Entity>(compositionService);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public StringValue Name { get; set; }
        public BooleanValue Invert { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public BooleanValue IsSelected { get; set; }
        public CompositionService CompositionService { get; set; }
        public BooleanValue Visibility { get; set; }
        public BooleanValue IsMask { get; set; }
        public PrefabManager<Entity> ModelEntityManager { get; set; }
        public ModifierManager ModifierManager { get; set; }
        public GenericValue<BlendModeEnum> BlendMode { get; set; }
        public GenericValue<MaskMode> MaskMode { get; set; }
        public GenericValue<MaskChannel> MaskChannel { get; set; }
        public AmbientOcclusion AmbientOcclusion { get; set; }
        public FloatValue Opacity { get; set; }
        public ColorSelector BackgroundColor { get; set; }

        [ObservableProperty]
        private int selectedTabItemIndex;
    }
}

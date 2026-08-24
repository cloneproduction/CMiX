// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Kuwahara : TextureFilterBase
    {
        public Kuwahara(PrefabService prefabService,
                        GenericValue<float> radius,
                        GenericValue<KuwaharaType> type,
                        GenericValue<float> control,
                        Blend blend)
            : base(prefabService, control, blend)
        {
            Radius = radius;
            Type = type;
        }

        public GenericValue<float> Radius { get; set; }
        public GenericValue<KuwaharaType> Type { get; set; }

        public override IControlModel ToModel()
        {
            var model = new KuwaharaModel
            {
                Radius = (GenericValueModel<float>)Radius.ToModel(),
                Type = (GenericValueModel<KuwaharaType>)Type.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (KuwaharaModel)model;
            LoadBaseModel(m);
            Radius.FromModel(m.Radius);
            Type.FromModel(m.Type);
        }
    }
}

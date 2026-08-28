// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation
{
    public partial class ScaleModifier : Modifier
    {
        public ScaleModifier(PrefabService prefabService,
                             PrefabManager modulatorManager,
                             Channel channelX,
                             Channel channelY,
                             Channel channelZ)
            : base(prefabService, modulatorManager)
        {
            channelX.Label = "X";
            channelY.Label = "Y";
            channelZ.Label = "Z";
            Channels = new List<Channel> { channelX, channelY, channelZ };
        }

        // Convenience accessors into Channels, purely for the view's ChannelVectorXYZ binding -
        // Channels itself stays the source of truth (used by Modifier's own ToModel/FromModel).
        public Channel X => Channels[0];
        public Channel Y => Channels[1];
        public Channel Z => Channels[2];

        public override IControlModel ToModel()
        {
            var model = new ScaleModifierModel();
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (ScaleModifierModel)model;
            LoadBaseModel(m);
        }
    }
}

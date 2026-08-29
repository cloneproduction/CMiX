// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Modulation
{
    // Same idea as ChannelVector3, for a Modifier that owns more than one XY group (e.g.
    // TexCoordModifier's separate Location and Scale), where the Modifier itself can only
    // directly expose one X/Y.
    public class ChannelVector2
    {
        public ChannelVector2(Channel x, Channel y)
        {
            x.Label = "X";
            y.Label = "Y";
            X = x;
            Y = y;
        }

        public Channel X { get; }
        public Channel Y { get; }
    }
}

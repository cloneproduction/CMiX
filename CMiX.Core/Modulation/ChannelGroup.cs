// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Modulation
{
    // A plain grouping of three channels satisfying IChannelGroup - for a Modifier that owns more
    // than one XYZ group (e.g. Grid's separate Width and Phase), where the Modifier itself can
    // only directly expose one X/Y/Z. Built manually from three already-DI-injected Channel
    // instances (not itself DI-resolved), the same way RandomPosition already builds its own
    // Vector3 from individually-injected GenericValue<float> instances - so it doesn't need to be
    // IControl, only its three Channels do, and they already are.
    public class ChannelGroup : IChannelGroup
    {
        public ChannelGroup(Channel x, Channel y, Channel z)
        {
            x.Label = "X";
            y.Label = "Y";
            z.Label = "Z";
            X = x;
            Y = y;
            Z = z;
        }

        public Channel X { get; }
        public Channel Y { get; }
        public Channel Z { get; }
    }
}

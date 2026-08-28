// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Modulation
{
    // Same idea as IChannelGroup, for ChannelVectorXY's two channels instead of three.
    public interface IChannelGroupXY
    {
        Channel X { get; }
        Channel Y { get; }
    }
}

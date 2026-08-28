// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Modulation
{
    // What ChannelVectorXYZ actually needs from its DataContext - satisfied directly by
    // ScaleModifier/PositionModifier/RotationModifier today (each already has X/Y/Z), and by a
    // dedicated small group object for a Modifier that owns more than one XYZ group (e.g. Grid's
    // separate Width and Phase).
    public interface IChannelGroup
    {
        Channel X { get; }
        Channel Y { get; }
        Channel Z { get; }
    }
}

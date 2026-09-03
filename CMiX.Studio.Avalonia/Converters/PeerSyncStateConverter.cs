// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using CMiX.Core.Networking;

namespace CMiX.Studio.Avalonia.Converters
{
    // The lag column of the peer list. Values are the position of the peer and the tail of the
    // stream. The lag is the time behind the tail, in milliseconds.
    public sealed class PeerSyncStateConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count < 2 || values[0] is not StreamPosition peer || values[1] is not StreamPosition tail)
                return AvaloniaProperty.UnsetValue;

            if (peer == tail)
                return "0";

            var lag = tail.Milliseconds - peer.Milliseconds;

            // A peer can be briefly ahead of the tail. Show no lag in that case.
            return lag < 0 ? "0" : $"{lag} ms";
        }
    }
}

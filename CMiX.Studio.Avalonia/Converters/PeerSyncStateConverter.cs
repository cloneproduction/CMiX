// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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

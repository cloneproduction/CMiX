// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace CMiX.Studio.Avalonia.Converters
{
    // True while SyncPeer.Status reports a connect attempt. The status indicator uses it for its
    // .connecting class, because SyncPeer has no boolean for this state.
    public sealed class SyncStatusIsConnectingConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var status = value as string;
            return status is "Connecting" or "Reconnecting";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return BindingOperations.DoNothing;
        }
    }
}

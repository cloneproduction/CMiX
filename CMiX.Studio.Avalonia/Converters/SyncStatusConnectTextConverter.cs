// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace CMiX.Studio.Avalonia.Converters
{
    // The text of the connect button from SyncPeer.Status.
    public sealed class SyncStatusConnectTextConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value as string == "Connecting" ? "Connecting..." : "Connect";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return BindingOperations.DoNothing;
        }
    }
}

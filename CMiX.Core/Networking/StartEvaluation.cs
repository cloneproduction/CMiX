// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Networking
{
    // The result of the start check: what the peer must do, and the stream position it keeps when in sync.
    public readonly record struct StartEvaluation(StartCheck Check, StreamPosition Position);
}

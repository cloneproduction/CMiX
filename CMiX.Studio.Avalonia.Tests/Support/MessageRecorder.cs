// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;

namespace CMiX.Studio.Avalonia.Tests.Support
{
    // A message sender that keeps every message it gets, in order.
    internal sealed class MessageRecorder : IMessageSender
    {
        public List<IMessage> Sent { get; } = new();

        public void SendMessage(IMessage message) => Sent.Add(message);
    }
}

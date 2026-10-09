// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Networking.Messages
{
    public class MessageEnvelope
    {
        public string SenderID { get; set; }
        public Guid MessageID { get; set; }
        public IMessage Payload { get; set; }
    }
}

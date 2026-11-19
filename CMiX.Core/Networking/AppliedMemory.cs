// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Networking
{
    // The message IDs of the last applied foreign entries, to find an entry that arrives twice.
    internal sealed class AppliedMemory
    {
        private const int Limit = 1000;

        private readonly Queue<Guid> _order = new();
        private readonly HashSet<Guid> _ids = new();

        public bool IsDuplicate(Guid messageId)
        {
            if (!_ids.Add(messageId)) return true;

            _order.Enqueue(messageId);
            if (_order.Count > Limit)
                _ids.Remove(_order.Dequeue());

            return false;
        }

        public void Forget()
        {
            _ids.Clear();
            _order.Clear();
        }
    }
}

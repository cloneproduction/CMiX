// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Networking
{
    // A Redis stream entry ID: milliseconds, then a sequence number within that millisecond.
    public readonly record struct StreamPosition(long Milliseconds, long Sequence) : IComparable<StreamPosition>
    {
        public static readonly StreamPosition Zero = new(0, 0);

        public static StreamPosition Parse(string value)
        {
            if (!TryParse(value, out var result))
                throw new FormatException($"'{value}' is not a valid stream position.");

            return result;
        }

        public static bool TryParse(string value, out StreamPosition result)
        {
            result = default;

            if (value is null)
                return false;

            var parts = value.Split('-');
            if (parts.Length != 2)
                return false;

            if (!long.TryParse(parts[0], out var milliseconds))
                return false;

            if (!long.TryParse(parts[1], out var sequence))
                return false;

            result = new StreamPosition(milliseconds, sequence);
            return true;
        }

        public override string ToString() => $"{Milliseconds}-{Sequence}";

        public int CompareTo(StreamPosition other)
        {
            var millisecondsComparison = Milliseconds.CompareTo(other.Milliseconds);
            if (millisecondsComparison != 0)
                return millisecondsComparison;

            return Sequence.CompareTo(other.Sequence);
        }

        public static bool operator <(StreamPosition left, StreamPosition right) => left.CompareTo(right) < 0;
        public static bool operator >(StreamPosition left, StreamPosition right) => left.CompareTo(right) > 0;
        public static bool operator <=(StreamPosition left, StreamPosition right) => left.CompareTo(right) <= 0;
        public static bool operator >=(StreamPosition left, StreamPosition right) => left.CompareTo(right) >= 0;
    }
}

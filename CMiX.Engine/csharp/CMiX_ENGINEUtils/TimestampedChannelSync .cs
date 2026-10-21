// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Reactive.Disposables;
using System.Reactive.Subjects;
using VL.Lib.Reactive;
using VL.Model;
using VL.IO.Redis;


namespace CMiX_ENGINEUtils
{
    /// <summary>
    /// Synchronizes a channel across machines via Redis pub/sub.
    /// Primary publishes (TimeStamp, Value) every frame, secondaries apply the
    /// sample that belongs to TimeStamp - Delay, so all genlocked clients apply
    /// the same sample in the same frame.
    /// </summary>
    [ProcessNode]
    public sealed class TimestampedChannelSync : IDisposable
    {
        private readonly NodeContext _nodeContext;

        private IChannel? _channel;
        private ISync? _sync;              // typed implementation, created per channel type

        public TimestampedChannelSync([Pin(Visibility = PinVisibility.Hidden)] NodeContext nodeContext)
        {
            _nodeContext = nodeContext;
        }

        public void Dispose()
        {
            _sync?.Dispose();
        }

        /// <param name="client">Redis client</param>
        /// <param name="channel">The channel to synchronize (untyped pin, typed internally)</param>
        /// <param name="isPrimary">True on the sender, false on all receivers</param>
        /// <param name="timeStamp">Shared clock, seconds (PTP)</param>
        /// <param name="delay">Receiver delay in seconds, identical on all receivers</param>
        /// <param name="redisKey">Name of the Redis pub/sub channel. Not set or empty: the channel path is used</param>
        /// <param name="serializationFormat">Default MessagePack</param>
        /// <param name="enabled">Disables publishing and subscribing</param>
        /// <param name="latency">Receiver only: TimeStamp minus timestamp of the applied sample</param>
        public void Update(
            RedisClient? client,
            IChannel? channel,
            bool isPrimary,
            double timeStamp,
            double delay,
            out double latency,
            Optional<string> redisKey = default,
            Optional<SerializationFormat> serializationFormat = default,
            bool enabled = true)
        {
            latency = double.NaN;

            // Build the typed implementation once per channel (reflection only here)
            if (!ReferenceEquals(channel, _channel))
            {
                _channel = channel;
                _sync?.Dispose();
                _sync = null;

                if (channel != null)
                {
                    var implType = typeof(Sync<>).MakeGenericType(channel.ClrTypeOfValues);
                    _sync = (ISync)Activator.CreateInstance(implType, _nodeContext, channel)!;
                }
            }

            // Explicit key wins, otherwise fall back to the channel path
            var key = redisKey.HasValue && !string.IsNullOrEmpty(redisKey.Value)
                ? redisKey.Value
                : channel?.Path;

            if (_sync == null || !enabled || client == null || string.IsNullOrEmpty(key))
            {
                _sync?.Update(null, false, timeStamp, delay, default, null, out _);
                return;
            }

            var format = serializationFormat.HasValue
                ? serializationFormat
                : new Optional<SerializationFormat>(SerializationFormat.MessagePack);

            _sync.Update(client, isPrimary, timeStamp, delay, format, key, out latency);
        }

        // Untyped view onto the generic implementation
        private interface ISync : IDisposable
        {
            void Update(RedisClient? client, bool isPrimary, double timeStamp, double delay,
                Optional<SerializationFormat> format, string? redisChannel, out double latency);
        }

        private sealed class Sync<T> : ISync
        {
            private struct Sample
            {
                public double TimeStamp;
                public T? Value;
            }

            private const int RingSize = 64;              // power of two; ~1 s at 60 Hz

            private readonly IChannel<T> _channel;
            private readonly Publish<(double, T?)> _publish;
            private readonly Subscribe<(double, T?)> _subscribe;
            private readonly Subject<(double, T?)> _outgoing = new();
            private readonly SerialDisposable _incoming = new();

            // Ring buffer: written on the Redis thread, read on the main thread
            private readonly Sample[] _ring = new Sample[RingSize];
            private readonly object _ringLock = new();
            private int _head;
            private int _count;

            private double _lastAppliedTimeStamp = double.NaN;
            private string? _redisChannel;

            // Called via Activator.CreateInstance: keep the signature in sync with the shell
            public Sync(NodeContext nodeContext, IChannel channel)
            {
                _channel = (IChannel<T>)channel;        // the shell guarantees T == channel.ClrTypeOfValues
                _publish = new Publish<(double, T?)>(nodeContext);
                _subscribe = new Subscribe<(double, T?)>(nodeContext);
            }

            public void Dispose()
            {
                _incoming.Dispose();
                _publish.Dispose();
                _subscribe.Dispose();
                _outgoing.Dispose();
            }

            public void Update(RedisClient? client, bool isPrimary, double timeStamp, double delay,
                Optional<SerializationFormat> format, string? redisChannel, out double latency)
            {
                latency = double.NaN;

                // Samples received under a previous key must not be applied under the new one
                if (redisChannel != _redisChannel)
                {
                    _redisChannel = redisChannel;
                    ClearRing();
                }

                // Inner nodes only resubscribe when their config changes; null client turns them off
                _publish.Update(isPrimary ? client : null, redisChannel, _outgoing, format);
                var observable = _subscribe.Update(isPrimary ? null : client, redisChannel, format);

                if (client == null || redisChannel == null)
                {
                    _incoming.Disposable = null;
                    return;
                }

                if (isPrimary)
                {
                    _incoming.Disposable = null;
                    // One message per frame, typed value, no boxing
                    _outgoing.OnNext((timeStamp, _channel.Value));
                    return;
                }

                if (_incoming.Disposable == null)
                    _incoming.Disposable = observable.Subscribe(OnMessage);

                if (!TrySelect(timeStamp - delay, out var sample))
                    return;

                latency = timeStamp - sample.TimeStamp;

                // Apply only when the selected sample changed
                if (sample.TimeStamp == _lastAppliedTimeStamp)
                    return;
                _lastAppliedTimeStamp = sample.TimeStamp;

                _channel.SetValueAndAuthor(sample.Value, author: null);
            }

            // Redis thread: append to the ring buffer
            private void OnMessage((double, T?) message)
            {
                lock (_ringLock)
                {
                    _head = (_head + 1) & (RingSize - 1);
                    _ring[_head] = new Sample { TimeStamp = message.Item1, Value = message.Item2 };
                    if (_count < RingSize) _count++;
                }
            }

            // Main thread: newest sample at or before target, scanning backwards from the head.
            // Samples arrive in order from a single publisher, so the first hit is the right one.
            private bool TrySelect(double target, out Sample sample)
            {
                lock (_ringLock)
                {
                    var index = _head;
                    for (var i = 0; i < _count; i++)
                    {
                        ref var s = ref _ring[index];
                        if (s.TimeStamp <= target)
                        {
                            sample = s;
                            return true;
                        }
                        index = (index - 1) & (RingSize - 1);
                    }
                }
                sample = default;
                return false;
            }

            private void ClearRing()
            {
                lock (_ringLock)
                {
                    Array.Clear(_ring);
                    _head = 0;
                    _count = 0;
                }
                _lastAppliedTimeStamp = double.NaN;
            }
        }
    }
}

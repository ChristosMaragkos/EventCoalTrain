#nullable enable
using System;
using System.Collections.Generic;
using EventCoalTrain.EventSource;
using EventCoalTrain.EventStructure;

namespace EventCoalTrain.EventHandling;

internal sealed class DefaultEventBus : IEventBus
{
    private readonly object _gate = new();
    private readonly Dictionary<IEventKey, List<Delegate>> _subscribers = new();

    public event Action<Exception, IEventKey, Delegate>? OnPublishError;

    public IDisposable Subscribe<TPayload>(EventKey<TPayload> key, Action<TPayload> handler)
    {
        if (key is null) throw new ArgumentNullException(nameof(key));
        if (handler is null) throw new ArgumentNullException(nameof(handler));

        lock (_gate)
        {
            if (!_subscribers.TryGetValue(key, out var list))
            {
                list = new List<Delegate>();
                _subscribers[key] = list;
            }
            list.Add(handler);
        }

        return new Unsubscriber(() => Unsubscribe(key, handler));
    }

    public IDisposable Subscribe(Notification notification, Action handler)
    {
        if (notification is null) throw new ArgumentNullException(nameof(notification));
        if (handler is null) throw new ArgumentNullException(nameof(handler));

        lock (_gate)
        {
            if (!_subscribers.TryGetValue(notification.Key, out var list))
            {
                list = new List<Delegate>();
                _subscribers[notification.Key] = list;
            }
            list.Add(handler);
        }

        return new Unsubscriber(() => Unsubscribe(notification, handler));
    }

    public void Unsubscribe<TPayload>(EventKey<TPayload> key, Action<TPayload> handler)
    {
        lock (_gate)
        {
            if (_subscribers.TryGetValue(key, out var list))
            {
                list.Remove(handler);
                if (list.Count == 0)
                    _subscribers.Remove(key);
            }
        }
    }

    public void Unsubscribe(Notification notification, Action handler)
    {
        lock (_gate)
        {
            var key = notification.Key;
            if (_subscribers.TryGetValue(key, out var list))
            {
                list.Remove(handler);
                if (list.Count == 0)
                    _subscribers.Remove(key);
            }
        }
    }

    public void Publish<TPayload>(Packet<TPayload> packet)
    {
        if (packet is null) throw new ArgumentNullException(nameof(packet));

        List<Delegate>? snapshot;
        lock (_gate)
        {
            _subscribers.TryGetValue(packet.Key, out var list);
            snapshot = list is null ? null : [..list];
        }
        if (snapshot is null || snapshot.Count == 0) return;

        foreach (var del in snapshot)
        {
            if (del is not Action<TPayload> action) continue;
            try
            {
                action(packet.Payload);
            }
            catch (Exception ex)
            {
                OnPublishError?.Invoke(ex, packet.Key, del);
            }
        }
    }

    public void Publish(Notification notification)
    {
        if (notification is null) throw new ArgumentNullException(nameof(notification));

        var key = notification.Key;
        List<Delegate>? snapshot;
        lock (_gate)
        {
            _subscribers.TryGetValue(key, out var list);
            snapshot = list is null ? null : [..list];
        }
        if (snapshot is null || snapshot.Count == 0) return;

        foreach (var d in snapshot)
        {
            if (d is Action action)
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    OnPublishError?.Invoke(ex, key, d);
                }
            }
        }
    }

    public void UnsubscribeAll(IEventKey key)
    {
        lock (_gate)
        {
            _subscribers.Remove(key);
        }
    }

    public bool HasSubscribers(IEventKey key)
    {
        lock (_gate)
        {
            return _subscribers.TryGetValue(key, out var list) && list.Count > 0;
        }
    }

    public int Count(IEventKey key)
    {
        lock (_gate)
        {
            return _subscribers.TryGetValue(key, out var list) ? list.Count : 0;
        }
    }

    public void Clear()
    {
        lock (_gate)
        {
            _subscribers.Clear();
        }
    }

    private sealed class Unsubscriber : IDisposable
    {
        private Action? _dispose;
        public Unsubscriber(Action dispose) => _dispose = dispose;
        public void Dispose() => System.Threading.Interlocked.Exchange(ref _dispose, null)?.Invoke();
    }
}


using System;
using System.Collections.Generic;

public static class EventBus
{
    // 事件订阅者字典（事件类型 -> 事件监听器列表）
    private static Dictionary<Type, List<Action<object>>> eventListeners = new Dictionary<Type, List<Action<object>>>();

    /// <summary>
    /// 发布事件
    /// </summary>
    /// <param name="eventData">事件数据</param>
    public static void Publish(object eventData)
    {
        Type eventType = eventData.GetType();
        if (eventListeners.ContainsKey(eventType))
        {
            // 遍历所有的监听器并调用它们
            foreach (var listener in eventListeners[eventType])
            {
                listener.Invoke(eventData);
            }
        }
    }

    /// <summary>
    /// 订阅事件
    /// </summary>
    /// <param name="eventListener">事件监听器</param>
    public static void Subscribe<T>(Action<T> eventListener)
    {
        Type eventType = typeof(T);
        if (!eventListeners.ContainsKey(eventType))
        {
            eventListeners[eventType] = new List<Action<object>>();
        }

        // 添加监听器
        eventListeners[eventType].Add((eventData) => eventListener.Invoke((T)eventData));
    }

    /// <summary>
    /// 取消订阅事件
    /// </summary>
    /// <param name="eventListener">事件监听器</param>
    public static void Unsubscribe<T>(Action<T> eventListener)
    {
        Type eventType = typeof(T);
        if (eventListeners.ContainsKey(eventType))
        {
            eventListeners[eventType].Remove((eventData) => eventListener.Invoke((T)eventData));
        }
    }
}

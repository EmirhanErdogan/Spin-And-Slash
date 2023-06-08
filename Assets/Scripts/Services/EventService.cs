using System;
using System.Collections.Generic;
public static class EventService
{
    public delegate void EventCallback<T>(T data);
    private static Dictionary <Type, List<Delegate>> eventDictionary = new ();
    /// <summary>
    /// This function helper for invoke base event.
    /// </summary>
    /// <param name="e"></param>
    public static void Dispatch(BaseEvent e)
    {
        if (eventDictionary.TryGetValue (e.GetType(), out List<Delegate> listeners))
        {
            foreach (var listener in listeners.ToArray())
            {
                listener.DynamicInvoke(e);
            }
        }
    }
    /// <summary>
    /// This function helper for add listener.
    /// </summary>
    /// <param name="listener"></param>
    /// <typeparam name="T"></typeparam>
    public static void AddListener<T>(EventCallback<T> listener)
    {
        if (eventDictionary.TryGetValue (typeof(T), out List<Delegate> listeners))
        {
            if (listeners.Contains(listener))
                return;
            listeners.Add(listener);
        }
        else
        {
            listeners = new List<Delegate> { listener };
            eventDictionary.Add(typeof(T), listeners);
        }
    }
    /// <summary>
    /// This function helper for remove listener.
    /// </summary>
    /// <param name="listener"></param>
    /// <typeparam name="T"></typeparam>
    public static void RemoveListener<T>(EventCallback<T> listener)
    {
        if (eventDictionary.TryGetValue(typeof(T), out List<Delegate> listeners))
        {
            listeners.Remove(listener);
        }
    }
}
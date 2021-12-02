using System;
using System.Collections.Generic;
using Event_Manager.Events;
using UnityEngine;

namespace Event_Manager
{
    public class EventManager
    {
        private static EventManager instanceInternal;
        public static EventManager instance => instanceInternal ?? (instanceInternal = new EventManager());

        public delegate void EventDelegate<in T>(T e) where T : GameEvent;

        private delegate void EventDelegate(GameEvent e);

        private readonly Dictionary<Type, EventDelegate> delegates = new Dictionary<Type, EventDelegate>();
        private readonly Dictionary<Delegate, EventDelegate> delegateLookup = new Dictionary<Delegate, EventDelegate>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void ClearEvents()
        {
            Debug.Log("Cleared all events");
            instance.delegates.Clear();
            instance.delegateLookup.Clear();
        }

        public static void AddListener<T>(EventDelegate<T> del) where T : GameEvent
        {
            if (instance.delegateLookup.ContainsKey(del))
                return;

            void InternalDelegate(GameEvent e) => del((T)e);
            instance.delegateLookup[del] = InternalDelegate;

            if (instance.delegates.TryGetValue(typeof(T), out var tempDel))
            {
                instance.delegates[typeof(T)] = tempDel + InternalDelegate;
            }
            else
            {
                instance.delegates[typeof(T)] = InternalDelegate;
            }
        }

        public static void RemoveListener<T>(EventDelegate<T> del) where T : GameEvent
        {
            if (instance.delegateLookup.TryGetValue(del, out var internalDelegate))
            {
                if (instance.delegates.TryGetValue(typeof(T), out var tempDel))
                {
                    tempDel -= internalDelegate;
                    if (tempDel == null)
                    {
                        instance.delegates.Remove(typeof(T));
                    }
                    else
                    {
                        instance.delegates[typeof(T)] = tempDel;
                    }
                }

                instance.delegateLookup.Remove(del);
            }
        }

        public static void Trigger(GameEvent e)
        {
            if (instance.delegates.TryGetValue(e.GetType(), out var del))
            {
                del.Invoke(e);
            }
        }
    }
}
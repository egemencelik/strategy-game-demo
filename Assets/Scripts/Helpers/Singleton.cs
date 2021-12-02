using UnityEngine;

namespace Helpers
{
    /// <summary>
    /// Generic singleton class
    /// </summary>
    /// <typeparam name="T">MonoBehavior type</typeparam>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                // Initialize instance
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));
                    
                    // If can't find type, create one
                    if (instance == null)
                    {
                        var singletonObject = new GameObject();
                        instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T) + " (Singleton)";
                    }
                }

                return instance;
            }
        }
    }
}
using UnityEngine;

namespace Emir
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        #region Serializable Fields
    
        [SerializeField] private bool m_isPersistant;
    
        #endregion
        #region Private Fields
    
        /// <summary>
        /// The instance.
        /// </summary>
        private static T instance;
    
        #endregion
        #region Properties
    
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static T Instance
        {
            get
            {
                if ( instance == null )
                {
                    instance = FindObjectOfType<T> ();
                    if ( instance == null )
                    {
                        GameObject obj = new GameObject ();
                        obj.name = typeof ( T ).Name;
                        instance = obj.AddComponent<T> ();
                    }
                }
                return instance;
            }
        }
    
        #endregion
    
    
        /// <summary>
        /// This function called when before first frame.
        /// </summary>
        protected virtual void Awake ()
        {
            if ( instance == null )
            {
                instance = this as T;
                
                if(m_isPersistant)
                    DontDestroyOnLoad ( gameObject );
            }
            else
            {
                Destroy ( gameObject );
            }
        }
    
        /// <summary>
        /// This function called when related game object destroyed.
        /// </summary>
        protected virtual void OnDestroy() { }
    }
}

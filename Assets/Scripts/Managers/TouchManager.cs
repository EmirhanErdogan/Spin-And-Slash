using UnityEngine;
using UnityEngine.EventSystems;

namespace Emir
{
    public class TouchManager : Singleton<TouchManager>
    {
        #region Serializable Fields

        [SerializeField] private TouchInput Touch;

        #endregion

        #region Private Fields

        private float touchVelocity;
        private Vector3 lastTouchPosition;

        #endregion

        /// <summary>
        /// This function called when per frame.
        /// </summary>
        private void Update()
        {
            // touchVelocity = GetTouchDirection().magnitude;
            // lastTouchPosition = Input.mousePosition;
        }

        /// <summary>
        /// This function returns related touch velocity.
        /// </summary>
        /// <param name="isNormalized"></param>
        /// <returns></returns>
        public float GetTouchVelocity(bool isNormalized = false)
        {
            return Mathf.Clamp(touchVelocity, 0, isNormalized ? 1 : Mathf.Infinity);
        }

        /// <summary>
        /// This function returns related touch direction.
        /// </summary>
        /// <param name="isNormalized"></param>
        /// <returns></returns>
        public Vector3 GetTouchDirection(bool isNormalized = false)
        {
            if (isNormalized)
                return (Input.mousePosition - lastTouchPosition).normalized;

            return Input.mousePosition - lastTouchPosition;
        }

        /// <summary>
        /// This function returns true if player touching screen.
        /// </summary>
        /// <returns></returns>
        public bool IsTouching()
        {
            return Input.GetMouseButton(0);
        }

        /// <summary>
        /// This function returns true if player touching up.
        /// </summary>
        /// <returns></returns>
        public bool IsTouchUp()
        {
            return Input.GetMouseButtonUp(0);
        }

        /// <summary>
        /// This function returns true if player touching down.
        /// </summary>
        /// <returns></returns>
        public bool IsTouchDown()
        {
            return Input.GetMouseButtonDown(0);
        }

        public TouchInput GetTouch()
        {
            return Touch;
        }
    }
}
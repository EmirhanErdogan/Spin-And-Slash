using UnityEngine;
using Cinemachine;

namespace Emir
{
    public class CameraManager : Singleton<CameraManager>
    {
        #region Serializable Fields

        [Header("General")] 
        [SerializeField] private Camera m_camera;
        [SerializeField] private CinemachineVirtualCamera m_virtualCamera;

        #endregion

        /// <summary>
        /// This function returns related camera component.
        /// </summary>
        /// <returns></returns>
        public Camera GetCamera()
        {
            return m_camera;
        }
        
        /// <summary>
        /// This function returns related virtual camera component.
        /// </summary>
        /// <returns></returns>
        public CinemachineVirtualCamera GetVirtualCamera()
        {
            return m_virtualCamera;
        }
    }  
}


using UnityEngine;

namespace Emir
{
    public class ThemeManager : Singleton<ThemeManager>
    {
        /// <summary>
        /// This function helper for initialize theme to the world.
        /// </summary>
        /// <param name="theme"></param>
        public void Initialize(Theme theme)
        {
            RenderSettings.skybox = theme.SkyBox;
        }
    }
}
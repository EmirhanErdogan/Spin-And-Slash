using UnityEngine;

namespace Emir
{
    [CreateAssetMenu(menuName = "Emir/Default/Theme", fileName = "Theme", order = 2)]
    public class Theme : ScriptableObject
    {
        [Header("Materials")] 
        public Material SkyBox;

        public Color[] CubeColors;
    }
}
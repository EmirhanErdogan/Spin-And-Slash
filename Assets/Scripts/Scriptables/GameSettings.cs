using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace Emir
{
    [CreateAssetMenu(menuName = "Emir/Default/GameSettings", fileName = "GameSettings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        [Header("Datas")] [ContextMenuItem("Update", "FindLevels")]
        public Level[] Levels;

        public int CurrentWeaponLevel;

        public float PlayerVelocityMinSpeed;
        [Header("Misc")] public string[] GoodFloatingMessages;
        public string[] BadFloatingMessages;
        [Header("Prefabs")] public BulletComponent BulletPrefab;

        public WeaponUIComponent HelmetPrefab;
        public WeaponUIComponent WeaponPrefab;

        [Header("Colors")] public Color UnselectedColor;
        public Color SelectedColor;
        public Color GoodFloatingTextColor;
        public Color BadFloatingTextColor;
        public TMP_Text FloatingTextPrefab;

        public string GetRandomGoodFloatingMessage()
        {
            return GoodFloatingMessages[Random.Range(0, GoodFloatingMessages.Length)];
        }

        public string GetRandomBadFloatingMessage()
        {
            return BadFloatingMessages[Random.Range(0, BadFloatingMessages.Length)];
        }

#if UNITY_EDITOR

        /// <summary>
        /// This function helper for update levels list.
        /// </summary>
        public void FindLevels()
        {
            Levels = null;

            List<Level> foundLevels = new List<Level>();
            Object[] objects = Resources.LoadAll(CommonTypes.EDITOR_LEVELS_PATH);

            foreach (Object targetObject in objects)
            {
                if (targetObject is not Level)
                    continue;

                foundLevels.Add(targetObject as Level);
            }

            Levels = foundLevels.ToArray();
        }

#endif
    }
}
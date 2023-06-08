using UnityEngine;

namespace Emir
{
    [CreateAssetMenu(menuName = "Emir/Default/Level", fileName = "Level", order = 1)]
    public class Level : ScriptableObject
    {
        [Header("General")]
        public int Id;
        public string SceneName;
        public Theme Theme;
        public GameObject LevelPrefab;

    }
}